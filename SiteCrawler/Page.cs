using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SiteCrawler.TestTemplates;

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
        public string Comment { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> InvalidIcons { get; set; }
        public List<TestResult> TestResults { get; set; }

        public Page(Uri uri, TestRun testRun, Uri source = null)
        {
            Crawled = false;
            Uri = uri;
            Source = source;
            Links = new List<Uri>();
            TestRun = testRun;
            TestResult = Result.Inconclusive;
            Comment = String.Empty;
            InvalidIcons = new List<string>();
            TestResults = new List<TestResult>();
        }

        public Page(Uri uri)
        {
            Crawled = false;
            Uri = uri;
            Links = new List<Uri>();
            TestRun = TestRun;
            TestResult = Result.Inconclusive;
            Comment = String.Empty;
            InvalidIcons = new List<string>();
            TestResults = new List<TestResult>();
        }

        public void Crawl()
        {
            try
            {
                GetContent();
                FindLinks();
                TestResult = VerifyPage();          
            }
            catch (Exception)
            {
                TestResult = Result.Failed;
            }

            Crawled = true;
            
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
            var cookieValue = TestRun.EpiLoginCookieValue;
            var handler = new HttpClientHandler { UseCookies = true };
            handler.CookieContainer = new CookieContainer();
            handler.CookieContainer.Add(Uri, new Cookie(".EPiServerLogin", $"{cookieValue}", "/", $"{TestRun.BaseUri.Host}"));
            

            using (HttpClient client = new HttpClient(handler))
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

            // Check max url segments 
            if (uri.Segments.Length >= TestRun.MaximumUrlSegments)
            {
                return false;
            }

            // Exclude ignored domains
            
            // Exclude ignored url segments
            foreach (var ignoredUrlSegment in TestRun.IgnoredUrlSegments)
            {
                if (uri.ToString().ToLower().Contains($"/{ignoredUrlSegment}/"))
                {
                    return false;
                }
            }

            // .png .jpg

            return true;
        }

        public Result VerifyPage()
        {
            Result result = Result.NotRun;

            // Verify ResponseCode = OK
            TestResults.Add(VerifyResponseCode.Run(ResponseCode));

            // check http/https
            TestResults.Add(VerifyUriScheme.Run(TestRun.BaseUri, Uri));

            // Find double H1 tags
            TestResults.Add(VerifySingleH1Tag.Run(Content));

            // Find invalid links
            TestResults.Add(VerifyNoInvalidLinks.Run(Links));

            // find invalid colors
            TestResults.Add(VerifyNoInvalidColors.Run(Content, TestRun.ValidColors));

            // find invalid icons
            TestResults.Add(VerifyNoInvalidIcons.Run(Content, TestRun.ValidIcons));

            // find Page ID
            MatchCollection idMatches = Regex.Matches(Content, "epi.cms.contentdata:\\/{3}(\\d+)\"");
            try
            {
                ID = idMatches[0].Groups[1].Value;
            }
            catch (Exception)
            {
            }

            // find page Title
            MatchCollection titleMatches = Regex.Matches(Content, "<title>(.+)<\\/title>");

            try
            {
                Title = titleMatches[0].Groups[1].Value;
            }
            catch (Exception)
            {
            }

            // find page Description
            MatchCollection descriptionMatches = Regex.Matches(Content, "<meta name=\"description\" content=\"(.+)\" \\/>");
            try
            {
                Description = descriptionMatches[0].Groups[1].Value;
            }
            catch (Exception)
            {
            }

            // Merge test results
            foreach (var testResult in TestResults)
            {
                if (testResult.Result == Result.Failed)
                {
                    result = Result.Failed;
                    break;
                }               
                result = Result.Passed;
                
            }

            return result;
        }
    }
}
