using System;
using Interview.Green.Job.Common;
using Interview.Green.Web.Scrapper.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interview.Green.Web.Scrapper.Tests.Unit.API.Models
{
    [TestClass]
    public class jobResultTests
    {
        [TestMethod, TestCategory("Unit")]
        public void ConstructorMapTest()
        {
            // Arrange for least required values
            Guid expectedId = Guid.NewGuid();
            DateTime expectedRequested = DateTime.UtcNow;
            DateTime? expectedProcessingPickup = null;
            DateTime? expectedProcessingComplete = null;
            TimeSpan? expectedElapsedTime = null;
            JobType expectedType = JobType.WebScrape;
            JobStatus expectedStatus = JobStatus.Ready;
            string expectedRequestedBy = "TestRequestor1";
            string expectedProcessorKey = null;
            string expectedErrorInformation = null;

            JobItem source = new JobItem()
            {
                Created = expectedRequested,
                CreatedBy = expectedRequestedBy,
                ElapsedTime = expectedElapsedTime,
                ErrorInformation = expectedErrorInformation,
                JobId = expectedId,
                ProcessingComplete = expectedProcessingComplete,
                ProcessingPickup = expectedProcessingPickup,
                ProcessorKey = expectedProcessorKey,
                Status = expectedStatus,
                Type = expectedType
            };

            // Create api model and validate values mapped corrected
            jobResult actual = new jobResult(source);

            Assert.AreEqual(expectedElapsedTime, actual.elapsedTime);
            Assert.AreEqual(expectedErrorInformation, actual.errorInformation);
            Assert.AreEqual(expectedId, actual.id);
            Assert.AreEqual(expectedProcessingComplete, actual.processingComplete);
            Assert.AreEqual(expectedProcessingPickup, actual.processingPickup);
            Assert.AreEqual(expectedProcessorKey, actual.processorKey);
            Assert.AreEqual(expectedRequested, actual.requested);
            Assert.AreEqual(expectedRequestedBy, actual.requestedBy);
            Assert.AreEqual(expectedStatus.ToString(), actual.jobStatus);
            Assert.AreEqual(expectedType.ToString(), actual.jobType);

            // Arrange for all values (no default values)
            expectedProcessingPickup = DateTime.UtcNow.AddSeconds(-2);
            expectedProcessingComplete = DateTime.UtcNow;
            expectedProcessorKey = "ProcessorKey";
            expectedElapsedTime = TimeSpan.FromMilliseconds(340);
            expectedStatus = JobStatus.Error;
            expectedErrorInformation = "An error!";

            source = new JobItem()
            {
                Created = expectedRequested,
                CreatedBy = expectedRequestedBy,
                ElapsedTime = expectedElapsedTime,
                ErrorInformation = expectedErrorInformation,
                JobId = expectedId,
                ProcessingComplete = expectedProcessingComplete,
                ProcessingPickup = expectedProcessingPickup,
                ProcessorKey = expectedProcessorKey,
                Status = expectedStatus,
                Type = expectedType
            };

            // Create api model and validate values mapped corrected
            actual = new jobResult(source);

            Assert.AreEqual(expectedElapsedTime, actual.elapsedTime);
            Assert.AreEqual(expectedErrorInformation, actual.errorInformation);
            Assert.AreEqual(expectedId, actual.id);
            Assert.AreEqual(expectedProcessingComplete, actual.processingComplete);
            Assert.AreEqual(expectedProcessingPickup, actual.processingPickup);
            Assert.AreEqual(expectedProcessorKey, actual.processorKey);
            Assert.AreEqual(expectedRequested, actual.requested);
            Assert.AreEqual(expectedRequestedBy, actual.requestedBy);
            Assert.AreEqual(expectedStatus.ToString(), actual.jobStatus);
            Assert.AreEqual(expectedType.ToString(), actual.jobType);

        }
    }
}
