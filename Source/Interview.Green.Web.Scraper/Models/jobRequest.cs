using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Interview.Green.Web.Scrapper.Models
{
    /// <summary>
    /// This model only supports Web Scrape job types.  None (No-op) job creation is not supported by the API.
    /// </summary>
    [DataContract()]
    public class jobRequest
    {
        [DataMember(IsRequired = true)]
        public string requestedBy { get; set; }

        [DataMember(IsRequired = true)]
        public string url { get; set; }
    }
}