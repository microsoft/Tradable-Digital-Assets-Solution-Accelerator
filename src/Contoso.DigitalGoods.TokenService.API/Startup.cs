using Contoso.DigitalGoods.DigitalLocker.Service;
using Contoso.DigitalGoods.TokenService.Interfaces;
using Contoso.DigitalGoods.TokenService.ServiceWrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
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

namespace Contoso.DigitalGoods.TokenService.API
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
            services.AddControllers()
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contoso DigitalGood's Blockchain Service Endpoint", Version = "v1" });
                c.CustomOperationIds(d => (d.ActionDescriptor as ControllerActionDescriptor)?.ActionName);
            });
            services.AddSwaggerGenNewtonsoftSupport();

            //adding DI
            //ConnectionString Accessor
            services.AddSingleton<CosmosConnectionStrings>(x =>
            {
                return ConnectionStringAccessor.Create(Configuration["Identity:SubscriptionId"], Configuration["Identity:ResourceGroupName"], Configuration["Identity:DatabaseAccountName"])
                    .GetConnectionStringsAsync(Configuration["Identity:ManagedIdentityId"]).GetAwaiter().GetResult();
            });


            //Adding ServiceAgent
            services.AddHttpClient<TokenManager>()
                            .SetHandlerLifetime(TimeSpan.FromSeconds(5))
                            .AddPolicyHandler(GetRetryPolicy());

            services.AddTransient<TokenManager>();

            //Adding ServiceAgent
            services.AddHttpClient<UserManager>()
                            .SetHandlerLifetime(TimeSpan.FromSeconds(5))
                            .AddPolicyHandler(GetRetryPolicy());

            services.AddTransient<UserManager>();


            services.AddTransient<DigitalLockerManager>();

            services.AddHttpClient<AccountServiceWrapper>()
                            .SetHandlerLifetime(TimeSpan.FromSeconds(5))
                            .AddPolicyHandler(GetRetryPolicy());

            services.AddTransient<AccountServiceWrapper>();


            //Adding ServiceAgent
            services.AddHttpClient<ITokenServiceAgent, ServiceAgent>()
                            .SetHandlerLifetime(TimeSpan.FromSeconds(5))
                            .AddPolicyHandler(GetRetryPolicy());


            services.AddTransient<ITokenServiceAgent, ServiceAgent>();
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contoso CryptoGood's Blockchain Service API V1");
                //c.RoutePrefix = string.Empty;
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
