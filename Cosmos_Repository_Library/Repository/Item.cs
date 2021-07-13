using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public abstract class Item:IItem
    {

        [JsonIgnore]
        public int retryattempt { get; set; }
        PartitionKey IItem.PartitionKey => new(GetPartitionKeyValue());

        protected virtual int GetPartitionKeyValue()=> 1;
    }
}
