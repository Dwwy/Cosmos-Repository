using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Repository;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Test1.Model;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public delegate IRepository<Menu> ServiceResolver(string key);
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication1", Version = "v1" });
            });
            HttpClientHandler socketsHttpHandler = new HttpClientHandler();
            socketsHttpHandler.MaxConnectionsPerServer = 10;
            services.AddSingleton<HttpClientHandler>(socketsHttpHandler);

            services.AddSingleton<CosmosClient>(serviceProvider =>
            {
                HttpClientHandler socketsHttpHandler = serviceProvider.GetRequiredService<HttpClientHandler>();

                CosmosClientOptions cosmosClientOptions = new CosmosClientOptions()
                {
                    //Application Region should be set accordingly.
                    //By setting the ApplicationRegion, there would be a possibility that when connection fail to establish to the region,
                    //it will establish the connection again to other region eg. SouthEastAsia or JapanEast
                    //Either ApplicationRegion or LimitToEndPoint can be set, not both together
                    ApplicationRegion = Regions.EastAsia,
                    HttpClientFactory = () => new HttpClient(socketsHttpHandler, disposeHandler: false),
                    ConnectionMode = ConnectionMode.Direct,
                    //Important to allow Bulk Execution for better performance
                    AllowBulkExecution = true,
                    //Reuse port
                    PortReuseMode = PortReuseMode.PrivatePortPool,
                    IdleTcpConnectionTimeout = TimeSpan.FromMinutes(10),
                    //Good for reducing latency on write operation
                    EnableContentResponseOnWrite = false,
                    //Concurrent Connection Settings for Cosmos Client Options
                    //MaxTCPConnectionPerEndpoint is default at 65,535 connections
                    MaxRequestsPerTcpConnection = 8,
                    MaxRetryAttemptsOnRateLimitedRequests = 1,
                    //Good to ensure the connection only connect to the region you desire
                    /*                    LimitToEndpoint = true
                    */
                };
                String connectionString = "Connection String Here";
                CosmosClient cosmosClient = new CosmosClient(connectionString, cosmosClientOptions);
                return cosmosClient;
            });
            services.AddSingleton<Repository<Menu>>(f => {
                CosmosClient client = f.GetService<CosmosClient>();
                String databaseid = "Database";
                String containerid = "Container1";
                int DefaultRetryAttempts = 5;
                return new Repository<Menu>(client, databaseid, containerid,DefaultRetryAttempts);
            });
            services.AddSingleton<Repository1<Menu>>(f =>
            {
                CosmosClient client = f.GetService<CosmosClient>();
                String databaseid = "Database";
                String containerid = "Container1";
                int DefaultRetryAttempts = 5;
                return new Repository1<Menu>(client, databaseid, containerid,DefaultRetryAttempts);
            });
            services.AddSingleton<ServiceResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case nameof(Repository<Menu>):
                        return serviceProvider.GetService<Repository<Menu>>();
                    case nameof(Repository1<Menu>):
                        return serviceProvider.GetService<Repository1<Menu>>();
                    default:
                        return null;
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
