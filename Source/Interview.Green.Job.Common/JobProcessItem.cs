using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Green.Job.Common
{
    public class JobProcessItem
    {
        public Guid JobId { get; set; }

        public JobType Type { get; set; }

        public int RetryCount { get; set; }
    }
}
