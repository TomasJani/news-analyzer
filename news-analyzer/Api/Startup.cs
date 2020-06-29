using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api
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
            services.Configure<NewsStoreDatabaseSettings>(
                Configuration.GetSection(nameof(NewsStoreDatabaseSettings)));

            services.AddSingleton<INewsStoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<NewsStoreDatabaseSettings>>().Value);
            
            services.AddSingleton<IMongoClient>(new MongoClient());
            services.AddHostedService<ConfigureMongodbService>();
            
            services.AddSingleton<AuthorService>();
            services.AddSingleton<TagService>();
            services.AddSingleton<ArticleService>();

            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}