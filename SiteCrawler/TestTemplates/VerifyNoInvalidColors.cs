using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SiteCrawler.TestTemplates
{
    public static class VerifyNoInvalidColors
    {
        public static TestResult Run(string content, List<string> validColors)
        {
            var testResult = new TestResult("VerifyNoInvalidColors");
            List<string> invalidColors = new List<string>();

            var hexMatches = Regex.Matches(content, "#([0-9a-fA-F]{3}){1,2}([, \"])");
            foreach (Match match in hexMatches)
            {
                string lowerMatch = match.ToString().ToLower().Remove(match.Length - 1);

                if (!validColors.Contains(lowerMatch))
                {
                    testResult.Result = Result.Failed;

                    if (!invalidColors.Contains(lowerMatch))
                    {
                        invalidColors.Add(lowerMatch);
                        testResult.Comment = testResult.Comment += $"Page contains icon invalid color: {lowerMatch}\n";
                    }
                }
            }

            if(invalidColors.Count == 0)
            {
                testResult.Result = Result.Passed;
            }
            return testResult;
        }
    }
}
