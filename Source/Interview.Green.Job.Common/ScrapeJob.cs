using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Green.Job.Common
{
    /// <summary>
    /// Represents a WebScrape job, including all Job state plus additional WebScrape specific information.
    /// </summary>
    public class ScrapeJob : JobItem
    {
        /// <summary>
        /// Gets or sets the url for this scrape job.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the raw response string for this scrape job.
        /// </summary>
        public string ResponseRaw { get; set; }

        /// <summary>
        /// Gets or sets the http response code for this scrape job.
        /// </summary>
        public HttpStatusCode? HttpStatus { get; set; }
    }
}
