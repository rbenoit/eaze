using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Green.Job.Common
{
    /// <summary>
    /// Represents a set of filter conditions when querying a job list.
    /// </summary>
    public class JobListFilter
    {
        public const int DefaultPageSize = 10;

        public JobListFilter()
        {
            PageSize = DefaultPageSize;
        }

        public JobType? Type { get; set; }

        public JobStatus? Status { get; set; }
        
        public string CreatedBy { get; set; }

        public DateTime? CreatedStart { get; set; }

        public DateTime? CreatedEnd { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}
