using System.Net;

namespace SiteCrawler.TestTemplates
{
    public static class VerifyResponseCode
    {     
        public static TestResult Run(HttpStatusCode responseCode)
        {
            var testResult = new TestResult("VerifyResponseCode");
            switch (responseCode)
            {
                case HttpStatusCode.OK:
                    testResult.Result = Result.Passed;
                    break;
                default:
                    testResult.Result = Result.Failed;
                    testResult.Comment = $"Response code is not OK. Response code is {responseCode}\n";
                    break;
            }
            return testResult;
        }
    }
}
