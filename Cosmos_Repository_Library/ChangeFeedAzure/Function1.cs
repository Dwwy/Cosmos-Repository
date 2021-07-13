using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Test1.Model;

namespace ChangeFeedAzure
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([CosmosDBTrigger(
            databaseName: "Zeoniq",
            collectionName: "Menu",
            ConnectionStringSetting = "CosmosDB_Connection",
            LeaseCollectionName = "Lease",
            StartFromBeginning = true,
            LeaseCollectionPrefix = "2")]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                foreach (var item in input)
                {
                    Menu m = JsonConvert.DeserializeObject<Menu>(item.ToString());
                    log.LogInformation($"Inserted item ID: {m.id}");
                }
            }
        }
    }
}
