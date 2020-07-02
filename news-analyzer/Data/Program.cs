using System.Threading.Tasks;
using Coravel;
using Data.DataService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Data
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var settings = host.Services.GetService<IDataSettings>();
            var service = host.Services.GetService<LoadDataInvocable>();
            
            if (settings.InitialDaysLoad != 0)
                await service.InitialLoad(settings.InitialDaysLoad);
            
            host.Services.UseScheduler(scheduler =>
            {
                scheduler
                    .Schedule<LoadDataInvocable>()
                    .DailyAtHour(settings.LoadDataHour);
            });
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    
                    services.Configure<DataSettings>(
                        configuration.GetSection(nameof(DataSettings)));

                    services.AddSingleton<IDataSettings>(sp =>
                        sp.GetRequiredService<IOptions<DataSettings>>().Value);
                    
                    services.AddScheduler();

                    services.AddTransient<LoadDataInvocable>();
                });
    }
}