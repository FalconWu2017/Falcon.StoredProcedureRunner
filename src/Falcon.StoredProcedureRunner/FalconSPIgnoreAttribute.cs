using System;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 表示调用时候忽略此属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FalconSPIgnoreAttribute:Attribute
    {
    }
}
