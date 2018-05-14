using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SiteCrawler.TestTemplates
{
    public static class VerifySingleH1Tag
    {
        public static TestResult Run(string content)
        {
            var testResult = new TestResult("VerifySingleH1Tag");
            int matches = Regex.Matches(content, "<h1").Count;
            if (matches > 1)
            {
                testResult.Result = Result.Failed;
                testResult.Comment = "The page contain several H1-tags\n";
            }
            else
            {
                testResult.Result = Result.Passed;
            }
            return testResult;
        }
    }
}
