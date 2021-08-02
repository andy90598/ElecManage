using ElecManage.Hubs;
using ElecManage.Models;
using ElecManage.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElecManage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IHttpClient, HttpClient>();
            services.AddControllers();
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
                builder
                    .WithOrigins( "http://localhost", "http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();                   
            }));


            services.AddSignalR(option =>
            {
                //�}�ҩ�����SignalR�Բӿ��~�T��
                option.EnableDetailedErrors = true;
            });

            services.AddSignalR().AddJsonProtocol(options =>
            {
                //�����w�]��camelCase�Ҧ��A���M�e��binding�|����
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ElecManage", Version = "v1" });
            });
            //Scaffold-DbContext "Data Source=./ElecDB.db" Microsoft.EntityFrameworkCore.Sqlite -OutputDir Models -Force
            services.AddDbContext<ElecDBContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("ElecDB"))
            );
            services.AddHttpClient();
            services.AddHostedService<Worker>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElecManage v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<BroadcastHub>("dashboard");
            });
        }
    }
}
