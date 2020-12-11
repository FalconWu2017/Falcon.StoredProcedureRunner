using System;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 定义存储过程名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FalconSPProcuderNameAttribute:Attribute
    {
        /// <summary>
        /// 存储过程名称
        /// </summary>
        public string ProcuderName { get; set; }
        /// <summary>
        /// 定义存储过程名称
        /// </summary>
        /// <param name="m"></param>
        public FalconSPProcuderNameAttribute(string m) => this.ProcuderName = m;
    }
}
