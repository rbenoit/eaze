using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interview.Green.Job.Common;

namespace Interview.Green.Job.Business.Dal
{
    /// <summary>
    /// A sql specific implementation of <see cref="IJobDao"/>.
    /// </summary>
    public class JobDao : BaseDao, IJobDao
    {
        /// <summary>
        /// Creates a new instance of <see cref="JobDao"/> connected to the proper data store.
        /// </summary>
        public JobDao()
            : base()
        {
        }

        /// <summary>
        /// Selects a list of jobs from the data store based on the given filter.
        /// </summary>
        /// <param name="filter">The filter conditions including paging information.</param>
        /// <returns>List of <see cref="JobItem"/> items.</returns>
        public List<JobItem> SelectJobList(JobListFilter filter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a given job's status to 'Cancelled'.
        /// </summary>
        /// <param name="jobId">The id of the job to update.</param>
        /// <returns><c>True</c> if the job is successfully canceled, otherwise <c>False</c>.</returns>
        public bool UpdateJobStatusCancel(Guid jobId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a given job's status to 'Error' and updates the error information.
        /// </summary>
        /// <param name="jobId">The id of the job to update.</param>
        /// <param name="errorInformation">Error information.</param>
        public void UpdateJobStatusError(Guid jobId, string errorInformation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updated a given job's status to 'Completed' with the elapsed execution time.
        /// </summary>
        /// <param name="jobId">The id of the job to update.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        public void UpdateJobStatusComplete(Guid jobId, TimeSpan elapsedTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Picks up jobs for processing, updating them with the given processor key and pickup time information. Can be filtered to one job type or provide <c>null</c> for any job types.
        /// </summary>
        /// <param name="processorKey">The key provided by the processor to identify it.</param>
        /// <param name="maximumJobs">The maximum number of jobs to process.</param>
        /// <param name="jobType">A <see cref="JobType"/> value indicating which job type to process; otherwise <c>null</c> for all job types.</param>
        /// <returns>A list of <see cref="JobItem"/> items representing the jobs being processed.</returns>
        public List<JobItem> PickupJobs(string processorKey, int maximumJobs, JobType? jobType)
        {
            throw new NotImplementedException();
        }
    }
}
