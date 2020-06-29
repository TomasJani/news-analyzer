using Coravel;
using Data.DataService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Data
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Services.UseScheduler(scheduler =>
            {
                var loadDataHour = host.Services.GetService<IDataSettings>().LoadDataHour;
                // Testing loading
                var service = host.Services.GetService<LoadDataInvocable>();
                service.Load();
                
                scheduler
                    .Schedule<LoadDataInvocable>()
                    .DailyAtHour(loadDataHour);
            });
            host.Run();
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