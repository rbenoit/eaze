using System;
using Interview.Green.Web.Scraper.Interfaces;

namespace Interview.Green.Web.Scraper.Models
{
    public class JobRequest : IJobQueuedRequest
    {
        public int Id { get; set; }
        public DateTime RequestedAt { get; set; }
        public Guid RequestId { get; set; }
    }
}