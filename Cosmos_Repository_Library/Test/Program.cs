using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository;
using Test1.Model;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "App.config", Watch = true)]

namespace Test
{
    public class Program
    {
        private  IRepository<Menu> repository;
        private ConcurrentBag<int> time = new ConcurrentBag<int>();
        private ConcurrentBag<int> retry = new ConcurrentBag<int>();
        private static ILog Log = LogManager.GetLogger(typeof(Program));
        private static IHost host;


        public static void Main(string[] args)
        {
            host = CreateHostBuilder(args).Build();
            Program program = host.Services.GetRequiredService<Program>();

        }
        public Program(IRepository<Menu> repository)
        {
            this.repository = repository;
            Console.WriteLine("Beginning operations...");
            Console.WriteLine("How many times do you want to read: ");
            String o = Console.ReadLine();
            this.run(Convert.ToInt32(o));
        }
        public void run(int u)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Parallel.For(0, u, x =>
            {

                Demo();

            });
            while (true)
            {
                if (time.Count() == u)
                {
                    timer.Stop();
                    print();
                    break;
                }
            }
            Log.Info("The total time taken: " + timer.ElapsedMilliseconds);
            Console.WriteLine("Enter 0 to stop running");

            string continues = Console.ReadLine();
            if (continues != "0")
            {
                time.Clear();
                run(u);
            }
            else
            {
                Environment.Exit(0);
            }

        }
        public async Task Demo()
        {
            Console.WriteLine(DateTime.Now.ToString());

            Stopwatch timer = new Stopwatch();
            timer.Start();
            string query = "SELECT * FROM C where C.refid = 'KJ'";
            QueryRequestOptions options = new QueryRequestOptions();
            options.MaxConcurrency = -1;
            options.MaxBufferedItemCount = -1;
            options.MaxItemCount = -1;
            String result = await repository.ReadByStreamQuery(query, options);
            timer.Stop();
            int i = (int)timer.ElapsedMilliseconds;
            int u = Convert.ToInt32(result["count"]);
            time.Add(i);
            retry.Add(u);
            Log.Info(i+ "-"+ u);

        }
        public void print()
        {
            Console.WriteLine("==============================END================================");
            Log.Info("The number of attempts: " + time.Count());
            Log.Info("The minimum time taken: " + time.Min());

            Log.Info("The average time taken: " + time.Average());

            Log.Info("The maximum time taken: " + time.Max());
        }
        /*        public delegate IRepository<Menu> ServiceResolver(string key);
        */
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
                        String connectionString = "AccountEndpoint=https://danielwong2.documents.azure.com:443/;AccountKey=zvwWycQB1dx4JkRFui7F6juRhGa2zXHHT0FrMQoyMcA2IGeybTw2UYwe4hbAnZzwqjnL37t00PfwlqQL56a8Yw==;";

                        CosmosClient cosmosClient = new CosmosClient(connectionString, cosmosClientOptions);
                        return cosmosClient;
                    });
                    services.AddSingleton<IRepository<Menu>,Repository<Menu>>(f => {
                        CosmosClient client = f.GetService<CosmosClient>();
                        String databaseid = "Zeoniq";
                        String containerid = "Menu";
                        int DefaultRetryAttempt = 5;
                        return new Repository<Menu>(client, databaseid, containerid, DefaultRetryAttempt);
                    });
                    /*                    services.AddSingleton<Repository1<Menu>>(f => {
                                            CosmosClient client = f.GetService<CosmosClient>();
                                            String databaseid = "Zeoniq";
                                            String containerid = "Lease";
                                            return new Repository1<Menu>(client, databaseid, containerid);
                                        });*/
                    /*                    services.AddSingleton<ServiceResolver>(serviceProvider => key =>
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
                                        });*/
                });
        }
    }
}
