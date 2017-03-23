using System;

namespace Interview.Green.Web.Scraper.Interfaces
{
    /// <summary>
    ///     Common interface used by other objects that share these properties.
    /// </summary>
    public interface IBasic
    {
        int Id { get; set; }
        DateTime RequestedAt { get; set; }
    }
}