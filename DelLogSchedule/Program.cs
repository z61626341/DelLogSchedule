using DelLogSchedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository.ProjectContext;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;


IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "DelLogSchedule";
    })
    .ConfigureAppConfiguration((host, builder) =>
    {
        var map = new Dictionary<string, string>()
           {
               { "--Num", "key1" },
               { "--Month", "key2" },
               { "--Min", "key3" },
           };
        builder.AddCommandLine(args, map);
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddDbContext<SysSystemLogContext>(
            options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
        //services.AddScoped((svc) => new SysSystemLogService(svc.GetRequiredService<SysSystemLogService>()));
        services.AddSingleton<LogService>();
    })
    .ConfigureLogging((host, builder) =>
    {
        builder.AddConsole();
        builder.ClearProviders();
        builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        builder.AddNLog(host.Configuration);
    })
    .Build();

await host.RunAsync();
