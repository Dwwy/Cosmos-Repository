using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository;
using Test1.Model;

namespace LibraryTest
{
    public class Program
    {
        private static IRepository<Menu> repository;

        public async static Task Main(string[] args )
        {
            var host = CreateHostBuilder(args).Build();
            await host.Services.GetRequiredService<Program>().print();


        }
        public Program (ServiceResolver serviceResolver)
        {
            run(serviceResolver);
        }
        public static void run(ServiceResolver serviceResolver)
        {
            repository = serviceResolver("Repository");
            //Depends on which container to access.
/*            repository = serviceResolver("Repository1");
*/

        }
        public async Task print()
        {
            string query = "SELECT * FROM C";
            QueryRequestOptions options = new QueryRequestOptions();
            options.MaxConcurrency = -1;
            options.MaxBufferedItemCount = -1;
            options.MaxItemCount = -1;
            String result = await repository.ReadByStreamQuery(query, options);
            Console.WriteLine(result);
        }
        public delegate IRepository<Menu> ServiceResolver(string key);

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<Program>();
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
                        int DefaultRetryAttempt = 5;
                        return new Repository<Menu>(client, databaseid, containerid, DefaultRetryAttempt);
                    });
                    services.AddSingleton<Repository1<Menu>>(f =>
                    {
                        CosmosClient client = f.GetService<CosmosClient>();
                        String databaseid = "Database";
                        String containerid = "Container2";
                        int DefaultRetryAttempt = 5;
                        return new Repository1<Menu>(client, databaseid, containerid,DefaultRetryAttempt);
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
                });
        }
    }
}
