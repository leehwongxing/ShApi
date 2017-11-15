using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private IOptionsSnapshot<Configs.Mongo> Options { get; set; }

        public ValuesController(IOptionsSnapshot<Configs.Mongo> options)
        {
            Options = options;
        }

        // GET api/values
        [HttpGet]
        public Dictionary<string, string> Get()
        {
            var DnC = new Dictionary<string, string>
            {
                { "First", Options.Value.ConnectString },
                { "Second", Options.Value.Database },
                { "Third", Options.Value.Password }
            };

            return DnC;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}