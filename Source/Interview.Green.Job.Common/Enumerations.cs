using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Green.Job.Common
{
    /// <summary>
    /// Represents the type of job being executed.
    /// </summary>
    public enum JobType
    {
        /// <summary>
        /// A no-op job type that can be used for testing.
        /// </summary>
        None = 0,
        /// <summary>
        /// A job that scrapes a url for a response.
        /// </summary>
        WebScrape = 1
    }

    /// <summary>
    /// Represents the processing lifecycle status of a job.
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// Default status, job has been created and needs processing.
        /// </summary>
        Ready = 0,
        /// <summary>
        /// Job has been picked up for processing.
        /// </summary>
		Processing = 1,
        /// <summary>
        /// Job has completed processing successfully.
        /// </summary>
		Completed = 2,
        /// <summary>
        /// Job has been cancelled and will not be processed.
        /// </summary>
		Cancelled = 3,
        /// <summary>
        /// Job has encountered an error and will not be processed.
        /// </summary>
        Error = 4 
    }

    
}
