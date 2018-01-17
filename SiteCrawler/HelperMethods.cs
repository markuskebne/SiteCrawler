using Newtonsoft.Json;
using System;

namespace SiteCrawler
{
    public static class HelperMethods
    {
        public static Uri VerifyURL(string link, Uri baseUri = null)
        {
            Uri uri;
            Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out uri);

            if (uri == null)
                return null;
            
            if (uri.IsAbsoluteUri)
            {
                return uri;
            }
            else
            {
                if (link.StartsWith("/"))
                    Uri.TryCreate(new Uri(baseUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped)), link, out uri);
                else
                {
                    var debug = Uri.TryCreate(baseUri, link, out uri);
                }
            }
            return uri;
        }

        public static TestRun ValidateArgumentAndReturnTestRun(string[] args)
        {
            string input;

            // Validate file
            if (!System.IO.File.Exists(args[0]))
            {
                Console.WriteLine($"Invalid argument. The given argument {args[0]} is not a valid file-path");
                return null;
            }
            input = System.IO.File.ReadAllText(args[0]);

            StartupData startupData = JsonConvert.DeserializeObject<StartupData>(input);
            
            // Validate BaseUri
            if (!startupData.BaseUri.IsAbsoluteUri)
            {
                Uri result;
                Uri.TryCreate(startupData.BaseUri.OriginalString, UriKind.Absolute, out result);
                if (result == null)
                {
                    Console.WriteLine($"Invalid argument. The given BaseUri {startupData.BaseUri.OriginalString} could not be parsed into a valid Absolute Uri. Use format http://www.google.com");
                    return null;
                }
                startupData.BaseUri = result;        
            }

            // Validate MaximumUrlSegments
            if (startupData.MaximumUrlSegments < 2) 
            {
                Console.WriteLine("Invalid argument. MaximumUrlSegments must be more than 1");
                return null;
            }

            // Validate ResultsFolder

            TestRun testRun = new TestRun
            {
                BaseUri = startupData.BaseUri,
                MaximumUrlSegments = startupData.MaximumUrlSegments,
                ResultsFolder = startupData.ResultsFolder.Replace(@"\", @"\\")               
            };
            testRun.Pages.Add(new Page(startupData.BaseUri, testRun));


            return testRun;
        }
    }
}
