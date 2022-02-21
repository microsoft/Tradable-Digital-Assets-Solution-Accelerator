using Azure.Identity;
using Contoso.DigitalGoods.ContosoProfile.Service;
using Contoso.DigitalGoods.CryptoGoodsGift.Service;
using Contoso.DigitalGoods.CryptoGoodsGift.Service.Interfaces;
using Contoso.DigitalGoods.DigitalLocker.App;
using Contoso.DigitalGoods.ProductCatalog.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Solutions.CosmosDB.Security.ManagedIdentity;
using Polly;
using Polly.Extensions.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Contoso.DigitalGoods.Application.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(new Uri(
                    Configuration["Encryption:KeyStorage"]
                    ))
                .ProtectKeysWithAzureKeyVault(
                    new Uri(Configuration["Encryption:KeyIdentifier"]),
                    new DefaultAzureCredential(
                        new DefaultAzureCredentialOptions()
                        {
                            ManagedIdentityClientId = Configuration["Identity:ManagedIdentityId"]
                        }
                    )
                 )
                .SetApplicationName("Contoso DigitalGoods");


            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .AddNewtonsoftJson();

            // services.AddAuthentication(option =>
            // {
            //     option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //     option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // }).AddJwtBearer(options =>
            // {
            //     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            //     {
            //         ValidateIssuer = true,
            //         ValidateAudience = true,
            //         ValidateLifetime = false,
            //         ValidateIssuerSigningKey = true,
            //         ValidIssuer = Configuration["JwtToken:Issuer"],
            //         ValidAudience = Configuration["JwtToken:Audience"],
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtToken:SecretKey"]))
            //     };
            // });

            //adding HTTPContext
            services.AddHttpContextAccessor();

            //Register swagger gen
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contoso DigitalGood's Application Service Endpoint", Version = "v1" });
            });
            services.AddSwaggerGenNewtonsoftSupport();

            //adding DI

            //Adding ConnectionString Accessor
            services.AddSingleton<CosmosConnectionStrings>(x =>
            {
                return ConnectionStringAccessor.Create(Configuration["Identity:SubscriptionId"], Configuration["Identity:ResourceGroupName"], Configuration["Identity:DatabaseAccountName"])
                    .GetConnectionStringsAsync(Configuration["Identity:ManagedIdentityId"]).GetAwaiter().GetResult();
            });

            //Infuse HTTPClient
            services.AddHttpClient<IContosoProfileManager, ContosoProfileManager>()
                                .SetHandlerLifetime(TimeSpan.FromSeconds(5))
                                .AddPolicyHandler(GetRetryPolicy());

            //Adding ContosoProfile Manager
            services.AddTransient<IContosoProfileManager, ContosoProfileManager>();

            //Infuse HTTPClient
            services.AddHttpClient<IDigitalLocker, CryptoGoodLocker>()
                                .SetHandlerLifetime(TimeSpan.FromSeconds(5))
                                .AddPolicyHandler(GetRetryPolicy());

            //Adding DitialLocker Manager
            services.AddTransient<IDigitalLocker, CryptoGoodLocker>();

            //Infuse HTTPClient
            services.AddHttpClient<IGiftManager, GiftManager>()
                              .SetHandlerLifetime(TimeSpan.FromSeconds(5))
                              .AddPolicyHandler(GetRetryPolicy());

            //Adding DigitalGift Manager
            services.AddTransient<IGiftManager, GiftManager>();

            //Adding ProductCatalog Manager
            services.AddTransient<ProductCatalogManager>();

            //Adding ProductCatalog Manager
            services.AddTransient<IProductCatalogManager, ProductCatalogManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contoso DigitalGood's Application Service API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
        }
    }
}
