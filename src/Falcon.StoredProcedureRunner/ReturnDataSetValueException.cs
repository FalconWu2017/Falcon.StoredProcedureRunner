using System;
using System.Reflection;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 设置返回对象值时候发生异常
    /// </summary>
    public class ReturnDataSetValueException : Exception
    {
        /// <summary>
        /// 实例化一个给返回数据设置值的异常
        /// </summary>
        /// <param name="rowId">返回数据行号</param>
        /// <param name="cName">返回的列名</param>
        /// <param name="pi">要设置值的属性</param>
        /// <param name="value">要设置的值</param>
        /// <param name="innException">内部异常</param>
        public ReturnDataSetValueException(int rowId, string cName, PropertyInfo pi, object value, Exception innException)
            : base($"存储过程返回第[{rowId}]行[{cName}]列数据值为({value.GetType().FullName})[{value}]无法赋给属性({pi.PropertyType.FullName})[{pi.Name}]。", innException) { }
    }
}
