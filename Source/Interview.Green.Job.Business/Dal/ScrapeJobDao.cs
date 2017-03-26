using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interview.Green.Job.Common;

namespace Interview.Green.Job.Business.Dal
{
    /// <summary>
    /// A sql specific implementation of <see cref="IScrapeJobDao"/>.
    /// </summary>
    public class ScrapeJobDao : BaseDao, IScrapeJobDao
    {
        /// <summary>
        /// Creates a new web scrape job with the given id and url, and records the created by information.
        /// </summary>
        /// <param name="jobId">The id of the job.</param>
        /// <param name="createdBy">A string indicating who is creating this job, which can be used in later searches.</param>
        /// <param name="url">The url to scrape.</param>
        /// <returns><c>True</c> if the insert was successful, otherwise <c>False</c>.</returns>
        public bool InsertScrapeJob(Guid jobId, string createdBy, string url)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a web scrape job's information.
        /// </summary>
        /// <param name="jobId">The id of the web scrape job to return.</param>
        /// <returns>A <see cref="ScrapeJob"/> representing the web scrape job.</returns>
        public ScrapeJob SelectScrapeJob(Guid jobId)
        {
            throw new NotImplementedException();
        }
    }
}
