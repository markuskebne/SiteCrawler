using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawler.TestTemplates
{
    public static class VerifyUriScheme
    {
        public static TestResult Run(Uri baseUri, Uri targetUri)
        {
            var testResult = new TestResult("VerifyUriScheme");
            if (baseUri.Scheme != targetUri.Scheme)
            {
                testResult.Result = Result.Failed;
                testResult.Comment = $"Uri-scheme differs from test-run base-url. Uri scheme is {targetUri.Scheme} but should be {baseUri.Scheme}\n";
            }
            else
            {
                testResult.Result = Result.Passed;
            }
            return testResult;
        }
    }
}
