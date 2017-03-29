using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Interview.Green.Job.Business.Dal;
using Interview.Green.Job.Common;

namespace Interview.Green.Web.Scrapper.Tests.Unit
{
    public class MockScrapeJobDao : IScrapeJobDao
    {
        public void InsertScrapeJob(Guid jobId, string createdBy, string url)
        {
        }

        public ScrapeJob SelectScrapeJob(Guid jobId)
        {
            return null;
        }

        public void UpdateScrapeJobStatusComplete(Guid jobId, TimeSpan elapsedTime, HttpStatusCode httpStatus, string responseRaw)
        {
        }
    }
}
