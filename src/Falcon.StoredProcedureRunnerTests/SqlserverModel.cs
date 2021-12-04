using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Falcon.StoredProcedureRunnerTests
{
    [Falcon.StoredProcedureRunner.FalconSPProcuderName("TestSp1")]
    public class SqlserverModel
    {
        public int p1 { get; set; }
        public int p2 { get; set; }
        public string p3 { get; set; }
    }

    public class SqlserverModelResult
    {
        public double Id { get; set; }
        public string s { get; set; }
        public double jia { get; set; }
        public double jian { get; set; }
        public double chen { get; set; }
        public double chu { get; set; }
    }
}
