using System;
using System.Collections.Generic;

namespace SiteCrawler.TestTemplates
{
    public static class VerifyNoInvalidLinks
    {
        public static TestResult Run(List<Uri> links)
        {
            var testResult = new TestResult("VerifyNoInvalidLinks") {Result = Result.Passed};
            var invalidUrlSegments = new List<string> { "/episerver/" };
            foreach (var invalidSegment in invalidUrlSegments)
            {
                foreach (var unused in links)
                {
                    if (links.ToString().ToLower().Contains(invalidSegment))
                    {
                        testResult.Result = Result.Failed;
                        testResult.Comment = $"The page contains links to {invalidSegment}\n";
                    }
                }
            }
            return testResult;
        }
    }
}
