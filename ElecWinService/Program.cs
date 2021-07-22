using ElecManage.Models;
using ElecManage.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HttpClient = ElecManage.Services.HttpClient;

namespace ElecWinService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.AddDbContext<ElecDBContext>(option =>
                    option.UseSqlite("Data Source=../ElecManage/ElecDB.db")
                    );
                    services.AddSingleton<IHttpClient, HttpClient>();
                    services.AddHttpClient();
                });
    }
}
