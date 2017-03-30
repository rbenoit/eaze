using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Interview.Green.Job.Business.Dal;
using Interview.Green.Job.Business.Facade;
using Interview.Green.Job.Common;
using Interview.Green.Web.Scrapper.Models;

namespace Interview.Green.Web.Scraper.Controllers
{
    /// <summary>
    /// Controller for api/job endpoints.
    /// </summary>
    /// <remarks>This class should do a better job of wrapping known exceptions and passing more consistent data back to the client, however due to time constraints this was not completed.</remarks>
    public class JobController : ApiController
    {
        private Func<IJobDao> JobDaoMethod { get; set; }
        private Func<IScrapeJobDao> ScrapeJobDaoMethod { get; set; }

        /// <summary>
        /// Default constructor uses default (sql) data access objects.
        /// </summary>
        public JobController()
            : this(() => new JobDao(), () => new ScrapeJobDao())
        {
        }

        /// <summary>
        /// Overloaded constructor allows interception of data access providers.
        /// </summary>
        /// <param name="jobDaoMethod"></param>
        /// <param name="scrapeJobDaoMethod"></param>
        public JobController(Func<IJobDao> jobDaoMethod, Func<IScrapeJobDao> scrapeJobDaoMethod)
        {
            JobDaoMethod = jobDaoMethod;
            ScrapeJobDaoMethod = scrapeJobDaoMethod;
        }

        private JobFacade CreateJobFacade()
        {
            return new JobFacade(JobDaoMethod);
        }

        private ScrapeJobFacade CreateScrapeJobFacade()
        {
            return new ScrapeJobFacade(JobDaoMethod, ScrapeJobDaoMethod);
        }

        // GET: api/job
        public HttpResponseMessage Get([FromUri] JobListFilter filter)
        {
            JobFacade jobFacade = CreateJobFacade();

            try
            {
                List<JobItem> jobItems = jobFacade.GetJobList(filter);

                return Request.CreateResponse<IEnumerable<jobResult>>(HttpStatusCode.OK, jobItems.Select(i => new jobResult(i)));
            } 
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/job/5
        public HttpResponseMessage Get(Guid id)
        {
            ScrapeJobFacade jobFacade = CreateScrapeJobFacade();

            try
            {
                ScrapeJob job = jobFacade.GetScrapeJob(id);

                if (job != null)
                    return Request.CreateResponse<scrapeJobResult>(HttpStatusCode.OK, new scrapeJobResult(job));

                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No 'WebScrape' job found with the given id.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST: api/job
        public HttpResponseMessage Post([FromBody] jobRequest req)
        {
            ScrapeJobFacade jobFacade = CreateScrapeJobFacade();

            try
            {
                Guid jobId = jobFacade.CreateScrapeJob(req.requestedBy, req.url);
                ScrapeJob job = jobFacade.GetScrapeJob(jobId);

                if (job == null)
                    throw new GreenException(string.Format("Could not find created job: {0}", jobId));
                
                return Request.CreateResponse<scrapeJobResult>(HttpStatusCode.OK, new scrapeJobResult(job));
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /* PUT: api/job/5
        public void Put(int id, [FromBody] string value)
        {
            throw new NotImplementedException();
        }*/

        // DELETE: api/job/5
        public HttpResponseMessage Delete(Guid id)
        {
            JobFacade jobFacade = CreateJobFacade();

            try
            {
                if (jobFacade.CancelJob(id))
                    return new HttpResponseMessage(HttpStatusCode.OK);
                else
                    return new HttpResponseMessage(HttpStatusCode.NotFound);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}