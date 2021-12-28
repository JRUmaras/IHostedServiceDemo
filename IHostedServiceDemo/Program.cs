using System.IO;
using IHostedServiceDemo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Topshelf;
using Host = Microsoft.Extensions.Hosting.Host;

Log.Logger = new LoggerConfiguration()
    .CreateLogger();

var rc = HostFactory.Run(x =>
{
    x.Service<IHost>(s =>
    {
        s.ConstructUsing(_ => Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(cb =>
            {
                cb.SetBasePath(Directory.GetCurrentDirectory());
                cb.AddJsonFile("appsettings.json", false, true);
            })
            .ConfigureServices(sp =>
            {
                sp.AddHostedService<HostedService>(); 
            })
            .UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
            })
            .Build());
        s.WhenStarted(service => service.Start());
        s.WhenStopped(service => service.StopAsync());
    });

    x.StartAutomatically();
    x.RunAsLocalSystem();

    x.SetServiceName("IHostedServiceDemo.ServiceName");
    x.SetDisplayName("IHostedServiceDemo.DisplayName");
    x.SetDescription("IHostedServiceDemo.Description");
});