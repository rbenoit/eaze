using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Interview.Green.Job.Common;

namespace Interview.Green.Web.Scrapper.Models
{
    [DataContract()]
    public class scrapeJobResult : jobResult
    {
        public scrapeJobResult(ScrapeJob source)
            : base(source)
        {
            url = source.Url;
            if (source.HttpStatus.HasValue)
                httpStatusCode = (int)source.HttpStatus;
            responseRaw = source.ResponseRaw;
        }

        [DataMember()]
        public string url { get; set; }

        [DataMember()]
        public int? httpStatusCode { get; set; }

        [DataMember()]
        public string responseRaw { get; set; }
    }
}