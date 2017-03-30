using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Interview.Green.Web.Scraper.Controllers;
using Interview.Green.Web.Scrapper.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interview.Green.Web.Scrapper.Tests.Unit.API.Controllers
{
    [TestClass]
    public class JobControllerTests
    {
        [TestMethod, TestCategory("Unit")]
        public void GetJobsTest()
        {
            JobController controller = new JobController(() => new MockJobDao(), () => new MockScrapeJobDao());
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());

            // Error Response tests
            // Passing in a null for filter will return a server app error wrapping an argumentnullexception
            HttpResponseMessage actual = controller.Get(null);
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.InternalServerError, actual.StatusCode);
            Assert.IsInstanceOfType(actual.Content, typeof(ObjectContent<HttpError>));
            Assert.AreEqual("Value cannot be null.\r\nParameter name: filter", ((HttpError)((ObjectContent<HttpError>)actual.Content).Value).Message);

            // Passing in a negative page index for filter will return a server app error wrapping a green validation exception
            actual = controller.Get(new Job.Common.JobListFilter() { PageIndex = -1 });
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.InternalServerError, actual.StatusCode);
            Assert.IsInstanceOfType(actual.Content, typeof(ObjectContent<HttpError>));
            Assert.AreEqual("Page index cannot be negative.", ((HttpError)((ObjectContent<HttpError>)actual.Content).Value).Message);

            // Successful Response tests
            // Passing in a valid filter will return an empty result set with the mock dao 
            actual = controller.Get(new Job.Common.JobListFilter());
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.OK, actual.StatusCode);
            Assert.IsInstanceOfType(actual.Content, typeof(ObjectContent<IEnumerable<jobResult>>));
        }

        [TestMethod, TestCategory("Unit")]
        public void GetJobTest()
        {
            JobController controller = new JobController(() => new MockJobDao(), () => new MockScrapeJobDao());
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());

            // Successful Response tests
            //TODO: Update mocks to allow static data to test additional conditions

            // Error Response tests
            // Empty guid will throw an argument exception
            HttpResponseMessage actual = controller.Get(Guid.Empty);
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.InternalServerError, actual.StatusCode);
            Assert.IsInstanceOfType(actual.Content, typeof(ObjectContent<HttpError>));
            Assert.AreEqual("Job id cannot be empty.", ((HttpError)((ObjectContent<HttpError>)actual.Content).Value).Message);

            // Default mock implementation returns null for any id, so we can use any id to test this error condition
            // Note: Would need to update mock dao's to hold / return static data so we can use for all test conditions
            actual = controller.Get(Guid.NewGuid());
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.NotFound, actual.StatusCode);
        }

        [TestMethod, TestCategory("Unit")]
        public void DeleteJobTest()
        {
            JobController controller = new JobController(() => new MockJobDao(), () => new MockScrapeJobDao());
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());

            // Successful Repsonse tests
            // Default mock implementation will return true for any cancel
            HttpResponseMessage actual = controller.Delete(Guid.NewGuid());
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.OK, actual.StatusCode);

            // Error Response tests
            // Empty guid will throw an argument exception
            actual = controller.Delete(Guid.Empty);
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.InternalServerError, actual.StatusCode);
            Assert.IsInstanceOfType(actual.Content, typeof(ObjectContent<HttpError>));
            Assert.AreEqual("Job id cannot be empty.", ((HttpError)((ObjectContent<HttpError>)actual.Content).Value).Message);

        }

        [TestMethod, TestCategory("Unit")]
        public void PostJobTest()
        {
            JobController controller = new JobController(() => new MockJobDao(), () => new MockScrapeJobDao());
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            string expectedRequestedBy = "PostJobTest";
            string expectedUrl = "http://stackoverflow.com";

            // Successful Repsonse tests
            //TODO: Update mocks to allow static data to test additional conditions

            // Error Response tests
            // Empty url value will throw 
            HttpResponseMessage actual = controller.Post(new jobRequest() { requestedBy = expectedRequestedBy });
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.InternalServerError, actual.StatusCode);
            Assert.IsInstanceOfType(actual.Content, typeof(ObjectContent<HttpError>));
            Assert.AreEqual("Value cannot be null.\r\nParameter name: url", ((HttpError)((ObjectContent<HttpError>)actual.Content).Value).Message);

            // Default mock implementation will return null for a select on a job, so we can use that to test a failure to create the job
            actual = controller.Post(new jobRequest() { requestedBy = expectedRequestedBy, url = expectedUrl });
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.InternalServerError, actual.StatusCode);
            Assert.IsInstanceOfType(actual.Content, typeof(ObjectContent<HttpError>));
            Assert.IsTrue(((HttpError)((ObjectContent<HttpError>)actual.Content).Value).Message.StartsWith("Could not find created job:"));
        }
    }
}