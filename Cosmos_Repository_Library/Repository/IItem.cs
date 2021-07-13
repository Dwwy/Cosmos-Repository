using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IItem
    {
        int retryattempt { get; set; }

        PartitionKey PartitionKey { get; }

    }
}
