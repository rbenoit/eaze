using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Interview.Green.Job.Common;

namespace Interview.Green.Web.Scrapper.Models
{
    [DataContract()]
    public class jobResult
    {
        [DataMember()]
        public Guid id { get; set; }

        [DataMember()]
        public string requestedBy { get; set; }

        [DataMember()]
        public DateTime requested { get; set; }

        [DataMember()]
        public string jobType { get; set; }

        [DataMember()]
        public string jobStatus { get; set; }

        [DataMember()]
        public DateTime? processingPickup { get; set; }

        [DataMember()]
        public DateTime? processingComplete { get; set; }

        [DataMember()]
        public TimeSpan? elapsedTime { get; set; }

        [DataMember()]
        public string processorKey { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string errorInformation { get; set; }

        public jobResult(JobItem source)
        {
            id = source.JobId;
            requestedBy = source.CreatedBy;
            requested = source.Created;
            jobType = source.Type.ToString();
            jobStatus = source.Status.ToString();
            processingPickup = source.ProcessingPickup;
            processingComplete = source.ProcessingComplete;
            elapsedTime = source.ElapsedTime;
            processorKey = source.ProcessorKey;
            errorInformation = source.ErrorInformation;
        }
    }
}