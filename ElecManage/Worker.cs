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
                    CalcAsync(dbCtx);
                    CalcHourAsync(dbCtx);
                    CalcDateAsync(dbCtx);
                    CalcMonthAsync(dbCtx);
                    await Task.Delay(10000, stoppingToken);
                    //
                }

            }
            //存資料到資料庫
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
            
            async Task CalcAsync(ElecDBContext dbCtx)
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
            }
            //每天每小時的用量
            async Task CalcHourAsync(ElecDBContext dbCtx)
            {
                //抓前一個小時完整資料
                //var comparedate = DateTime.Now.AddHours(-1);
                var hourList = dbCtx.Electricities
                        //.Where(x => x.Time.Date == comparedate.Date && x.Time.Hour == comparedate.Hour)
                        //x.Time.Date == DateTime.Now.Date && x.Time.Hour== DateTime.Now.Hour - 1
                        //用日期 
                        .GroupBy(x=>new 
                        {
                            Date =x.Time.Date,
                            Hour =x.Time.Hour,
                            ID =x.DeviceId
                        })
                        .Select(g => new
                        {
                            uuid = g.Key.ID,
                            //1小時平均電流
                            SUM = Math.Round((decimal)g.Average(s => s.Value), 2),
                            time=DateTime.Parse(g.Key.Date.GetDateTimeFormats('d')[0]+' '+g.Key.Hour.ToString()+ ":00:00"),
                        }).ToList();
                //signalR 推送資料
                await _hub.Clients.All.SendAsync("transferdataHour", hourList);    
            }
            //每天用量
            async Task CalcDateAsync(ElecDBContext dbCtx)
            {
                var dateList = dbCtx.Electricities
                        //.Where(x => x.Time.Date == comparedate.Date && x.Time.Hour == comparedate.Hour)
                        //x.Time.Date == DateTime.Now.Date && x.Time.Hour== DateTime.Now.Hour - 1
                        //用日期 
                        .GroupBy(x => new
                        {
                            Date = x.Time.Date,
                            ID = x.DeviceId
                        })
                        .Select(g => new
                        {
                            uuid = g.Key.ID,
                            SUM = Math.Round((decimal)g.Average(s => s.Value), 2),
                            time = g.Key.Date.GetDateTimeFormats('d')[0],
                        }).ToList();
                //signalR 推送資料
                await _hub.Clients.All.SendAsync("transferdataDate", dateList);
            }
            //每月用量
            async Task CalcMonthAsync(ElecDBContext dbCtx)
            {
                var dateList = dbCtx.Electricities              
                        //x.Time.Date == DateTime.Now.Date && x.Time.Hour== DateTime.Now.Hour - 1
                        //用日期 
                        .GroupBy(x => new
                        {
                            Date = x.Time.Date.Month,
                            ID = x.DeviceId
                        })
                        .Select(g => new
                        {
                            uuid = g.Key.ID, 
                            SUM = Math.Round((decimal)g.Average(s => s.Value), 2),
                            time = g.Key.Date
                        })                                         
                        .ToList()
                        .OrderBy(z => z.uuid);
                
                //signalR 推送資料               
                await _hub.Clients.All.SendAsync("transferdataMonth", dateList);
            }

        }
    }
}
