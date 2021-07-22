using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElecManage.Models;
using System.Net.Http;
using System.Text.Json;

namespace ElecManage.Services
{
    public class HttpClient: IHttpClient
    {
        private readonly IHttpClientFactory _clientFactory;
        public List<ElecData> Datas { get; private set; }
        public bool GetDatasError { get; private set; }
        public HttpClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<List<ElecData>> OnGet()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "http://192.168.140.80:9210/list");
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                Datas = await JsonSerializer.DeserializeAsync<List<ElecData>>(responseStream);
            }
            else
            {
                GetDatasError = true;
                Datas = new List<ElecData>();
            }
            return Datas;
        }
    }

    public interface IHttpClient
    {
        public Task<List<ElecData>> OnGet();
    }
}
