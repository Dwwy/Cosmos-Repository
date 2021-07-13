using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test1.Model;
using static WebApplication1.Startup;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Sample2Controller : ControllerBase
    {
        private readonly IRepository<Menu> repository;

        public Sample2Controller(ServiceResolver serviceResolver)
        {
            repository = serviceResolver("Repository1");
        }
        // GET: api/<ValuesController>
        [HttpPut]
        public async Task<Menu> Update(Menu menu)
        {
            Menu result = await repository.Update(menu);
            return result;
        }
        [HttpGet]
        public async Task<String> ReadOneStream()
        {
            string query = "SELECT * FROM C ";
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
