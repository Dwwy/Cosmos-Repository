using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public sealed class PartitionKeyPathAttribute : Attribute
        {

            public string Path { get; } = "/id";
            
            public PartitionKeyPathAttribute(string path) =>
                Path = path ?? throw new ArgumentNullException(nameof(path), "A path is required.");
        }
    }

