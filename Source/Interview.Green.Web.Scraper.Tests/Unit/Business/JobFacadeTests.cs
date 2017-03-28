using System;
using System.Collections.Generic;
using Interview.Green.Job.Business.Facade;
using Interview.Green.Job.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interview.Green.Web.Scrapper.Tests.Unit.Business
{
    [TestClass]
    public class JobFacadeTests
    {
        [TestMethod, TestCategory("Unit")]
        public void GetJobListTest()
        {
            //TODO: All exception messages should be resource driven, using literal values for this excercise

            // Set up facade with mock dao
            JobFacade actual = new JobFacade(() => new MockJobDao());

            // Mock dao is no-op
            actual.GetJobList(new JobListFilter() { PageSize = 20 });

            // Exception tests
            // Null filter
            AssertUtility.ThrowsException<ArgumentNullException>(() => actual.GetJobList(null));
            // Page Index out of range
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.GetJobList(new JobListFilter() { PageIndex = -1 }),
                "Page index cannot be negative.");
            // Page Size out of range
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.GetJobList(new JobListFilter() { PageSize = 0 }),
                "Page size cannot be less than 1.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.GetJobList(new JobListFilter() { PageSize = int.MaxValue }),
                "Page size cannot be greater than 200.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.GetJobList(new JobListFilter() { PageSize = -1 }),
                "Page size cannot be less than 1.");
            // Missing start or end of created date range
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.GetJobList(new JobListFilter() { CreatedStart = DateTime.UtcNow }),
                "Both start and end job created dates must be supplied.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.GetJobList(new JobListFilter() { CreatedEnd = DateTime.UtcNow }),
                "Both start and end job created dates must be supplied.");
            // Invalid date range (start after end)
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.GetJobList(new JobListFilter() { CreatedStart = DateTime.UtcNow, CreatedEnd = DateTime.UtcNow.AddDays(-1) }),
                "Job created start date cannot be later than end date.");
        }

        [TestMethod, TestCategory("Unit")]
        public void CancelJobTest()
        {
            //TODO: All exception messages should be resource driven, using literal values for this excercise

            // Set up facade with mock dao
            JobFacade actual = new JobFacade(() => new MockJobDao());

            // Mock dao is no-op
            actual.CancelJob(Guid.NewGuid());

            // Exception tests
            // Empty guid not allowed
            AssertUtility.ThrowsException<ArgumentException>(() => actual.CancelJob(Guid.Empty));
        }

        [TestMethod, TestCategory("Unit")]
        public void FailJobTest()
        {
            //TODO: All exception messages should be resource driven, using literal values for this excercise

            // Set up facade with mock dao
            JobFacade actual = new JobFacade(() => new MockJobDao());

            // Mock dao is no-op
            actual.FailJob(Guid.NewGuid(), "Error data.");

            // Exception tests
            // Empty guid not allowed
            AssertUtility.ThrowsException<ArgumentException>(() => actual.FailJob(Guid.Empty, "Error data."));
            // null error information not allowed
            AssertUtility.ThrowsException<ArgumentException>(() => actual.FailJob(Guid.NewGuid(), null));
        }

        [TestMethod, TestCategory("Unit")]
        public void CompleteJobTest()
        {
            //TODO: All exception messages should be resource driven, using literal values for this excercise

            // Set up facade with mock dao
            JobFacade actual = new JobFacade(() => new MockJobDao());

            // Mock dao is no-op
            actual.CompleteJob(Guid.NewGuid(), TimeSpan.FromMilliseconds(500));

            // Exception tests
            // Empty guid not allowed
            AssertUtility.ThrowsException<ArgumentException>(() => actual.CompleteJob(Guid.Empty, TimeSpan.FromMilliseconds(500)));
        }

        [TestMethod, TestCategory("Unit")]
        public void PickupJobsTest()
        {
            //TODO: All exception messages should be resource driven, using literal values for this excercise

            // Set up facade with mock dao
            JobFacade actual = new JobFacade(() => new MockJobDao());

            // Mock dao is no-op
            actual.PickupJobs("key", 5, null);
            actual.PickupJobs("key", 5, JobType.None);

            // Exception tests
            // processorKey cannot be null
            AssertUtility.ThrowsException<ArgumentNullException>(() => actual.PickupJobs(null, 5, null));
            // Key cannot be an empty or whitespace only string
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.PickupJobs(string.Empty, 5, null)
                , "Processor key cannot be blank.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.PickupJobs("   ", 5, null)
                , "Processor key cannot be blank.");
            // Maximum jobs cannot be less than 1
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.PickupJobs("key", 0, null)
                , "Maximum jobs to process cannot be less than 1.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.PickupJobs("key", -1, null)
                , "Maximum jobs to process cannot be less than 1.");
            // Maximum jobs cannot be greater than configured max (100)
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.PickupJobs("key", int.MaxValue, null)
                , "Maximum jobs to process cannot be greater than 100.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.PickupJobs("key", 101, null)
                , "Maximum jobs to process cannot be greater than 100.");
        }
    }
}