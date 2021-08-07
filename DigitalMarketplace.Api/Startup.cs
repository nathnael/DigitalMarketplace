using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;

namespace DigitalMarketplace.Api
{
    public class Startup
    {
        // we use appsettings.json as configuration file to store key-value pairs, 
        // like database connection string, etc
        public IConfiguration configuration { get; }

        // we are using Autofac container here to add services and setup Dependency Injection
        public static IContainer container { get; private set; }

        // Constructor: initialize configuration 
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder().
                            SetBasePath(env.ContentRootPath).
                            AddJsonFile("appsettings.json", false, true).
                            AddEnvironmentVariables();
            configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    p => p.AllowAnyOrigin().
                        AllowAnyHeader().
                        AllowAnyMethod()
                        );
            });
            var builder = new ContainerBuilder();

            builder.Populate(services);

            container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFctory, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });

            app.UseCors("AllowAll");
            //app.UseMvc();
            applicationLifetime.ApplicationStopped.Register(() => container.Dispose());
        }
    }
}
