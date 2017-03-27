using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Interview.Green.Job.Common;

namespace Interview.Green.Job.Business.Dal
{
    /// <summary>
    /// A sql specific implementation of <see cref="IScrapeJobDao"/>.
    /// </summary>
    public class ScrapeJobDao : BaseDao, IScrapeJobDao
    {
        /// <summary>
        /// Creates a new web scrape job with the given id and url, and records the created by information.
        /// </summary>
        /// <param name="jobId">The id of the job.</param>
        /// <param name="createdBy">A string indicating who is creating this job, which can be used in later searches.</param>
        /// <param name="url">The url to scrape.</param>
        /// <returns><c>True</c> if the insert was successful, otherwise <c>False</c>.</returns>
        public void InsertScrapeJob(Guid jobId, string createdBy, string url)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ScrapeJobInsert]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JobId", jobId);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                cmd.Parameters.AddWithValue("@Url", url);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// Returns a web scrape job's information.
        /// </summary>
        /// <param name="jobId">The id of the web scrape job to return.</param>
        /// <returns>A <see cref="ScrapeJob"/> representing the web scrape job.</returns>
        public ScrapeJob SelectScrapeJob(Guid jobId)
        {
            ScrapeJob job = null;
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ScrapeJobSelect]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JobId", jobId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        job = new ScrapeJob();
                        job.Created = (DateTime)reader["JobCreated"];
                        job.CreatedBy = reader["CreatedBy"].ToString();
                        job.ElapsedTime = (reader["ExecutionElapsed"] == DBNull.Value) ? null : (TimeSpan?)TimeSpan.FromMilliseconds((long)reader["ExecutionElapsed"]);
                        job.JobId = (Guid)reader["JobId"];
                        job.ProcessingComplete = (reader["ProcessingComplete"] == DBNull.Value) ? null : (DateTime?)reader["ProcessingComplete"];
                        job.ProcessingPickup = (reader["ProcessingPickup"] == DBNull.Value) ? null : (DateTime?)reader["ProcessingPickup"];
                        job.ProcessorKey = (reader["ProcessorKey"] == DBNull.Value) ? null : reader["ProcessorKey"].ToString();
                        job.RetryCount = (int)reader["RetryCount"];
                        job.Status = (JobStatus)(int)reader["JobStatus"];
                        job.Type = (JobType)(int)reader["JobType"];
                        job.ErrorInformation = (reader["ErrorInformation"] == DBNull.Value) ? null : reader["ErrorInformation"].ToString();
                        job.Url = reader["Url"].ToString();
                        job.HttpStatus = (reader["HttpStatusCode"] == DBNull.Value) ? null : (HttpStatusCode?)(int)reader["HttpStatusCode"];
                        job.ResponseRaw = (reader["ResponseRaw"] == DBNull.Value) ? null : reader["ResponseRaw"].ToString();
                    }
                }
                connection.Close();
            }

            return job;
        }

        /// <summary>
        /// Updated a given scrape job's status to 'Completed' with the elapsed execution time and associated scrape data.
        /// </summary>
        /// <param name="jobId">The id of the job to update.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <param name="httpStatus">The http status return code.</param>
        /// <param name="responseRaw">The raw response.</param>
        public void UpdateScrapeJobStatusComplete(Guid jobId, TimeSpan elapsedTime, HttpStatusCode httpStatus, string responseRaw)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ScrapeJobUpdateComplete]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JobId", jobId);
                cmd.Parameters.AddWithValue("@ExecutionElapsed", elapsedTime.TotalMilliseconds);
                cmd.Parameters.AddWithValue("@HttpStatusCode", (int)httpStatus);
                cmd.Parameters.AddWithValue("@ResponseRaw", responseRaw);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
