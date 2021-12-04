using System;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 定义存储过程参数类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FalconSPPrarmTypeAttribute:Attribute
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public FalconSPDbType PType { get; set; }
        /// <summary>
        /// 定于存储过程参数名称
        /// </summary>
        /// <param name="pDbType">参数类型.DbType</param>
        public FalconSPPrarmTypeAttribute(FalconSPDbType pDbType) { this.PType = pDbType; }
    }
}
