using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interview.Green.Job.Business.Dal;
using Interview.Green.Job.Common;

namespace Interview.Green.Web.Scrapper.Tests.Unit
{
    /// <summary>
    /// A mock job data access provider to allow unit testing of busines / facade layer that contains no functional implementation.
    /// </summary>
    public class MockJobDao : IJobDao
    {
        public void InsertNoOpJob(Guid jobId, string createdBy)
        {
        }

        public List<JobProcessItem> PickupJobs(string processorKey, int maximumJobs, JobType? jobType)
        {
            return new List<JobProcessItem>();
        }

        public List<JobItem> SelectJobList(JobListFilter filter)
        {
            return new List<JobItem>();
        }

        public bool UpdateJobStatusCancel(Guid jobId)
        {
            return true;
        }

        public void UpdateJobStatusComplete(Guid jobId, TimeSpan elapsedTime)
        {
        }

        public void UpdateJobStatusError(Guid jobId, string errorInformation)
        {
        }
    }
}
