using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Interview.Green.Job.Business.Dal;
using Interview.Green.Job.Common;

namespace Interview.Green.Job.Business.Facade
{
    public class ScrapeJobFacade : JobFacade
    {

        private Func<IScrapeJobDao> CreateScrapeDao { get; set; }

        public ScrapeJobFacade()
            : this(() => new JobDao(), () => new ScrapeJobDao())
        {
        }

        public ScrapeJobFacade(Func<IJobDao> jobDaoMethod, Func<IScrapeJobDao> scrapeDaoMethod)
            : base(jobDaoMethod)
        {
            CreateScrapeDao = scrapeDaoMethod;
        }

        /// <summary>
        /// Creates a new web scrape job with the given id and url, and records the created by information.
        /// </summary>
        /// <param name="createdBy">A string indicating who is creating this job, which can be used in later searches.</param>
        /// <param name="url">The url to scrape.</param>
        /// <returns><c>True</c> if the insert was successful, otherwise <c>False</c>.</returns>
        public Guid CreateScrapeJob(string createdBy, string url)
        {
            Guid jobId = Guid.NewGuid();

            if (createdBy == null)
                throw new ArgumentNullException("createdBy");
            if (url == null)
                throw new ArgumentNullException("url");
            if (string.IsNullOrWhiteSpace(createdBy))
                throw new GreenValidationException("createdBy", "Creator string cannot be empty.");
            if (string.IsNullOrWhiteSpace(url))
                throw new GreenValidationException("url", "Url cannot be empty.");
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                throw new GreenValidationException("url", "Url is not well formed.");
            if (!(string.Equals("http", uri.Scheme, StringComparison.InvariantCultureIgnoreCase) || string.Equals("https", uri.Scheme, StringComparison.InvariantCultureIgnoreCase)))
                throw new GreenValidationException("url", "Only HTTP and HTTPS schemes supported.");

            IScrapeJobDao dao = CreateScrapeDao();
            dao.InsertScrapeJob(jobId, createdBy, url);

            return jobId;
        }

        /// <summary>
        /// Returns a web scrape job's information.
        /// </summary>
        /// <param name="jobId">The id of the web scrape job to return.</param>
        /// <returns>A <see cref="ScrapeJob"/> representing the web scrape job.</returns>
        public ScrapeJob GetScrapeJob(Guid jobId)
        {
            if (jobId == Guid.Empty)
                throw new ArgumentOutOfRangeException("Job id cannot be empty.");

            IScrapeJobDao dao = CreateScrapeDao();
            return dao.SelectScrapeJob(jobId);
        }

        /// <summary>
        /// Updated a given scrape job's status to 'Completed' with the elapsed execution time and associated scrape data.
        /// </summary>
        /// <param name="jobId">The id of the job to update.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <param name="httpStatus">The http status return code.</param>
        /// <param name="responseRaw">The raw response.</param>
        public void CompleteScrapeJob(Guid jobId, TimeSpan elapsedTime, HttpStatusCode httpStatus, string responseRaw)
        {
            if (jobId == Guid.Empty)
                throw new ArgumentOutOfRangeException("Job id cannot be empty.");
            if (responseRaw == null)
                throw new ArgumentNullException("responseRaw");

            IScrapeJobDao dao = CreateScrapeDao();
            dao.UpdateScrapeJobStatusComplete(jobId, elapsedTime, httpStatus, responseRaw);
        }

        /// <summary>
        /// Processes the web scrape job.
        /// </summary>
        /// <param name="jobId">The web scrape job to process.</param>
        public void ProccessScrapeJob(Guid jobId)
        {
            if (jobId == Guid.Empty)
                throw new ArgumentOutOfRangeException("Job id cannot be empty.");

            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();

                ScrapeJob job = GetScrapeJob(jobId);
                if (job == null)
                    throw new GreenException(string.Format("Could not find job for id: {0}", jobId));
                if (job.Status != JobStatus.Processing)
                    throw new GreenException(string.Format("Invalid job status '{0}', expected Processing for id: {1}", job.Status, jobId));

                HttpWebRequest request = HttpWebRequest.CreateHttp(job.Url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseRaw = (new StreamReader(response.GetResponseStream())).ReadToEnd();

                timer.Stop();

                CompleteScrapeJob(jobId, timer.Elapsed, response.StatusCode, responseRaw);

            }
            catch(Exception ex)
            {
                FailJob(jobId, ex.ToString());
            }

        }
    }
}
