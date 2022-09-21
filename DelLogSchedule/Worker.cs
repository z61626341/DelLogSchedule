using NLog;
using Repository.ProjectContext;
using System;
using System.Diagnostics;
using System.IO;
using NLog.Extensions.Logging;
using static System.Reflection.Metadata.BlobBuilder;

namespace DelLogSchedule
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        //public readonly SysSystemLogContext db;

        //public Worker(ILogger<Worker> logger, SysSystemLogContext db)
        //{
        //    _logger = logger;
        //    this.db = db;
        //}

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        // 服務啟動時
        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            Log("DelLogSchedule Service started");
            // 基底類別 BackgroundService 在 StartAsync() 呼叫 ExecuteAsync、
            // 在 StopAsync() 時呼叫 stoppingToken.Cancel() 優雅結束
            await base.StartAsync(stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    int del_num = _config.GetValue<int>("Num", 10000);
                    int time_interval = _config.GetValue<int>("Month", 6);
                    int duration_min = _config.GetValue<int>("Min", 5);
                    Log($"DelLogSchedule will delete num :{del_num}, In {time_interval} months ago from now, By every {duration_min}mins.");
                    Stopwatch timer = new Stopwatch();

                    //var halfYearDate = DateTime.Now.AddMonths(-time_interval);
                    //EF Core
                    //var db = new SysSystemLogContext();
                    //var context = db.SysSystemLogsContext;
                    //var sysLogQuery = context.AsQueryable().Where(c => c.CreateDateTime < halfYearDate);
                    //var sysLogs = sysLogQuery.AsEnumerable().ToList();
                    //foreach (var sysLog in sysLogs)
                    //{
                    //    var sysLogRemove = context.Remove(sysLog);
                    //}

                    using (LogService repo = new LogService(_config))
                    {
                        timer.Restart();
                        Log("- Delete Log SQL Start.");
                        var delLogs = repo.DelSystemLogs(del_num, time_interval);
                        Log("- Delete Log SQL End.");
                        if (delLogs == 0)
                        {
                            Log($"- Delete Log Success CostTime : {(timer.Elapsed - new TimeSpan()).TotalSeconds}s");
                        }
                        else
                        {
                            Log("--- Delete Log Fail. ---");
                        }
                    }
                    var duration_sec = duration_min * 60;
                    Log($"- Wait {duration_sec}s.");
                    //await Task.Delay(duration_min * 60 * 1000, stoppingToken);
                    try
                    {
                        var delaySec = (int)Math.Max((TimeSpan.FromSeconds(duration_sec) - timer.Elapsed).TotalSeconds, 0);
                        Log($"- Actually Wait {delaySec}s");
                        await Task.Delay(delaySec * 1000, stoppingToken);
                    }
                    catch
                    {
                        return;
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        // 服務停止時
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Log("DelLogSchedule Service stopped");
            await base.StopAsync(stoppingToken);
        }

        private void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}|{message}");
            _logger.LogInformation(message);
        }
    }
}