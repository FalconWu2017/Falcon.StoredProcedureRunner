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
        int Execute<TPrarmType>(DbContext db,TPrarmType data);

        /// <summary>
        /// 根据模型定义参数执行存储过程进行查询，参数类型必须定义ReturnTypeAttribute特性
        /// </summary>
        /// <typeparam name="TPrarmType">存储过程参数类型</typeparam>
        /// <param name="db">数据上下文</param>
        /// <param name="data">存储过程参数</param>
        /// <returns>返回类型枚举FalconSPReturnTypeAttribute定义的类型枚举。</returns>
        IEnumerable<object> Run<TPrarmType>(DbContext db,TPrarmType data);

        /// <summary>
        /// 通过数据库上下文执行存储过程，并返回查询结果
        /// </summary>
        /// <typeparam name="TPrarmType">参数类型</typeparam>
        /// <typeparam name="TReturnType">返回结果项类型</typeparam>
        /// <param name="db">数据上下文</param>
        /// <param name="data">参数数据</param>
        /// <returns>查询结果枚举</returns>
        IEnumerable<TReturnType> Run<TPrarmType, TReturnType>(DbContext db,TPrarmType data) where TReturnType : class, new();

        /// <summary>
        /// 通过数据库上下文执行存储过程，并返回查询结果
        /// </summary>
        /// <param name="db">数据上下文</param>
        /// <param name="prarmType">参数类型</param>
        /// <param name="returnType">返回结果项类型</param>
        /// <param name="data">执行参数</param>
        /// <returns>查询结果枚举</returns>
        IEnumerable<object> Run(DbContext db,Type prarmType,Type returnType ,object data) ;
    }
}
