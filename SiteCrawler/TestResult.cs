using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawler
{
    public class TestResult
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public Result Result { get; set; }

        public TestResult(string name)
        {
            Name = name;
            Comment = string.Empty;
            Result = Result.NotRun;
        }
    }

}
