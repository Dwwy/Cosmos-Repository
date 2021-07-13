using log4net;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "App.config", Watch = true)]

namespace HttpClientConsole
{
    public class Program
    {
        private static ILog Log = LogManager.GetLogger(typeof(Program));
        private ConcurrentBag<int> time = new ConcurrentBag<int>();
        private ConcurrentBag<int> retry = new ConcurrentBag<int>();
        static async Task Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44338/api/");
                //HTTP GET
                System.Net.ServicePointManager.SecurityProtocol =
            SecurityProtocolType.Tls12
            ;
                Console.WriteLine("Beginning operations...");
                Console.WriteLine("How many times do you want to read: ");
                String times = Console.ReadLine();
                int u = Convert.ToInt32(times);
                Program p = new Program();
                await p.Demo(u, client);

            }
        }
        public async Task Demo(int times, HttpClient client)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Parallel.For(0, times, i => {
                run(client);
            });
            while (true)
            {
                if (time.Count() == times)
                {
                    timer.Stop();
                    break;
                }
            }
            Console.WriteLine("==============================END================================");
            Log.Info("The number of attempts: " + time.Count());
            Log.Info("The minimum time taken: " + time.Min());
            Log.Info("The average time taken: " + time.Average());
            Log.Info("The maximum time taken: " + time.Max());
            Log.Info("Total Time Elapsed: " + timer.ElapsedMilliseconds);
            Console.WriteLine("Enter 0 to stop running");

            string continues = Console.ReadLine();
            if (continues != "0")
            {
                time.Clear();
                await Demo(times, client);
            }
            else
            {
                Environment.Exit(0);
            }
            Console.ReadLine();
        }
        public async Task run(HttpClient client)
        {
            Console.WriteLine(DateTime.Now);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var responseTask = client.GetAsync("Sample1");
            responseTask.Wait();
            var result = responseTask.Result;
            String e = await result.Content.ReadAsStringAsync();
            stopwatch.Stop();
            int i = (int)stopwatch.ElapsedMilliseconds;
            time.Add(i);
            Log.Info(stopwatch.ElapsedMilliseconds);



        }


    }
}
