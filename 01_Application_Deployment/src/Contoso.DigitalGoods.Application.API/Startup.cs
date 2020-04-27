using Contoso.DigitalGoods.ContosoProfile.Service;
using Contoso.DigitalGoods.CryptoGoodsGift.Service;
using Contoso.DigitalGoods.CryptoGoodsGift.Service.Interfaces;
using Contoso.DigitalGoods.DigitalLocker.App;
using Contoso.DigitalGoods.ProductCatalog.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
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
                    Configuration["Encryption:KeyIdentifier"],
                    Configuration["Encryption:ClientID"],
                    Configuration["Encryption:ClientSecret"]
                    )
                .SetApplicationName("Contoso CryptoGoods");


            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JwtToken:Issuer"],
                    ValidAudience = Configuration["JwtToken:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtToken:SecretKey"]))
                };
            });

            //adding HTTPContext
            services.AddHttpContextAccessor();

            //Register swagger gen
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contoso CryptoGood's Application Service Endpoint", Version = "v1" });
                //var filePath = Path.Combine(System.AppContext.BaseDirectory, "Contoso.CryptoGoods.Application.API.xml");
                //c.IncludeXmlComments(filePath);
            });
            services.AddSwaggerGenNewtonsoftSupport();

            //adding DI
            //Adding ContosoProfile Manager
            services.AddTransient<IContosoProfileManager, ContosoProfileManager>(c =>
            {
                return new ContosoProfileManager(Configuration["Values:offchain_connectionstring"],
                    Configuration["Values:profileCollectionName"],
                    Configuration["Values:tokenAPIURL"],
                    Configuration["Values:PartyID"],
                    Configuration["Values:BlockchainNetworkID"]
                    );

            });

            //Adding DitialLocker Manager
            services.AddTransient<IDigitalLocker, CryptoGoodLocker>(c =>
            {
                return new CryptoGoodLocker(new ProductCatalogManager(
                    Configuration["Values:offchain_connectionstring"],
                    Configuration["Values:productCatalogCollectionName"]),
                    Configuration["Values:tokenAPIURL"]);

            });

            //Adding DigitalGift Manager
            services.AddTransient<IGiftManager, GiftManager>(c =>
            {
                return new GiftManager(Configuration["Values:offchain_connectionstring"],
                    Configuration["Values:giftCollectionName"],
                    Configuration["Values:ContosoID"],
                    Configuration["Values:tokenAPIURL"],
                    Configuration["Values:PartyID"],
                    Configuration["Values:BlockchainNetworkID"]

                    );

            });

            //Adding ProductCatalog Manager
            services.AddTransient<IProductCatalogManager, ProductCatalogManager>(c =>
            {
                return new ProductCatalogManager(
                    Configuration["Values:offchain_connectionstring"],
                    Configuration["Values:productCatalogCollectionName"]);

            });

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

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contoso CryptoGood's Application Service API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
