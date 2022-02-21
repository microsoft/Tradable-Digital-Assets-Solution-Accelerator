using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using System.Windows.Forms;

namespace Contoso.DigitalGoods.ClientApp
{
    internal class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;


                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
                System.Windows.Forms.Application.Run(services.GetRequiredService<DigitalGoodsClient>());

            }
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                        .ConfigureServices((hostContext, services) =>
                        {
                            services.AddAutoMapper(System.AppDomain.CurrentDomain.GetAssemblies());

                            services.AddSingleton<AppPropertyBag>();

                            services.AddHttpClient<DigitalGoodsClient>()
                                        .SetHandlerLifetime(TimeSpan.FromSeconds(5))
                                        .AddPolicyHandler(GetRetryPolicy());
                            services.AddScoped<DigitalGoodsClient>();
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
