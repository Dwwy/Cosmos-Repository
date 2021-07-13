using Newtonsoft.Json;
using Repository;
using System;

namespace Test1.Model
{
    //The column name of the partition key in the database must match the name in the model object (case-sensitive).
    [PartitionKeyPath("/storeid")]

    public class Menu : Item
    {
        
        [JsonIgnore]
        public int storeid { get; set; }

        public String id { get; set; } = Guid.NewGuid().ToString();

        public String refid { get; set; }

        public OrderMenus[] OrderMenus { get; set; }
        public String LastUpdated { get; set; } = DateTime.Now.ToString();

        [JsonProperty("storeid")]
        public int SyntheticPartitionKey =>
            storeid;

        protected override int GetPartitionKeyValue() => SyntheticPartitionKey;
        
    }
}
