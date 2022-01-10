using System;

namespace Falcon.StoredProcedureRunner.Attributes
{
    /// <summary>
    /// 定义存储过程返回值类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FalconSPReturnTypeAttribute : Attribute
    {
        /// <summary>
        /// 返回的数据类型
        /// </summary>
        public Type ReturnType { get; set; }
        /// <summary>
        /// 定义存储过程名称
        /// </summary>
        /// <param name="t">存储过程返回值类型</param>
        public FalconSPReturnTypeAttribute(Type t) => this.ReturnType = t;
    }
}
