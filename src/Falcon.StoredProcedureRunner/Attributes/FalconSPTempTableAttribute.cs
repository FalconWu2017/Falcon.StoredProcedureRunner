using System;

namespace Falcon.StoredProcedureRunner.Attributes
{
    /// <summary>
    /// 定义存储过程返回临时表名称。主要应用与oracle数据库通过临时表返回结果。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FalconSPTempTableAttribute : Attribute
    {
        /// <summary>
        /// 存储返回临时表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 定义存储返回临时表名称
        /// </summary>
        /// <param name="name">临时表名称</param>
        public FalconSPTempTableAttribute(string name) => TableName=name;
    }
}
