using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Interview.Green.Web.Scraper.Controllers
{
    public class JobController : ApiController
    {
        // GET: api/job
        public IEnumerable<string> Get()
        {
            return new[] {"value1", "value2"};
        }

        // GET: api/job/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/job
        public void Post([FromBody] string value)
        {
            throw new NotImplementedException();
        }

        // PUT: api/job/5
        public void Put(int id, [FromBody] string value)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/job/5
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}