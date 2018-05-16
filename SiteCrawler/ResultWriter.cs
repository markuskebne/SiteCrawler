using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.IO;

namespace SiteCrawler
{
    public class ResultWriter
    {
        public string Path { get; set; }
        private readonly Stream _resourceStream;

        public ResultWriter()
        { }

        public ResultWriter(Stream resourceStream)
        {
            _resourceStream = resourceStream;
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
                _resourceStream.CopyTo(fs);
                _resourceStream.Dispose();
            }

            FileInfo newFile = new FileInfo(reportPath);
            ExcelPackage pck = new ExcelPackage(newFile);
            var ws = pck.Workbook.Worksheets["Tests"];
            

            for (int i = 0; i < testRun.Pages.Count; i++)
            {
                ws.Cells[i + 7, 1].Value = testRun.Pages[i].Uri;
                ws.Cells[i + 7, 2].Value = testRun.Pages[i].TestResult;

                string testResultComments = string.Empty;
                foreach (var testResult in testRun.Pages[i].TestResults)
                {
                    testResultComments += testResult.Comment;
                }
                ws.Cells[i + 7, 3].Value = testResultComments;

                ws.Cells[i + 7, 4].Value = testRun.Pages[i].Id;
                ws.Cells[i + 7, 5].Value = testRun.Pages[i].Title;
                ws.Cells[i + 7, 6].Value = testRun.Pages[i].Description;
                ws.Cells[i + 7, 7].Value = testRun.Pages[i].Source;
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
