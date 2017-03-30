using System;
using System.Net;
using Interview.Green.Job.Common;
using Interview.Green.Web.Scrapper.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interview.Green.Web.Scrapper.Tests.Unit.API.Models
{
    [TestClass]
    public class scrapeJobResultTests
    {
        [TestMethod, TestCategory("Unit")]
        public void ConstructorMapTest()
        {
            // Testing only for mapped properties defined in this type; for other properties see base jobResult model tests
            // Arrange for least required values
            string expectedUrl = null;
            int? expectedHttpStatusCode = null;
            string expectedResponseRaw = null;

            ScrapeJob source = new ScrapeJob()
            {
                Url = expectedUrl,
                HttpStatus = (HttpStatusCode?)expectedHttpStatusCode,
                ResponseRaw = expectedResponseRaw
            };

            // Create api model and validate values mapped corrected
            scrapeJobResult actual = new scrapeJobResult(source);

            Assert.AreEqual(expectedUrl, actual.url);
            Assert.AreEqual(expectedHttpStatusCode, actual.httpStatusCode);
            Assert.AreEqual(expectedResponseRaw, actual.responseRaw);

            // Arrange for all values (no default values)
            expectedUrl = "http://stackoverflow.com";
            expectedHttpStatusCode = (int)HttpStatusCode.Accepted;
            expectedResponseRaw = "This is a raw response.";

            source = new ScrapeJob()
            {
                Url = expectedUrl,
                HttpStatus = (HttpStatusCode)expectedHttpStatusCode,
                ResponseRaw = expectedResponseRaw
            };

            // Create api model and validate values mapped corrected
            actual = new scrapeJobResult(source);

            Assert.AreEqual(expectedUrl, actual.url);
            Assert.AreEqual(expectedHttpStatusCode, actual.httpStatusCode);
            Assert.AreEqual(expectedResponseRaw, actual.responseRaw);
        }
    }
}
