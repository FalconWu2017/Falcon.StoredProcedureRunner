using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 执行数据库存储过程
    /// </summary>
    public interface IRunner
    {
        /// <summary>
        /// 通过数据库上下文执行无返回值的存储过程
        /// </summary>
        /// <typeparam name="TPrarmType">参数类型</typeparam>
        /// <param name="db">数据上下文</param>
        /// <param name="data">参数数据</param>
        int Execute<TPrarmType>(DbContext db, TPrarmType data);

        /// <summary>
        /// 通过数据库上下文执行存储过程，并返回查询结果
        /// </summary>
        /// <param name="db">数据上下文</param>
        /// <param name="prarmType">参数类型</param>
        /// <param name="returnType">返回结果项类型</param>
        /// <param name="data">执行参数</param>
        /// <returns>查询结果枚举</returns>
        IEnumerable<object> Run(DbContext db, Type prarmType, Type returnType, object data);

        /// <summary>
        /// （存在sql注入风险）执行Sql语句，并将数据库返回结果以json数据对象返回。
        /// </summary>
        /// <param name="db">数据上下文</param>
        /// <param name="sql">要执行的sql语句</param>
        /// <returns>数据库返回值json格式</returns>
        string RunRaw(DbContext db, string sql);
    }
}
