using System.Text.RegularExpressions;

namespace SiteCrawler.TestTemplates
{
    public static class FindClass
    {
        public static TestResult Run(string content, string className)
        {
            var testResult = new TestResult("FindClass");
            int matches = Regex.Matches(content, $"class=\"{className}\"").Count;
            if (matches > 0)
            {
                testResult.Comment = $"The page contain class: {className}\n";
            }
            testResult.Result = Result.Passed;
            return testResult;
        }
    }
}
