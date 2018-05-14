using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SiteCrawler.TestTemplates
{
    public static class VerifyNoInvalidIcons
    {
        public static TestResult Run(string content, List<string> validicons)
        {
            var testResult = new TestResult("VerifyNoInvalidColors");
            List<string> invalidIcons = new List<string>();

            MatchCollection iconMatches = Regex.Matches(content, "\\s(icon-[0-9a-zA-Z-]+)");
            foreach (Match match in iconMatches)
            {
                var iconMatch = match.Groups[1].Value;
                if (!validicons.Contains(iconMatch))
                {
                    testResult.Result = Result.Failed;

                    if (!invalidIcons.Contains(iconMatch))
                    {
                        invalidIcons.Add(iconMatch);

                        if (invalidIcons.Count > 0)
                        {
                            testResult.Comment = testResult.Comment += $"Page contains icon invalid icon: {iconMatch}\n";
                        }
                    }
                }
            }

            if (invalidIcons.Count == 0)
            {
                testResult.Result = Result.Passed;
            }

            return testResult;
        }
    }
}
