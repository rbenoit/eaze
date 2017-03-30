using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interview.Green.Job.Business.Dal;
using Interview.Green.Job.Common;

namespace Interview.Green.Job.Business.Facade
{
    public class JobFacade
    {
        //TODO:These values should be configurable.        
        public const int MaximumPageSize = 200;
        public const int MaximumJobsToProcess = 100;

        private Func<IJobDao> CreateJobDao { get; set; }

        public JobFacade()
            : this(() => new JobDao())
        {
        }

        public JobFacade(Func<IJobDao> jobDao)
        {
            CreateJobDao = jobDao;
        }

        /// <summary>
        /// Selects a list of jobs from the data store based on the given filter.
        /// </summary>
        /// <param name="filter">The filter conditions including paging information.</param>
        /// <returns>List of <see cref="JobItem"/> items.</returns>
        public List<JobItem> GetJobList(JobListFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            // Validate filter settings
            if (filter.PageIndex < 0)
                throw new GreenValidationException("PageIndex", "Page index cannot be negative.");
            if (filter.PageSize <= 0)
                throw new GreenValidationException("PageSize", "Page size cannot be less than 1.");
            if (filter.PageSize > MaximumPageSize)
                throw new GreenValidationException("PageSize", string.Format("Page size cannot be greater than {0}.", MaximumPageSize));
            if (filter.CreatedStart.HasValue && filter.CreatedEnd.HasValue && filter.CreatedStart > filter.CreatedEnd)
                throw new GreenValidationException("CreatedStart", "Job created start date cannot be later than end date.");
            if (filter.CreatedStart.HasValue != filter.CreatedEnd.HasValue)
                throw new GreenValidationException("CreatedDateRange", "Both start and end job created dates must be supplied.");

            // Return value from 
            IJobDao dao = CreateJobDao();
            return dao.SelectJobList(filter);
        }

        /// <summary>
        /// Updates a given job's status to 'Cancelled'.
        /// </summary>
        /// <param name="jobId">The id of the job to update.</param>
        /// <returns><c>True</c> if the job is successfully canceled, otherwise <c>False</c>.</returns>
        public bool CancelJob(Guid jobId)
        {
            if (jobId == Guid.Empty)
                throw new ArgumentException("Job id cannot be empty.");

            IJobDao dao = CreateJobDao();
            return dao.UpdateJobStatusCancel(jobId);
        }

        /// <summary>
        /// Updates a given job's status to 'Error' and updates the error information.
        /// </summary>
        /// <param name="jobId">The id of the job to update.</param>
        /// <param name="errorInformation">Error information.</param>
        public void FailJob(Guid jobId, string errorInformation)
        {
            if (jobId == Guid.Empty)
                throw new ArgumentException("Job id cannot be empty.");

            if (errorInformation == null)
                throw new ArgumentNullException("errorInformation");

            IJobDao dao = CreateJobDao();
            dao.UpdateJobStatusError(jobId, errorInformation);
        }

        /// <summary>
        /// Updated a given job's status to 'Completed' with the elapsed execution time.
        /// </summary>
        /// <param name="jobId">The id of the job to update.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        public void CompleteJob(Guid jobId, TimeSpan elapsedTime)
        {
            if (jobId == Guid.Empty)
                throw new ArgumentException("Job id cannot be empty.");

            IJobDao dao = CreateJobDao();
            dao.UpdateJobStatusComplete(jobId, elapsedTime);
        }

        /// <summary>
        /// Picks up jobs for processing, updating them with the given processor key and pickup time information. Can be filtered to one job type or provide <c>null</c> for any job types.
        /// </summary>
        /// <param name="processorKey">The key provided by the processor to identify it.</param>
        /// <param name="maximumJobs">The maximum number of jobs to process.</param>
        /// <param name="jobType">A <see cref="JobType"/> value indicating which job type to process; otherwise <c>null</c> for all job types.</param>
        /// <returns>A list of <see cref="JobItem"/> items representing the jobs being processed.</returns>
        /// <remarks>As the central authority for scheduling, this data access operation uses a serializable transaction scope inside the stored procedure. 
        /// As a result, under severe load it may reach an update deadlock. As a measure against this, a deadlock retry mechanism is utilized. In a real world scenario, this would be investigated
        /// for a more scalable solution.</remarks>
        public List<JobProcessItem> PickupJobs(string processorKey, int maximumJobs, JobType? jobType)
        {
            if (processorKey == null)
                throw new ArgumentNullException("processorKey");
            if (string.IsNullOrWhiteSpace(processorKey))
                throw new GreenValidationException("processorKey", "Processor key cannot be blank.");
            if (maximumJobs < 1)
                throw new GreenValidationException("maximumJobs", "Maximum jobs to process cannot be less than 1.");
            if (maximumJobs > MaximumJobsToProcess)
                throw new GreenValidationException("maximumJobs", string.Format("Maximum jobs to process cannot be greater than {0}.", MaximumJobsToProcess));

            IJobDao dao = CreateJobDao();
            return dao.PickupJobs(processorKey, maximumJobs, jobType);
        }
    }
}
        
