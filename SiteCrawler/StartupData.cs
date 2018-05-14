using System;
using System.Collections.Generic;

namespace SiteCrawler
{
    public class StartupData
    {
        public Uri BaseUri { get; set; }
        public int MaximumUrlSegments  { get; set; }
        public string ResultsFolder { get; set; }
        public string EpiLoginCookieValue { get; set; }
        public List<String> IgnoredUrlSegments = new List<string>();
        public List<String> ValidColors = new List<string>();
        public List<String> ValidIcons = new List<string>();
    }
}
