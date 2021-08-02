using ElecManage.Hubs;
using ElecManage.Models;
using ElecManage.Services;
using ElecManage.TimerFeatures;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElecDBContext = ElecManage.Models.ElecDBContext;

class view
{
    public string UUID { get; set; }
    public DateTime? Date { get; set; }
    public int? Value { get; set; }
}
namespace ElecManage

{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClient _httpClient;
        private readonly IServiceProvider serviceProvider;
        private IHubContext<BroadcastHub> _hub;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IHttpClient httpClient, IHubContext<BroadcastHub> hub)
        {
            _logger = logger;
            this.serviceProvider = serviceProvider;
            _httpClient = httpClient;
            _hub = hub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //生命週期
                using (var scope = serviceProvider.CreateScope())
                {
                    //資料庫連接
                    var dbCtx = scope.ServiceProvider.GetRequiredService<ElecDBContext>();
                    //取到192.168.140.80:9210/list的資料
                    var datas = _httpClient.OnGet().Result;
                    //把取道的資料塞到資料庫
                    await AddDataAsync(dbCtx, datas);
                    await Calc(dbCtx);
                    await Task.Delay(10000, stoppingToken);
                    //
                }

            }
            async Task AddDataAsync(ElecDBContext dbCtx,List<ElecData>datas)
            {
                foreach (ElecData x in datas)
                {
                    var elecTable = new Electricity();
                    //x是data=從192.168.140.80:9210/list取道的資料
                    elecTable.DeviceId = x.uuid;
                    elecTable.Value = x.value;
                    elecTable.Time = Convert.ToDateTime(x.formatedTime);
                    //資料庫dbCtx 新增一筆 elecTable資料
                    dbCtx.Electricities.Add(elecTable);
                    //資料庫儲存
                    dbCtx.SaveChanges();
                }
                
                await _hub.Clients.All.SendAsync("transferdata", datas);
            }
            async Task Calc(ElecDBContext dbCtx)
            {
                var valueList = dbCtx.Electricities
                    .Select(g => new
                    {
                        sn=g.ElecSn,
                        uuid = g.DeviceId,
                        value = g.Value,
                        time=g.Time.GetDateTimeFormats('g')[0]
                    })
                    .OrderByDescending(x=>x.sn).Take(2)
                    .ToList();
                await _hub.Clients.All.SendAsync("transferdataBar", valueList);
                //計算時間內有幾筆
                //var count = dbCtx.Electricities
                //    .Where(x => x.Time.Date == DateTime.Now.Date && x.Time.Hour == DateTime.Now.Hour && x.DeviceId == "0081F924C0C6").Count();

                //var valueList = dbCtx.Electricities
                //    .Where(x => x.Time.Date == DateTime.Now.Date && x.Time.Hour == DateTime.Now.Hour && x.Time.Minute == DateTime.Now.Minute)
                //    .GroupBy(x => x.DeviceId)
                //    .Select(g => new
                //    {
                //        uuid = g.Key,
                //        //平均電流*110(電壓)/60
                //        SUM = Math.Round((decimal)g.Sum(s => s.Value)/count,2),

                //        Time = g.Max(s => s.Time).GetDateTimeFormats('g')[0]
                //    }).ToList();
                //await _hub.Clients.All.SendAsync("transferdataBar", valueList);
            }
        }
    }
}
