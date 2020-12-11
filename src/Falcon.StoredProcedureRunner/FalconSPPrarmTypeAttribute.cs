using System;
using System.Data;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 定义存储过程参数类型
    /// </summary>
    public class FalconSPPrarmTypeAttribute:Attribute
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public SqlDbType PType { get; set; }
        /// <summary>
        /// 定于存储过程参数名称
        /// </summary>
        /// <param name="pType">参数类型</param>
        public FalconSPPrarmTypeAttribute(SqlDbType pType) { this.PType = pType; }
    }
}
