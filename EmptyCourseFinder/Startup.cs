using EmptyCourseFinder;
using EmptyCourseFinder.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]

namespace EmptyCourseFinder
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder config)
        {
            var context = config.GetContext();
            var env = context.EnvironmentName;
            var appPath = context.ApplicationRootPath;

            config.ConfigurationBuilder
                .AddJsonFile(Path.Combine(appPath, "appsettings.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(Path.Combine(appPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<TwilioSettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("Twilio").Bind(settings);
            });

            builder.Services.AddOptions<MongoSettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("Mongo").Bind(settings);
            });
        }
    }
}
