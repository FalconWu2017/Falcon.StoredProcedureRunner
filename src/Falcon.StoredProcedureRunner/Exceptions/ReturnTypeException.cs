using System;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 返回类型定义异常
    /// </summary>
    public class ReturnTypeException:Exception
    {
        /// <summary>
        /// 构造一个返回类型错误异常
        /// </summary>
        public ReturnTypeException() : base("必须在参数类型上设置ReturnTypeAttribute或者通过合适重载明确指定返回数据类型。") {
        }
    }
}
