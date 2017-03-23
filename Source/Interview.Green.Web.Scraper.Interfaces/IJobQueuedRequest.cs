using System;

namespace Interview.Green.Web.Scraper.Interfaces
{
    /// <summary>
    ///     Used for Queued Jobs.
    /// </summary>
    public interface IJobQueuedRequest : IBasic
    {
        Guid RequestId { get; set; }
    }
}