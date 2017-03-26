using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Green.Job.Common
{
    public class JobItem
    {
        public Guid JobId { get; set; }

        public JobType Type { get; set; }

        public DateTime Created { get; set; }

        public JobStatus Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ProcessingPickup { get; set; }

        public DateTime? ProcessingComplete { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public string ProcessorKey { get; set; }

        public int RetryCount { get; set; }

    }
}
