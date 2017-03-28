using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using Interview.Green.Job.Business.Dal;
using Interview.Green.Job.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interview.Green.Web.Scrapper.Tests.Integration.DAO
{
    [TestClass]
    public class ScrapeJobDaoTests
    {
        [TestInitialize()]
        public void Init()
        {
            // Clear any outstanding "Ready" jobs in test database
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Job] SET [JobStatus] = 4, [ErrorInformation] = 'Cancelled by test init' WHERE [JobStatus] = 0 AND [JobType] = 1", connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void ScrapeJobBatteryCompleteTest()
        {
            Guid expectedJobId = Guid.NewGuid();
            string expectedCreatedBy = string.Format("STest:{0}", expectedJobId);
            string expectedUrl = "http://http://stackoverflow.com";
            string expectedResponseRaw = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam sagittis nibh vitae neque iaculis, sit amet pretium lectus dapibus. Nullam nec urna egestas mauris rutrum fermentum. Nam at ante erat. Duis eu vehicula sem. In tristique dui id justo dapibus, eget pharetra turpis placerat. Cras aliquam convallis nulla, vel sodales sem hendrerit non. Integer pellentesque metus cursus, rhoncus turpis sed, pulvinar tortor.";
            int expectedMilliseconds = 2345;
            HttpStatusCode expectedStatusCode = HttpStatusCode.Accepted;

            // Create scrape job
            ScrapeJobDao dao = new ScrapeJobDao();
            dao.InsertScrapeJob(expectedJobId, expectedCreatedBy, expectedUrl);

            // Verify status of scrape job
            ScrapeJob actual = dao.SelectScrapeJob(expectedJobId);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedJobId, actual.JobId);
            Assert.AreEqual(expectedCreatedBy, actual.CreatedBy);
            Assert.IsNull(actual.ProcessingComplete);
            Assert.IsNull(actual.ProcessingPickup);
            Assert.IsNull(actual.ProcessorKey);
            Assert.AreEqual(0, actual.RetryCount);
            Assert.AreEqual(JobStatus.Ready, actual.Status);
            Assert.AreEqual(JobType.WebScrape, actual.Type);
            Assert.IsNull(actual.ErrorInformation);
            Assert.IsNull(actual.ElapsedTime);
            Assert.AreEqual(expectedUrl, actual.Url);
            Assert.IsNull(actual.HttpStatus);
            Assert.IsNull(actual.ResponseRaw);

            // Complete scrape job
            dao.UpdateScrapeJobStatusComplete(expectedJobId, TimeSpan.FromMilliseconds(expectedMilliseconds), expectedStatusCode, expectedResponseRaw);

            // Verify status of scrape job
            actual = dao.SelectScrapeJob(expectedJobId);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedJobId, actual.JobId);
            Assert.AreEqual(expectedCreatedBy, actual.CreatedBy);
            Assert.IsNotNull(actual.ProcessingComplete);
            Assert.IsNull(actual.ProcessingPickup);
            Assert.IsNull(actual.ProcessorKey);
            Assert.AreEqual(0, actual.RetryCount);
            Assert.AreEqual(JobStatus.Completed, actual.Status);
            Assert.AreEqual(JobType.WebScrape, actual.Type);
            Assert.IsNull(actual.ErrorInformation);
            Assert.AreEqual(expectedMilliseconds, actual.ElapsedTime.Value.TotalMilliseconds);
            Assert.AreEqual(expectedUrl, actual.Url);
            Assert.AreEqual(expectedStatusCode, actual.HttpStatus);
            Assert.AreEqual(expectedResponseRaw, actual.ResponseRaw);
        }
    }
}
