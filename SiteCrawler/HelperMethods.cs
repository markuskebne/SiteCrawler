using Newtonsoft.Json;
using System;

namespace SiteCrawler
{
    public static class HelperMethods
    {
        public static Uri VerifyUrl(string link, Uri baseUri = null)
        {
            Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out var uri);

            if (uri != null && uri.IsAbsoluteUri)
                return uri;
            
            if (link.StartsWith("/"))
            {
                if (baseUri != null)
                    Uri.TryCreate(new Uri(baseUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped)),
                        link, out uri);
            }

            else
            {
                Uri.TryCreate(baseUri, link, out uri);
            }
            return uri;
        }

        public static TestRun ValidateArgumentAndReturnTestRun(string[] args)
        {
            // Validate file
            if (!System.IO.File.Exists(args[0]))
            {
                Console.WriteLine($@"Invalid argument. The given argument {args[0]} is not a valid file-path");
                return null;
            }
            var input = System.IO.File.ReadAllText(args[0]);

            StartupData startupData = JsonConvert.DeserializeObject<StartupData>(input);
            
            // Validate BaseUri
            if (!startupData.BaseUri.IsAbsoluteUri)
            {
                Uri.TryCreate(startupData.BaseUri.OriginalString, UriKind.Absolute, out var result);
                if (result == null)
                {
                    Console.WriteLine($@"Invalid argument. The given BaseUri {startupData.BaseUri.OriginalString} could not be parsed into a valid Absolute Uri. Use format http://www.google.com");
                    return null;
                }
                startupData.BaseUri = result;        
            }

            // Validate MaximumUrlSegments
            if (startupData.MaximumUrlSegments < 2) 
            {
                Console.WriteLine(@"Invalid argument. MaximumUrlSegments must be more than 1");
                return null;
            }

            // Validate ResultsFolder

            TestRun testRun = new TestRun
            {
                BaseUri = startupData.BaseUri,
                MaximumUrlSegments = startupData.MaximumUrlSegments,
                ResultsFolder = startupData.ResultsFolder.Replace(@"\", @"\\"),
                EpiLoginCookieValue = startupData.EpiLoginCookieValue,
                IgnoredUrlSegments = startupData.IgnoredUrlSegments,
                ValidColors = startupData.ValidColors,
                ValidIcons = startupData.ValidIcons
            };
            testRun.Pages.Add(new Page(startupData.BaseUri, testRun));


            return testRun;
        }
    }
}
