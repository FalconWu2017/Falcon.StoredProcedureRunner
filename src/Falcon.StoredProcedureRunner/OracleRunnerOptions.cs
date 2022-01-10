namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// Oracle执行配置
    /// </summary>
    public class OracleRunnerOptions
    {
        /// <summary>
        /// 获取数据返回方式 DbReader 1或TempTable 2
        /// </summary>
        public ReturnModel ReturnModel { get; set; } = ReturnModel.TempTable;
    }
}
