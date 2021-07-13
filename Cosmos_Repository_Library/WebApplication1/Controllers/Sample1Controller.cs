using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test1.Model;
using static WebApplication1.Startup;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Sample1Controller : ControllerBase
    {
        private readonly IRepository<Menu> repository;



        public Sample1Controller(ServiceResolver serviceResolver)
        {
            repository = serviceResolver("Repository");
        }
        [HttpPut]
        public async Task<Menu> Update (Menu menu)
        {
            Menu result = await repository.Update(menu);
            return result;
        }
        [HttpGet]
        public async Task<String> ReadOneStream()
        {
            string query = "SELECT * FROM C WHERE C.refid = 'KJ'";
            QueryRequestOptions options = new QueryRequestOptions();
            options.MaxConcurrency = 1000;
            options.MaxBufferedItemCount = 5;
            options.MaxItemCount = 5;
            String result = await repository.ReadByStreamQuery(query, options);

            return result;
        }
        [HttpPost]
        public async Task<bool> Create(String ID, int Partitionkey)
        {
            bool result = await repository.Delete(ID, new PartitionKey(Partitionkey));

            return result;
        }
    }
}
