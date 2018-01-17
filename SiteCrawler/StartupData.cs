using System;

namespace SiteCrawler
{
    public class StartupData
    {
        public Uri BaseUri { get; set; }
        public int MaximumUrlSegments  { get; set; }
        public string ResultsFolder { get; set; }
    }
}
