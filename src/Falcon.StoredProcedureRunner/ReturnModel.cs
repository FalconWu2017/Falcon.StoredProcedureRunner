namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 获取返回值方式
    /// </summary>
    public enum ReturnModel
    {
        /// <summary>
        /// 通过存储过程返回值获取数据
        /// </summary>
        Return = 1,
        /// <summary>
        /// 通过临时表表获取数据
        /// </summary>
        TempTable = 2
    }
}
