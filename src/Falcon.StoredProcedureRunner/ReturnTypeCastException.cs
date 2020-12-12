using System;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 返回类型转换异常
    /// </summary>
    public class ReturnTypeCastException:Exception
    {
        /// <summary>
        /// 构造一个返回类型转换异常。
        /// </summary>
        /// <param name="ex">类型转换异常</param>
        public ReturnTypeCastException(Exception ex)
            : base("将存储过程返回结果转换为指定返回类型时发生错误！无法转换",ex) { }
    }
}
