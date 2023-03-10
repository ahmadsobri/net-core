using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Core.Authentication.Auth;
using Net.Core.Authentication.Entities.Contexts;
using Net.Core.Authentication.Logs;
using Net.Core.Authentication.Utilities;
using NLog;
using System;
using System.IO;
using System.Reflection;

namespace Net.Core.Authentication
{
    public class Startup
    {
        private Settings _settings;
        private string basePackage = Assembly.GetExecutingAssembly().GetName().Name;
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; private set; }
        public ILifetimeScope AutofacContainer { get; private set; }

        // ConfigureServices is where you register dependencies. This gets
        // called by the runtime before the ConfigureContainer method, below.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddOptions()
                .Configure<Settings>(options => Configuration
                .GetSection(nameof(Settings))
                .Bind(options))
                .AddSingleton(Configuration);

            _settings = Configuration.GetSection(nameof(Settings)).Get<Settings>();

            services.AddTransient<ContextFactory>();

            services.AddDbContext<AuthContext>(options =>
            {
                if (!_settings.DatabaseSettings.UseInMemory)
                {
                    options.UseMySQL(_settings.DatabaseSettings.BuildConnectionString());
                }
                else
                {
                    //// Option 2
                    //options.UseSqlServer(_settings.ConnectionStrings.MyConnection);
                }
            });

            //filter
            services.AddScoped<ValidateModelAttribute>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //claims principal
            services.AddHttpContextAccessor();
            services.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Add any Autofac modules or registrations.
            // This is called AFTER ConfigureServices so things you
            // register here OVERRIDE things registered in ConfigureServices.
            //
            // You must have the call to `UseServiceProviderFactory(new AutofacServiceProviderFactory())`
            // when building the host or this won't be called.
            builder.RegisterModule(new Autoload());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment()) // or u can use : if (env.isenvironment("development")),if (env.isproduction()),if (env.isstaging())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // exception middleware
            app.UseExceptionMiddleware();

            // auth middleware
            app.UseAuthMiddleware();

            //route middleware
            app.UseRouteMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();
        }
    }
}
