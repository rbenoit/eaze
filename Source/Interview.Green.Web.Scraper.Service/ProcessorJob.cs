using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Interview.Green.Job.Business.Dal;
using Interview.Green.Job.Business.Facade;
using Interview.Green.Job.Common;
using Quartz;

namespace Interview.Green.Web.Scrapper.Service
{
    /// <summary>
    /// Performs the actual work of processing requested scrape jobs.
    /// </summary>    
    public class ProcessorJob : IJob
    {
        public const string JobDataKey_MaximumJobsPerThread = "MaximumJobsPerThread";
        private Func<IJobDao> JobDaoMethod { get; set; } 
        private Func<IScrapeJobDao> ScrapeJobDaoMethod { get; set; }
        private ILog Logger { get; set; }

        public ProcessorJob()
            : this(()=> new JobDao(), ()=> new ScrapeJobDao())
        {
        }

        public ProcessorJob(Func<IJobDao> jobDaoMethod, Func<IScrapeJobDao> scrapeJobDaoMethod)
        {
            JobDaoMethod = jobDaoMethod;
            ScrapeJobDaoMethod = scrapeJobDaoMethod;
            Logger = LogManager.GetLogger(this.GetType());
        }

        private JobFacade CreateJobFacade()
        {
            return new JobFacade(JobDaoMethod);
        }

        private ScrapeJobFacade CreateScrapeJobFacade()
        {
            return new ScrapeJobFacade(JobDaoMethod, ScrapeJobDaoMethod);
        }

        public void Execute(IJobExecutionContext context)
        {            
            Logger.InfoFormat("EXECUTING - {0} - {1}", context.JobDetail.Key, DateTime.Now.ToLongTimeString());

            JobFacade jobFacade = new JobFacade();
            ScrapeJobFacade scrapeJobFacade = new ScrapeJobFacade();

            // Pull job items to process based on configuration
            List<JobProcessItem> jobsToProcess = jobFacade.PickupJobs(context.JobDetail.Key.ToString(), (int)context.JobDetail.JobDataMap[JobDataKey_MaximumJobsPerThread], null);
            Logger.InfoFormat("\tFound {0} jobs to process", jobsToProcess.Count);
            foreach(JobProcessItem job in jobsToProcess)
            {
                try
                {
                    // This implementation should be separated into different implementors, that is being skipped due to time constraints
                    switch (job.Type)
                    {
                        case JobType.None:
                            jobFacade.CompleteJob(job.JobId, TimeSpan.MinValue);
                            break;
                        case JobType.WebScrape:
                            scrapeJobFacade.ProccessScrapeJob(job.JobId);
                            break;
                        default:
                            throw new GreenException(string.Format("Cannot process job type '{0}' for id '{1}'", job.Type, job.JobId));
                    }
                }
                catch(Exception ex)
                {
                    Logger.Error(ex);
                    jobFacade.FailJob(job.JobId, ex.ToString());
                }
            }
        }

        
    }
}
