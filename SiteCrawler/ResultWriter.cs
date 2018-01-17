using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.IO;

namespace SiteCrawler
{
    public class ResultWriter
    {
        public string Path { get; set; }
        private readonly Stream resourceStream;

        public ResultWriter()
        { }

        public ResultWriter(Stream resourceStream)
        {
            this.resourceStream = resourceStream;
        }

        public bool SaveToExcel(string reportPath, TestRun testRun)
        {
            try
            {
                if (File.Exists(reportPath))
                {
                    File.Delete(reportPath);
                }
            }
            catch (IOException)
            {
                return false;
            }

            using (var fs = new FileStream(reportPath, FileMode.CreateNew))
            {
                resourceStream.CopyTo(fs);
                resourceStream.Dispose();
            }

            FileInfo newFile = new FileInfo(reportPath);
            ExcelPackage pck = new ExcelPackage(newFile);
            var ws = pck.Workbook.Worksheets["Tests"];

            for (int i = 0; i < testRun.Pages.Count; i++)
            {
                ws.Cells[i + 5, 1].Value = testRun.Pages[i].Uri;
                ws.Cells[i + 5, 2].Value = testRun.Pages[i].TestResult;
                ws.Cells[i + 5, 3].Value = testRun.Pages[i].Source;
                //ws.Cells[i + 5, 4].Value = String.Join("\n ", Enumerable.Repeat(model.Results[i].GeneralError, 1).Concat(model.Results[i].Results.Select(s => s.ResultMessage)).Where(s => !string.IsNullOrWhiteSpace(s)));
                //ws.Cells[i + 5, 5].Value = model.Results[i].Duration;
            }

            pck.Save();

            return true;

        }

        public bool SaveToJson(string resultsFolder, TestRun testRun)
        {
            try
            {
                using (StreamWriter file = File.CreateText(System.IO.Path.Combine(resultsFolder, "CrawlerResult" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".txt")))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serialize object directly into file stream
                    serializer.Serialize(file, testRun);
                }
            }
            catch (Exception) { return false; }

            return true;
        }
    }
}
