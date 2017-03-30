using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Quartz.Impl;

namespace Interview.Green.Web.Scrapper.Service
{   
    public class JobProcessorService : ServiceBase
    {
        public const string Name = "Interview Job Processing Service";
        // Grab the Scheduler instance from the Factory 
        IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
        private ILog Logger { get; set; }

        public JobProcessorService()
        {
            ServiceName = Name;
            Logger = LogManager.GetLogger(this.GetType());
        }
        
        public void Start(string[] args)
        {
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            Logger.WarnFormat("{0} service started..", ServiceName);

            try
            {
                Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Common.Logging.LogLevel.Info };

                scheduler.Start();

                // Pull additional scheduling configuration from app settings
                int numberOfJobs;
                if (!int.TryParse(ConfigurationManager.AppSettings["NumberOfJobThreads"], out numberOfJobs))
                    numberOfJobs = 1;
                else if (numberOfJobs < 0 || numberOfJobs > 20)
                    throw new ConfigurationErrorsException("AppSetting 'NumberOfJobThreads' cannot have a value less than 1 or greater than 20.");
                int intervalInSeconds;
                if (!int.TryParse(ConfigurationManager.AppSettings["PollingIntervalSeconds"], out intervalInSeconds))
                    intervalInSeconds = 10;
                else if (intervalInSeconds < 2 || intervalInSeconds > 60000)
                    throw new ConfigurationErrorsException("AppSetting 'PollingIntervalSeconds' cannot have a value less than 2 or greater than 60000.");
                int maximumJobs;
                if (!int.TryParse(ConfigurationManager.AppSettings["MaximumJobsPerThread"], out maximumJobs))
                    maximumJobs = 10;
                else if (maximumJobs < 1 || maximumJobs > 20)
                    throw new ConfigurationErrorsException("AppSetting 'MaximumJobsPerThread' cannot have a value less than 1 or greater than 20.");


                // Define jobs/triggers based on number of jobs desired
                IJobDetail[] jobs = new IJobDetail[numberOfJobs];
                ITrigger[] triggers = new ITrigger[numberOfJobs];
                for (int i = 0; i < numberOfJobs; i++)
                {
                    // Trigger the job to run now, and then repeat every 'intervalInSeconds' seconds
                    triggers[i] = TriggerBuilder.Create()
                        .WithIdentity(string.Format("trigger-{0}", i), "group1")
                        .StartNow()
                        .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds(intervalInSeconds)
                            .RepeatForever())
                        .Build();

                    // Create job
                    jobs[i] = JobBuilder.Create<ProcessorJob>()
                        .WithIdentity(string.Format("job-{0}", i), "group1")
                        .UsingJobData(ProcessorJob.JobDataKey_MaximumJobsPerThread, maximumJobs)
                        .Build();

                }
                
                for (int i = 0; i < numberOfJobs; i++)
                {
                    // Tell quartz to schedule the job using our trigger
                    scheduler.ScheduleJob(jobs[i], triggers[i]);
                }
                
            }
            catch (SchedulerException se)
            {
                Logger.Error(se);
            }
        }

        protected override void OnStop()
        {
            Logger.WarnFormat("{0} service stopped.", ServiceName);
            scheduler.Shutdown();
        }
    }
}
