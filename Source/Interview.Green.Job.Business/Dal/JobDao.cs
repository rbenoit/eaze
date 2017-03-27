using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interview.Green.Job.Common;

namespace Interview.Green.Job.Business.Dal
{
    /// <summary>
    /// A sql specific implementation of <see cref="IJobDao"/>.
    /// </summary>
    public class JobDao : BaseDao, IJobDao
    {
        private const int RetryMax = 2;

        /// <summary>
        /// Creates a new instance of <see cref="JobDao"/> connected to the proper data store.
        /// </summary>
        public JobDao()
            : base()
        {
        }

        /// <summary>
        /// Selects a list of jobs from the data store based on the given filter.
        /// </summary>
        /// <param name="filter">The filter conditions including paging information.</param>
        /// <returns>List of <see cref="JobItem"/> items.</returns>
        public List<JobItem> SelectJobList(JobListFilter filter)
        {
            List<JobItem> list = new List<JobItem>();
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[JobListSelect]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PageSize", filter.PageSize);
                cmd.Parameters.AddWithValue("@PageIndex", filter.PageIndex);
                if (filter.Status != null)
                    cmd.Parameters.AddWithValue("@JobStatus", (int)filter.Status);
                if (filter.Type != null)
                    cmd.Parameters.AddWithValue("@JobType", (int)filter.Type);
                if (filter.CreatedBy != null)
                    cmd.Parameters.AddWithValue("@CreatedBy", filter.CreatedBy);
                if (filter.CreatedStart != null && filter.CreatedEnd != null)
                {
                    cmd.Parameters.AddWithValue("@JobCreatedStart", filter.CreatedStart);
                    cmd.Parameters.AddWithValue("@JobCreatedEnd", filter.CreatedEnd);
                }
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        JobItem item = new JobItem();
                        item.Created = (DateTime)reader["JobCreated"];
                        item.CreatedBy = reader["CreatedBy"].ToString();
                        item.ElapsedTime = (reader["ExecutionElapsed"] == DBNull.Value) ? null : (TimeSpan?)TimeSpan.FromMilliseconds((long)reader["ExecutionElapsed"]);
                        item.JobId = (Guid)reader["JobId"];
                        item.ProcessingComplete = (reader["ProcessingComplete"] == DBNull.Value) ? null : (DateTime?)reader["ProcessingComplete"];
                        item.ProcessingPickup = (reader["ProcessingPickup"] == DBNull.Value) ? null : (DateTime?)reader["ProcessingPickup"];
                        item.ProcessorKey = (reader["ProcessorKey"] == DBNull.Value) ? null : reader["ProcessorKey"].ToString();
                        item.RetryCount = (int)reader["RetryCount"];
                        item.Status = (JobStatus)(int)reader["JobStatus"];
                        item.Type = (JobType)(int)reader["JobType"];
                        item.ErrorInformation = (reader["ErrorInformation"] == DBNull.Value) ? null : reader["ErrorInformation"].ToString();
                        list.Add(item);                        
                    }
                }

                connection.Close();
            }

            return list;
        }

        /// <summary>
        /// Updates a given job's status to 'Cancelled'.
        /// </summary>
        /// <param name="jobId">The id of the job to update.</param>
        /// <returns><c>True</c> if the job is successfully canceled, otherwise <c>False</c>.</returns>
        public bool UpdateJobStatusCancel(Guid jobId)
        {
            bool result = false;
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[JobCancel]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JobId", jobId);
                cmd.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.InputOutput;        
                cmd.ExecuteNonQuery();
                if ((int)cmd.Parameters["@Result"].Value == 0)
                    result = true;
                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Updates a given job's status to 'Error' and updates the error information.
        /// </summary>
        /// <param name="jobId">The id of the job to update.</param>
        /// <param name="errorInformation">Error information.</param>
        public void UpdateJobStatusError(Guid jobId, string errorInformation)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[JobUpdateError]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JobId", jobId);
                cmd.Parameters.AddWithValue("@ErrorInformation", errorInformation);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// Updated a given job's status to 'Completed' with the elapsed execution time.
        /// </summary>
        /// <param name="jobId">The id of the job to update.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        public void UpdateJobStatusComplete(Guid jobId, TimeSpan elapsedTime)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[JobUpdateComplete]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JobId", jobId);
                cmd.Parameters.AddWithValue("@ExecutionElapsed", elapsedTime.TotalMilliseconds);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
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
            List<JobProcessItem> list = new List<JobProcessItem>();
            int retry = 0;
            bool continueAttempt = true;
            while (continueAttempt)
            {
                try
                {
                    using (SqlConnection connection = GetConnection())
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("[dbo].[JobProcessPickup]", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProcessorKey", processorKey);
                        cmd.Parameters.AddWithValue("@MaximumRows", maximumJobs);
                        if (jobType != null)
                            cmd.Parameters.AddWithValue("@JobType", (int)jobType);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new JobProcessItem()
                                {
                                    JobId = (Guid)reader["JobId"],
                                    Type = (JobType)(int)reader["JobType"],
                                    RetryCount = (int)reader["RetryCount"]
                                });
                            }
                        }
                        connection.Close();
                        continueAttempt = false;
                    }
                }
                catch(SqlException sx)
                {
                    if (sx.Message.Contains("deadlock"))
                    {
                        if (retry++ >= RetryMax)
                            throw;
                    }
                }                
            }

            return list;
        }

        /// <summary>
        /// Inserts a new non task specific job (a no-operation job).
        /// </summary>
        /// <param name="jobId">The id of the job.</param>
        /// <param name="createdBy">A string indicating who is creating this job, which can be used in later searches.</param>
        /// <returns><c>True</c> if the job was created successfully; otherwise <c>False</c>.</returns>
        public void InsertNoOpJob(Guid jobId, string createdBy)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[JobInsert]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JobId", jobId);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                cmd.Parameters.AddWithValue("@JobType", (int)JobType.None);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
