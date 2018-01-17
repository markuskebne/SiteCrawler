using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace SiteCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            TestRun TestRun = null;

            // Startas med ett argument som är en path till en json fil
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid argument. Please give json filepath as only argument");
                Console.ReadLine();
                return;
            }

            // Parse data in json file
            TestRun = HelperMethods.ValidateArgumentAndReturnTestRun(args);
            if (TestRun == null)
            {
                Console.WriteLine("Error Invalid input file");
                Console.ReadLine();
                return;
            }

            // Crawl the site
            while ((TestRun.Pages.Where(page => page.Crawled == false)).Count() > 0)
            {
                TestRun.Pages.Where(page => page.Crawled == false).FirstOrDefault().Crawl();
            }

            // Write results to excel
            var excelWriter = new ResultWriter(typeof(Program).Assembly.GetManifestResourceStream("SiteCrawler.ResultExcelTemplate.xlsx"));
            Directory.CreateDirectory(TestRun.ResultsFolder);
            var excelReportPath = Path.Combine(TestRun.ResultsFolder, "CrawlerResult" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx");
            excelWriter.SaveToExcel(excelReportPath, TestRun);

            // write results to json
            //open file stream
            var jsonWriter = new ResultWriter();
            jsonWriter.SaveToJson(TestRun.ResultsFolder, TestRun);

            // Exit
            Console.WriteLine("Crawl Completed");
            Console.ReadLine(); 
        }
    }
}
