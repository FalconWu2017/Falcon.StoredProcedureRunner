using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 执行器扩展
    /// </summary>
    public static class IRunnerExtend
    {
        /// <summary>
        /// 根据模型定义参数执行存储过程进行查询，参数类型必须定义ReturnTypeAttribute特性
        /// </summary>
        /// <typeparam name="TPrarmType">存储过程参数类型</typeparam>
        /// <param name="runner">执行器</param>
        /// <param name="db">数据上下文</param>
        /// <param name="data">存储过程参数</param>
        /// <returns>返回类型枚举FalconSPReturnTypeAttribute定义的类型枚举。</returns>
        public static IEnumerable<object> Run<TPrarmType>(this IRunner runner,DbContext db,TPrarmType data) {
            var dType = typeof(TPrarmType);
            var attr = dType.GetCustomAttribute<FalconSPReturnTypeAttribute>();
            if(attr != null && attr is FalconSPReturnTypeAttribute pna && pna.ReturnType != null) {
                return runner.Run(db,dType,pna.ReturnType,data);
            } else {
                throw new ReturnTypeException();
            }
        }

        /// <summary>
        /// 通过数据库上下文执行存储过程，并返回查询结果
        /// </summary>
        /// <typeparam name="TPrarmType">参数类型</typeparam>
        /// <typeparam name="TReturnType">返回结果项类型</typeparam>
        /// <param name="runner">执行器</param>
        /// <param name="db">数据上下文</param>
        /// <param name="data">参数数据</param>
        /// <returns>查询结果枚举</returns>
        public static IEnumerable<TReturnType> Run<TPrarmType, TReturnType>(this IRunner runner,DbContext db,TPrarmType data) {
            try {
                return runner.Run(db,typeof(TPrarmType),typeof(TReturnType),data).Cast<TReturnType>();
            } catch(InvalidCastException ice) {
                throw new ReturnTypeCastException(ice);
            } catch(Exception) {
                throw;
            }
        }

    }
}
