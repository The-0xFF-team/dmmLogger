using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


static IHost AppStartup()
{
    var builder = new ConfigurationBuilder();

    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Build())
        .CreateBootstrapLogger();

    Log.Logger.Information("starting serilog in a console app...");

    var host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            services.AddSingleton<IDmmService, DmmService>();
        })
        .UseSerilog()
        .Build();

    return host;
}

var host = AppStartup();
var service = ActivatorUtilities.CreateInstance<DmmService>(host.Services);

service.Run();