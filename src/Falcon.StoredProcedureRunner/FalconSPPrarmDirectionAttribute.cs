using System;
using System.Data;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 定义存储过程参数方向。默认Input
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false,Inherited =false)]
    public class FalconSPPrarmDirectionAttribute:Attribute
    {
        /// <summary>
        /// 参数方向
        /// </summary>
        public ParameterDirection Direction { get; }

        /// <summary>
        /// 定义参数方向
        /// </summary>
        /// <param name="direction"></param>
        public FalconSPPrarmDirectionAttribute(ParameterDirection direction)
            => this.Direction = direction;

    }
}
