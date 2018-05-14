using System;
using System.Collections.Generic;

namespace SiteCrawler
{
    public class TestRun
    {
        public Uri BaseUri { get; set; }
        public List<Page> Pages { get; set; }
        public int PagesToCrawl { get; set; }
        public List<Page> CrawledPages { get; set; }
        public Result Result { get; set; }
        public int MaximumUrlSegments;
        public string ResultsFolder { get; set; }
        public string EpiLoginCookieValue { get; set; }

        public List<String> InvalidColors = new List<string>();
        public List<String> InvalidIcons = new List<string>();
        public List<String> IgnoredUrlSegments = new List<string>();
        public List<String> ValidColors = new List<string>();
        public List<String> ValidIcons = new List<string>();

        public TestRun(Uri startpage = null)
        {
            BaseUri = startpage;
            Pages = new List<Page>();
            PagesToCrawl = 1;
            Result = Result.NotRun;
        }
    }
}
