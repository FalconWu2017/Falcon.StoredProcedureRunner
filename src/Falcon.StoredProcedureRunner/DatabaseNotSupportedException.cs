using System;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 设置返回对象值时候发生异常
    /// </summary>
    public class DatabaseNotSupportedException : Exception
    {
        /// <summary>
        /// 实例化不支持的数据库对象异常
        /// </summary>
        public DatabaseNotSupportedException() : base("仅仅支持Oracle和SqlServer数据库")
        {
        }
    }
}
