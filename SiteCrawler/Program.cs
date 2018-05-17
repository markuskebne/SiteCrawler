using System;
using System.IO;
using System.Linq;

namespace SiteCrawler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Startas med ett argument som är en path till en json fil
            if (args.Length != 1)
            {
                Console.WriteLine(@"Invalid argument. Please give json filepath as only argument");
                Console.ReadLine();
                return;
            }

            // Parse data in json file
            var testRun = HelperMethods.ValidateArgumentAndReturnTestRun(args);
            if (testRun == null)
            {
                Console.WriteLine(@"Error Invalid input file");
                Console.ReadLine();
                return;
            }

            // Crawl the site
            while ((testRun.Pages.Where(page => page.Crawled == false)).Any())
            {
                testRun.Pages.FirstOrDefault(page => page.Crawled == false)?.Crawl();
            }

            // Write results to excel
            var excelWriter = new ResultWriter(typeof(Program).Assembly.GetManifestResourceStream("SiteCrawler.ResultExcelTemplate.xlsx"));
            Directory.CreateDirectory(testRun.ResultsFolder);
            var excelReportPath = Path.Combine(testRun.ResultsFolder, "CrawlerResult" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx");
            excelWriter.SaveToExcel(excelReportPath, testRun);

            // write results to json
            //open file stream
            // var jsonWriter = new ResultWriter();
            // jsonWriter.SaveToJson(testRun.ResultsFolder, testRun);

            // Exit
            Console.WriteLine(@"Crawl Completed!");
            Console.ReadLine(); 
        }
    }
}
