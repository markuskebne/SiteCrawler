using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawler
{
    public class Page
    {
        public bool Crawled { get; set; }
        public Uri Uri { get; set; }
        public Uri Source { get; set; }
        public string Content { get; set; }
        public List<Uri> Links { get; set; }
        public TestRun TestRun { get; set; }
        public System.Net.HttpStatusCode ResponseCode { get; set; }
        public Result TestResult { get; set; }

        public Page(Uri uri, TestRun testRun, Uri source = null)
        {
            Crawled = false;
            Uri = uri;
            Source = source;
            Links = new List<Uri>();
            TestRun = testRun;
            TestResult = Result.Inconclusive;
        }

        public Page(Uri uri)
        {
            Crawled = false;
            Uri = uri;
            Links = new List<Uri>();
            TestRun = TestRun;
            TestResult = Result.Inconclusive;

        }

        public void Crawl()
        {
            try
            {
                GetContent();
                TestResult = VerifyPage();
                FindLinks();
            }
            catch (Exception)
            {
                TestResult = Result.Failed;
            }

            Crawled = true;
            
            TestRun.CrawledPages.Add(new Page(Uri, TestRun));

            TestRun.PagesToCrawl -= 1;
            Console.WriteLine($@"Page Crawled: {Uri} - Result: {TestResult}");
        }

        public void GetContent()
        {
            try
            {
                Content = GetPageContentAsync().Result;
            }
            catch (Exception)
            {
                TestResult = Result.Failed;
            }      
        }

        async Task<string> GetPageContentAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(Uri).Result)
                {
                    ResponseCode = response.StatusCode;
                    using (HttpContent content = response.Content)
                    {
                        var byteArray = content.ReadAsByteArrayAsync().Result;
                        var result = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

                        //string result =  content.ReadAsStringAsync().Result;
                        return result;
                    }
                }
            }
        }

        public void FindLinks()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(new StringReader(Content));

            var links = (from node in doc.DocumentNode.DescendantsAndSelf()
                         where node.NodeType == HtmlNodeType.Element
                         let name = node.Name.ToLower()
                         where name == "a" || name == "img" || name == "script" || name == "style" || name == "link"
                         let hrefAttributes = node.Attributes.FirstOrDefault(a => a.Name == "href" || a.Name == "src")
                         where hrefAttributes != null
                         let link = hrefAttributes.Value
                         where !link.StartsWith("javascript")
                         where !link.StartsWith("mailto:")
                         where !link.StartsWith("tel:")
                         where link != "#"
                         select link);

            foreach (var link in links)
            {
                string input = link;
                //Make them Absolute
                if (link.Contains('#'))
                    input = link.Split('#')[0];

                if (link.Contains('?'))
                    input = link.Split('?')[0];

                Uri uri = HelperMethods.VerifyUrl(input, Uri);
                if (uri != null)
                {               
                    Links.Add(uri);
                        
                    if (ShouldBeAddedToCrawlerQue(uri))
                    {
                        TestRun.Pages.Add(new Page(uri, TestRun, Uri));
                        TestRun.PagesToCrawl += 1;
                    }                 
                }
            }
        }

        public bool ShouldBeAddedToCrawlerQue(Uri uri)
        {
            if (TestRun.Pages.Select(page => page.Uri).Contains(uri))
            {
                return false;
            }
            if (uri.Host != TestRun.BaseUri.Host)
            {
                return false;
            }

            // check http/https
            if (uri.Scheme != TestRun.BaseUri.Scheme)
            {
                return false;
            }

            // Check max url segments ToDo: Make this configurable 
            if (uri.Segments.Length >= TestRun.MaximumUrlSegments)
            {
                return false;
            }

            // Exclude ignored domains
            
            // Exclude ignored url segments

            return true;
        }

        public Result VerifyPage()
        {
            Result result;

            // Verify ResponseCode = OK
            switch (ResponseCode)
            {
                case System.Net.HttpStatusCode.OK:
                    result = Result.Passed;
                    break;
                default:
                    result = Result.Failed;
                    break;                  
            }
            return result;
        }
    }
}
