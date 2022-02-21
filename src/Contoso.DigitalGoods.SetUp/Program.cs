using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Collections.Generic;
using Contoso.DigitalGoods.TokenService.ServiceWrapper;
using Microsoft.Solutions.NFT;
using Contoso.DigitalGoods.TokenService;
using Contoso.DigitalGoods.DigitalLocker.Service;
using System.Net.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Solutions.CosmosDB.Security.ManagedIdentity;

namespace Contoso.DigitalGoods.SetUp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                        .ConfigureServices((hostContext, services) =>
                        {
                            services.AddHttpClient<DigitalGoodsSetUp>()
                                .SetHandlerLifetime(TimeSpan.FromSeconds(5))
                                .AddPolicyHandler(GetRetryPolicy());

                            services.AddSingleton<CosmosConnectionStrings>(x =>
                            {
                                return ConnectionStringAccessor.Create(hostContext.Configuration["Identity:SubscriptionId"],
                                                                       hostContext.Configuration["Identity:ResourceGroupName"],
                                                                       hostContext.Configuration["Identity:DatabaseAccountName"])
                                    .GetConnectionStringsAsync(hostContext.Configuration["Identity:ManagedIdentityId"]).GetAwaiter().GetResult();
                            });

                            services.AddTransient<DigitalGoodsSetUp>();

                            services.AddTransient<TokenManager>();

                            services.AddHostedService<SetupApplication>();
                        }
                        );
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
