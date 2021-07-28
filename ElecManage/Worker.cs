using ElecManage.Models;
using ElecManage.Services;
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

namespace ElecManage
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClient _httpClient;
        private readonly IServiceProvider serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IHttpClient httpClient)
        {
            _logger = logger;
            this.serviceProvider = serviceProvider;
            _httpClient = httpClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbCtx = scope.ServiceProvider.GetRequiredService<ElecDBContext>();
                    var datas = _httpClient.OnGet().Result;
                    foreach (ElecData x in datas)
                    {
                        var elecTable = new ElecManage.Models.Electricity();
                        elecTable.DeviceId = x.uuid;
                        elecTable.Value = x.value;
                        elecTable.Time = x.formatedTime;

                        dbCtx.Electricities.Add(elecTable);
                        dbCtx.SaveChanges();
                    }
                }
                await Task.Delay(10000, stoppingToken);
                
            }
        }
    }
}
