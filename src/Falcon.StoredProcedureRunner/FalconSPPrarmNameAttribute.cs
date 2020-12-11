﻿using System;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 定义名称
    /// </summary>
    public class FalconSPPrarmNameAttribute:Attribute
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 定于存储过程参数名称
        /// </summary>
        /// <param name="name">参数名称</param>
        public FalconSPPrarmNameAttribute(string name) { this.Name = name; }
    }
}
