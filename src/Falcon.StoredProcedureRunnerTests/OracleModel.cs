using Falcon.StoredProcedureRunner;
using System.Data;

namespace Falcon.StoredProcedureRunnerTests
{
    internal class GetEmployee
    {
        public string v_orgaid { get; set; }

        [FalconSPPrarmDirection(ParameterDirection.Output)]
        [FalconSPPrarmType(FalconSPDbType.OracleRefCursor)]
        public object v_data { get; set; }
    }
    internal class GetEmployeeResult
    {
        public string empcode { get; set; }
        public string empname { get; set; }
        public string empactive { get; set; }
    }
}
