using System;
using System.Net;
using System.Reflection;
using Interview.Green.Job.Business.Facade;
using Interview.Green.Job.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interview.Green.Web.Scrapper.Tests.Unit.Business
{
    [TestClass]
    public class ScrapeJobFacadeTests
    {
        [TestMethod, TestCategory("Unit")]
        public void CreateScrapeJobTest()
        {
            string expectedUrl = "http://www.yahoo.com";
            string expectedCreatedBy = string.Format("TestSJ:{0}", Guid.NewGuid());

            //TODO: All exception messages should be resource driven, using literal values for this excercise

            // Set up facade with mock dao
            ScrapeJobFacade actual = new ScrapeJobFacade(() => new MockJobDao(), () => new MockScrapeJobDao());

            // Mock dao is no-op
            actual.CreateScrapeJob(expectedCreatedBy, expectedUrl);

            // Exception test
            // null created by
            AssertUtility.ThrowsException<ArgumentNullException>(() => actual.CreateScrapeJob(null, expectedUrl));
            // null url
            AssertUtility.ThrowsException<ArgumentNullException>(() => actual.CreateScrapeJob(expectedCreatedBy, null));
            // Empty created by not allowed
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.CreateScrapeJob(string.Empty, expectedUrl),
                "Creator string cannot be empty.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.CreateScrapeJob("  ", expectedUrl),
                "Creator string cannot be empty.");
            // Empty url not allowed
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.CreateScrapeJob(expectedCreatedBy, string.Empty),
                "Url cannot be empty.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.CreateScrapeJob(expectedCreatedBy, "  "),
                "Url cannot be empty.");
            // Url must be well formed
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.CreateScrapeJob(expectedCreatedBy, "abckjagsfd"),
                "Url is not well formed.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.CreateScrapeJob(expectedCreatedBy, "http://domain with a space.com"),
                "Url is not well formed.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.CreateScrapeJob(expectedCreatedBy, "http:/missingslash.com"),
                "Url is not well formed.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.CreateScrapeJob(expectedCreatedBy, "http:://toomanycolons.com"),
                "Url is not well formed.");
            // Only HTTP and HTTPS schemes allowed
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.CreateScrapeJob(expectedCreatedBy, "ftp://yahoo.com/folder"),
                "Only HTTP and HTTPS schemes supported.");
            AssertUtility.ThrowsException<GreenValidationException>(() => actual.CreateScrapeJob(expectedCreatedBy, "file://yahoo.com/folder"),
                "Only HTTP and HTTPS schemes supported.");
        }

        [TestMethod, TestCategory("Unit")]
        public void GetScrapeJobTest()
        {
            //TODO: All exception messages should be resource driven, using literal values for this excercise

            // Set up facade with mock dao
            ScrapeJobFacade actual = new ScrapeJobFacade(() => new MockJobDao(), () => new MockScrapeJobDao());

            // Mock dao is no-op
            actual.GetScrapeJob(Guid.NewGuid());

            // Exception test
            // Empty job id not allowed
            AssertUtility.ThrowsException<ArgumentOutOfRangeException>(() => actual.GetScrapeJob(Guid.Empty));
        }

        [TestMethod, TestCategory("Unit")]
        public void CompleteScrapeJobTest()
        {
            TimeSpan exepectedElapsedTime = TimeSpan.FromMilliseconds(500);

            //TODO: All exception messages should be resource driven, using literal values for this excercise

            // Set up facade with mock dao
            ScrapeJobFacade actual = new ScrapeJobFacade(() => new MockJobDao(), () => new MockScrapeJobDao());

            // Mock dao is no-op
            actual.CompleteScrapeJob(Guid.NewGuid(), exepectedElapsedTime, System.Net.HttpStatusCode.Accepted, "abc");

            // Exception test
            // Empty job id not allowed
            AssertUtility.ThrowsException<ArgumentOutOfRangeException>(() => actual.CompleteScrapeJob(Guid.Empty, exepectedElapsedTime, System.Net.HttpStatusCode.Accepted, "abc"));
            // Null response not allowed
            AssertUtility.ThrowsException<ArgumentNullException>(() => actual.CompleteScrapeJob(Guid.NewGuid(), exepectedElapsedTime, System.Net.HttpStatusCode.Accepted, null));

        }

        [TestMethod, TestCategory("Unit")]
        public void ProcessScrapeJobTest()
        {
            //TODO: All exception messages should be resource driven, using literal values for this excercise

            // Set up facade with mock dao
            ScrapeJobFacade actual = new ScrapeJobFacade(() => new MockJobDao(), () => new MockScrapeJobDao());

            // Mock dao is no-op
            
            // Exception test
            // Empty job id not allowed
            AssertUtility.ThrowsException<ArgumentOutOfRangeException>(() => actual.ProccessScrapeJob(Guid.Empty));
        }

        [TestMethod, TestCategory("Unit")]
        public void ProcessUriRequestTest()
        {
            //TODO: All exception messages should be resource driven, using literal values for this excercise

            // Set up facade with mock dao
            ScrapeJobFacade actual = new ScrapeJobFacade(() => new MockJobDao(), () => new MockScrapeJobDao());

            // Use valid uri for test, validate known string appears in response string
            HttpStatusCode actualStatus;
            string actualResponse = Call_ProcessUriRequest(actual, new Uri("http://stackoverflow.com"), out actualStatus);
            Assert.IsNotNull(actualResponse);
            Assert.IsFalse(string.IsNullOrWhiteSpace(actualResponse));
            Assert.AreEqual(HttpStatusCode.OK, actualStatus);
            Assert.IsTrue(actualResponse.Contains("<title>Stack Overflow</title>"));

            // Exception test
            // Invalid site, expect web exception (wrapped in target invocation exception by caller)
            AssertUtility.ThrowsException<TargetInvocationException>(() => Call_ProcessUriRequest(actual, new Uri("http://stackoverflowthisdomaindoesnotexist.com/"), out actualStatus));
        }

        /// <summary>
        /// Reflective wrapper around protected "ProcessUriRequest" method to allow testing.
        /// </summary>
        /// <param name="obj">Instance of <see cref="ScrapeJobFacade"/> to invoke the method call from.</param>
        /// <param name="uri">The uri parameter.</param>
        /// <param name="httpStatus">The out status code value.</param>
        /// <returns>Result of method call.</returns>
        protected string Call_ProcessUriRequest(ScrapeJobFacade obj, Uri uri, out HttpStatusCode httpStatus)
        {
            MethodInfo method = obj.GetType().GetMethod("ProcessUriRequest", BindingFlags.Instance | BindingFlags.NonPublic);
            object[] parameters = new object[] { uri, null };
            string result = (string)method.Invoke(obj, parameters);
            httpStatus = (HttpStatusCode)parameters[1];

            return result;
        }

    }
}
