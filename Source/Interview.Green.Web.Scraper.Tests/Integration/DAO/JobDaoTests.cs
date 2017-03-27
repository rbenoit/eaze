using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Interview.Green.Job.Common;
using Interview.Green.Job.Business.Dal;
using System.Threading.Tasks;

namespace Interview.Green.Web.Scrapper.Tests.Integration.DAO
{
    [TestClass]
    public class JobDaoTests
    {
        [TestInitialize()]
        public void Init()
        {
            // Clear any outstanding "Ready" jobs in test database
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Job] SET [JobStatus] = 4, [ErrorInformation] = 'Cancelled by test init' WHERE [JobStatus] = 0 AND [JobType] = 0", connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        [TestMethod]
        public void NoOpJobBatteryCancelTest()
        {
            // Arrange
            Guid expectedJobId = Guid.NewGuid();
            string expectedCreatedBy = string.Format("Test:{0}", expectedJobId);

            JobDao dao = new JobDao();

            // Create job
            dao.InsertNoOpJob(expectedJobId, expectedCreatedBy);

            // Verify job comes back on select
            List<JobItem> list = dao.SelectJobList(new JobListFilter() { CreatedBy = expectedCreatedBy });
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(expectedJobId, list[0].JobId);
            Assert.AreEqual(expectedCreatedBy, list[0].CreatedBy);
            Assert.IsNull(list[0].ProcessingComplete);
            Assert.IsNull(list[0].ProcessingPickup);
            Assert.IsNull(list[0].ProcessorKey);
            Assert.AreEqual(0, list[0].RetryCount);
            Assert.AreEqual(JobStatus.Ready, list[0].Status);
            Assert.AreEqual(JobType.None, list[0].Type);
            Assert.IsNull(list[0].ErrorInformation);
            Assert.IsNull(list[0].ElapsedTime);

            // Cancel Job
            bool result = dao.UpdateJobStatusCancel(expectedJobId);
            Assert.IsTrue(result);

            // Verify job comes back on select with updated status
            list = dao.SelectJobList(new JobListFilter() { CreatedBy = expectedCreatedBy });
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(expectedJobId, list[0].JobId);
            Assert.AreEqual(expectedCreatedBy, list[0].CreatedBy);
            Assert.IsNull(list[0].ProcessingComplete);
            Assert.IsNull(list[0].ProcessingPickup);
            Assert.IsNull(list[0].ProcessorKey);
            Assert.AreEqual(0, list[0].RetryCount);
            Assert.AreEqual(JobStatus.Cancelled, list[0].Status);
            Assert.AreEqual(JobType.None, list[0].Type);
            Assert.IsNull(list[0].ErrorInformation);
            Assert.IsNull(list[0].ElapsedTime);

            // Verify cannot cancel again
            result = dao.UpdateJobStatusCancel(expectedJobId);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NoOpJobBatteryErrorTest()
        {
            // Arrange
            Guid expectedJobId = Guid.NewGuid();
            string expectedCreatedBy = string.Format("Test:{0}", expectedJobId);
            string expectedErrorInformation = (new ApplicationException("Test error information")).ToString();

            JobDao dao = new JobDao();

            // Create job
            dao.InsertNoOpJob(expectedJobId, expectedCreatedBy);

            // Verify job comes back on select
            List<JobItem> list = dao.SelectJobList(new JobListFilter() { CreatedBy = expectedCreatedBy });
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(expectedJobId, list[0].JobId);
            Assert.AreEqual(expectedCreatedBy, list[0].CreatedBy);
            Assert.IsNull(list[0].ProcessingComplete);
            Assert.IsNull(list[0].ProcessingPickup);
            Assert.IsNull(list[0].ProcessorKey);
            Assert.AreEqual(0, list[0].RetryCount);
            Assert.AreEqual(JobStatus.Ready, list[0].Status);
            Assert.AreEqual(JobType.None, list[0].Type);
            Assert.IsNull(list[0].ErrorInformation);
            Assert.IsNull(list[0].ElapsedTime);

            // Error Job
            dao.UpdateJobStatusError(expectedJobId, expectedErrorInformation);

            // Verify job comes back on select with updated status
            list = dao.SelectJobList(new JobListFilter() { CreatedBy = expectedCreatedBy });
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(expectedJobId, list[0].JobId);
            Assert.AreEqual(expectedCreatedBy, list[0].CreatedBy);
            Assert.IsNull(list[0].ProcessingComplete);
            Assert.IsNull(list[0].ProcessingPickup);
            Assert.IsNull(list[0].ProcessorKey);
            Assert.AreEqual(0, list[0].RetryCount);
            Assert.AreEqual(JobStatus.Error, list[0].Status);
            Assert.AreEqual(JobType.None, list[0].Type);
            Assert.AreEqual(expectedErrorInformation, list[0].ErrorInformation);
            Assert.IsNull(list[0].ElapsedTime);
        }

        [TestMethod]
        public void NoOpJobBatteryCompleteTest()
        {
            // Arrange
            Guid expectedJobId = Guid.NewGuid();
            string expectedCreatedBy = string.Format("Test:{0}", expectedJobId);
            int expectedMilliseconds = 5000;

            JobDao dao = new JobDao();

            // Create job
            dao.InsertNoOpJob(expectedJobId, expectedCreatedBy);

            // Verify job comes back on select
            List<JobItem> list = dao.SelectJobList(new JobListFilter() { CreatedBy = expectedCreatedBy });
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(expectedJobId, list[0].JobId);
            Assert.AreEqual(expectedCreatedBy, list[0].CreatedBy);
            Assert.IsNull(list[0].ProcessingComplete);
            Assert.IsNull(list[0].ProcessingPickup);
            Assert.IsNull(list[0].ProcessorKey);
            Assert.AreEqual(0, list[0].RetryCount);
            Assert.AreEqual(JobStatus.Ready, list[0].Status);
            Assert.AreEqual(JobType.None, list[0].Type);
            Assert.IsNull(list[0].ErrorInformation);
            Assert.IsNull(list[0].ElapsedTime);

            // Complete Job
            dao.UpdateJobStatusComplete(expectedJobId, TimeSpan.FromMilliseconds(expectedMilliseconds));

            // Verify job comes back on select with updated status
            list = dao.SelectJobList(new JobListFilter() { CreatedBy = expectedCreatedBy });
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(expectedJobId, list[0].JobId);
            Assert.AreEqual(expectedCreatedBy, list[0].CreatedBy);
            Assert.IsNotNull(list[0].ProcessingComplete);
            Assert.IsNull(list[0].ProcessingPickup);
            Assert.IsNull(list[0].ProcessorKey);
            Assert.AreEqual(0, list[0].RetryCount);
            Assert.AreEqual(JobStatus.Completed, list[0].Status);
            Assert.AreEqual(JobType.None, list[0].Type);
            Assert.IsNull(list[0].ErrorInformation);
            Assert.AreEqual(expectedMilliseconds, list[0].ElapsedTime.Value.TotalMilliseconds);
        }

        [TestMethod]
        public void NoOpJobBatteryPickupTest()
        {
            // Arrange
            Guid expectedJobId = Guid.NewGuid();
            string expectedCreatedBy = string.Format("Test:{0}", expectedJobId);
            string expectedProcessorKey = string.Format("PK:{0}", expectedJobId);
            int expectedMilliseconds = 5000;

            JobDao dao = new JobDao();

            // Create job
            dao.InsertNoOpJob(expectedJobId, expectedCreatedBy);

            // Verify job comes back on select
            List<JobItem> list = dao.SelectJobList(new JobListFilter() { CreatedBy = expectedCreatedBy });
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(expectedJobId, list[0].JobId);
            Assert.AreEqual(expectedCreatedBy, list[0].CreatedBy);
            Assert.IsNull(list[0].ProcessingComplete);
            Assert.IsNull(list[0].ProcessingPickup);
            Assert.IsNull(list[0].ProcessorKey);
            Assert.AreEqual(0, list[0].RetryCount);
            Assert.AreEqual(JobStatus.Ready, list[0].Status);
            Assert.AreEqual(JobType.None, list[0].Type);
            Assert.IsNull(list[0].ErrorInformation);
            Assert.IsNull(list[0].ElapsedTime);

            // Pick up job for processing
            List<JobProcessItem> processList = dao.PickupJobs(expectedProcessorKey, 1, JobType.None);
            Assert.IsNotNull(processList);
            Assert.AreEqual(1, processList.Count);
            Assert.AreEqual(expectedJobId, processList[0].JobId);

            // Complete Job
            dao.UpdateJobStatusComplete(expectedJobId, TimeSpan.FromMilliseconds(expectedMilliseconds));

            // Verify job comes back on select with updated status
            list = dao.SelectJobList(new JobListFilter() { CreatedBy = expectedCreatedBy });
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(expectedJobId, list[0].JobId);
            Assert.AreEqual(expectedCreatedBy, list[0].CreatedBy);
            Assert.IsNotNull(list[0].ProcessingComplete);
            Assert.IsNotNull(list[0].ProcessingPickup);
            Assert.AreEqual(expectedProcessorKey, list[0].ProcessorKey);
            Assert.AreEqual(0, list[0].RetryCount);
            Assert.AreEqual(JobStatus.Completed, list[0].Status);
            Assert.AreEqual(JobType.None, list[0].Type);
            Assert.IsNull(list[0].ErrorInformation);
            Assert.AreEqual(expectedMilliseconds, list[0].ElapsedTime.Value.TotalMilliseconds);
        }

        [TestMethod]
        public void NoOpJobJobListSelectTest()
        {
            // Arrange several jobs (20)
            Guid[] expectedJobIds = Enumerable.Range(0, 20).Select(a => Guid.NewGuid()).ToArray();
            // Most jobs will use the same created by, except the first three will use unqiue ones
            string staticCreatedBy = string.Format("TestS:{0}", Guid.NewGuid());
            string[] createdByValues = expectedJobIds.Select((a, index) => (index < 3) ? string.Format("Test:{0}", a) : staticCreatedBy).ToArray();

            // Insert test jobs, adding 50ms pause between each so we can test date range filters too
            JobDao dao = new JobDao();
            for (int i = 0; i < expectedJobIds.Length; i++)
            {
                dao.InsertNoOpJob(expectedJobIds[i], createdByValues[i]);
                System.Threading.Thread.Sleep(50);
            }
            // Update status on first, fifth, sixth to Cancelled
            Assert.IsTrue(dao.UpdateJobStatusCancel(expectedJobIds[0]));
            Assert.IsTrue(dao.UpdateJobStatusCancel(expectedJobIds[4]));
            Assert.IsTrue(dao.UpdateJobStatusCancel(expectedJobIds[5]));

            // Verify created by filter works properly
            // Should only get 1 result for first three, then all remaining using the static created by value      
            // Record created dates
            List<JobItem> list = null;
            DateTime[] createdDateTimes = new DateTime[20]; 
            for (int i = 0; i < 3; i++)
            {
                list = dao.SelectJobList(new JobListFilter() { CreatedBy = createdByValues[i] });
                Assert.AreEqual(1, list.Count);
                createdDateTimes[i] = list[0].Created;
            }
            list = dao.SelectJobList(new JobListFilter() { PageSize = 99, CreatedBy = staticCreatedBy });
            Assert.AreEqual(expectedJobIds.Length - 3, list.Count);
            for (int i = 0; i < list.Count; i++)
                createdDateTimes[i + 3] = list[i].Created;

            // Verify job type (using created by as additional filter, should re-test on task specific job daos
            list = dao.SelectJobList(new JobListFilter() { CreatedBy = createdByValues[0], Type = JobType.None });
            Assert.AreEqual(1, list.Count);
            list = dao.SelectJobList(new JobListFilter() { CreatedBy = createdByValues[0], Type = JobType.WebScrape});
            Assert.AreEqual(0, list.Count);

            // Verify date range
            list = dao.SelectJobList(new JobListFilter() { PageSize = 99, CreatedStart = createdDateTimes[0], CreatedEnd = createdDateTimes[expectedJobIds.Length - 1] });
            Assert.AreEqual(expectedJobIds.Length, list.Count);

            // Verify job status 
            list = dao.SelectJobList(new JobListFilter() { Status = JobStatus.Cancelled, CreatedBy = staticCreatedBy });
            Assert.AreEqual(2, list.Count);
            list = dao.SelectJobList(new JobListFilter() { PageSize = 99, Status = JobStatus.Ready, CreatedBy = staticCreatedBy });
            Assert.AreEqual(expectedJobIds.Length - 3 - 2, list.Count);
            // Verify job status plus created by (first is cancelled, second is not)
            list = dao.SelectJobList(new JobListFilter() { Status = JobStatus.Cancelled, CreatedBy = createdByValues[0] });
            Assert.AreEqual(1, list.Count);
            list = dao.SelectJobList(new JobListFilter() { Status = JobStatus.Cancelled, CreatedBy = createdByValues[1] });
            Assert.AreEqual(0, list.Count);

            // Verify paging
            list = dao.SelectJobList(new JobListFilter() { PageSize = 2, PageIndex = 0, CreatedBy = staticCreatedBy });
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(expectedJobIds[3], list[0].JobId);
            Assert.AreEqual(expectedJobIds[4], list[1].JobId);
            list = dao.SelectJobList(new JobListFilter() { PageSize = 2, PageIndex = 1, CreatedBy = staticCreatedBy });
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(expectedJobIds[5], list[0].JobId);
            Assert.AreEqual(expectedJobIds[6], list[1].JobId);
            list = dao.SelectJobList(new JobListFilter() { PageSize = 2, PageIndex = 4, CreatedBy = staticCreatedBy });
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(expectedJobIds[11], list[0].JobId);
            Assert.AreEqual(expectedJobIds[12], list[1].JobId);

            // Cancel all jobs
            for (int i = 0; i < expectedJobIds.Length; i++)
                dao.UpdateJobStatusCancel(expectedJobIds[0]);
        }

        [TestMethod]
        public void NoOpJobPickupConcurrentTest()
        {
            // Arrange several jobs (20)
            Guid[] expectedJobIds = Enumerable.Range(0, 20).Select(a => Guid.NewGuid()).ToArray();
            // Use same created by
            Guid guid = Guid.NewGuid();
            string staticCreatedBy = string.Format("TestS:{0}", guid);
            string processorKey = string.Format("PK:{0}", guid.ToString().Substring(20));
            
            // Insert test jobs
            JobDao dao = new JobDao();
            for (int i = 0; i < expectedJobIds.Length; i++)
                dao.InsertNoOpJob(expectedJobIds[i], staticCreatedBy);

            // Verify only 20 ready jobs in system
            List<JobItem> list = dao.SelectJobList(new JobListFilter() { Status = JobStatus.Ready, Type = JobType.None, PageSize = 99 });
            Assert.AreEqual(20, list.Count);

            // Spin off 10 threads, each limiting to 2 pickups each - create new instance of dao on each thread to simulate load from scheduler
            List<JobProcessItem>[] processLists = new List<JobProcessItem>[10];
            Parallel.For(0, 10,
                   index => {
                       processLists[index] = (new JobDao()).PickupJobs(processorKey + " > " + Thread.CurrentThread.ManagedThreadId.ToString(), 2, JobType.None);
                   });

            // Verify we have 10 collections of 2 job ids each, and that no job ids were picked up more than once
            HashSet<Guid> jobIds = new HashSet<Guid>();
            for(int i = 0; i < 10; i++)
            {
                Assert.IsNotNull(processLists[i]);
                Assert.AreEqual(2, processLists[i].Count);
                jobIds.Add(processLists[i][0].JobId);
                jobIds.Add(processLists[i][1].JobId);
            }

            // Cancel all jobs
            for (int i = 0; i < expectedJobIds.Length; i++)
                dao.UpdateJobStatusCancel(expectedJobIds[0]);
        }
    }
}
