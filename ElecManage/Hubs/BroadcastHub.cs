using ElecManage.Models;
using ElecManage.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElecManage.Hubs
{
    public class BroadcastHub :Hub
    {
        private readonly ElecDBContext _context;
        private readonly IHubContext<BroadcastHub> _hubContext;
        private readonly IHttpClient _httpClient;
        public BroadcastHub(ElecDBContext context, IHttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }
        public Task ReturnDashBoardDataToAll()
        {
            var data = _httpClient.OnGet().Result;
            return Clients.All.SendAsync("RefreshDashBoardData", data);
        }
    }
}
