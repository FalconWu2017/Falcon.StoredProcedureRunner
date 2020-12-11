using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 存储过程执行
    /// </summary>
    public partial class Runner:IRunner
    {
        /// <summary>
        /// 执行无返回值的存储过程
        /// </summary>
        /// <typeparam name="TPrarmType">参数类型</typeparam>
        /// <param name="db">数据上下文</param>
        /// <param name="data">参数数据</param>
        public int Execute<TPrarmType>(DbContext db,TPrarmType data) {
            var parms = getParams(data).ToArray();
            var pName = getProcuderName<TPrarmType>();

#if NETSTANDARD2_1
            return db.Database.ExecuteSqlRaw(pName,parms);
#else
            return db.Database.ExecuteSqlCommand(pName,parms);
#endif
            ;
        }

        /// <summary>
        /// 根据模型定义参数执行存储过程进行查询，参数类型必须定义ReturnTypeAttribute特性
        /// </summary>
        /// <typeparam name="TPrarmType">存储过程参数类型</typeparam>
        /// <param name="db">数据上下文</param>
        /// <param name="data">存储过程参数</param>
        /// <returns>返回类型枚举FalconSPReturnTypeAttribute定义的类型枚举。</returns>
        public IEnumerable<object> Run<TPrarmType>(DbContext db,TPrarmType data) {
            var rType = getRequtnType(typeof(TPrarmType));
            if(rType == null) {
                throw new Exception("必须在参数类型上设置ReturnTypeAttribute");
            }
            return Run(db,typeof(TPrarmType),rType,data);
        }

        /// <summary>
        /// 通过数据库上下文执行存储过程，并返回查询结果
        /// </summary>
        /// <typeparam name="TPrarmType">参数类型</typeparam>
        /// <typeparam name="TReturnType">返回结果项类型</typeparam>
        /// <param name="db">数据上下文</param>
        /// <param name="data">参数数据</param>
        /// <returns>查询结果枚举</returns>
        public IEnumerable<TReturnType> Run<TPrarmType, TReturnType>(DbContext db,TPrarmType data) where TReturnType : class, new() {
            return Run(db,typeof(TPrarmType),typeof(TReturnType),data).Cast<TReturnType>();
        }

        /// <summary>
        /// 通过数据库上下文执行存储过程，并返回查询结果
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="prarmType">参数类型</param>
        /// <param name="returnType">返回值类型</param>
        /// <param name="data">存储过程参数</param>
        /// <returns>查询结果枚举</returns>
        public IEnumerable<object> Run(DbContext db,Type prarmType,Type returnType,object data) {
            var pm = getProcuderName(prarmType);
            var paras = getParams(data).ToArray();
            var connection = db.Database.GetDbConnection();
            using(var cmd = connection.CreateCommand()) {
                cmd.CommandText = pm;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);
                connection.Open();
                var dr = cmd.ExecuteReader();
                var result = new List<object>();
                if(!dr.CanGetColumnSchema())
                    return result;
                while(dr.Read()) {
                    var item = returnType.Assembly.CreateInstance(returnType.FullName);
                    var columnSchema = dr.GetColumnSchema();
                    for(var i = 0;i < columnSchema.Count;i++) {
                        var name = dr.GetName(i);
                        var value = dr.IsDBNull(i) ? null : dr.GetValue(i);
                        var pi = getProperty(returnType,name);
                        if(pi == null || !pi.CanWrite)
                            continue;
                        pi.SetValue(item,value);
                    }
                    result.Add(item);
                }
                connection.Close();
                return result;
            }
        }
    }

    public partial class Runner
    {
        /// <summary>
        /// 获取存储过程名
        /// </summary>
        /// <param name="pramType">参数类型</param>
        private static string getProcuderName(Type pramType) {
            var attr = pramType.GetCustomAttribute<FalconSPProcuderNameAttribute>();
            if(attr != null && attr is FalconSPProcuderNameAttribute pna && !string.IsNullOrEmpty(pna.ProcuderName)) {
                return pna.ProcuderName;
            }
            return pramType.Name;
        }

        /// <summary>
        /// 获取存储过程名返回值类型
        /// </summary>
        /// <param name="pramType">返回值类型</param>
        private static Type getRequtnType(Type pramType) {
            var attr = pramType.GetCustomAttribute<FalconSPReturnTypeAttribute>();
            if(attr != null && attr is FalconSPReturnTypeAttribute pna && pna.ReturnType != null) {
                return pna.ReturnType;
            }
            return null;
        }

        /// <summary>
        /// 获取存储过程参数枚举
        /// </summary>
        /// <typeparam name="T">参数模型类型</typeparam>
        /// <param name="data">参数实例</param>
        private static IEnumerable<SqlParameter> getParams<T>(T data) {
            if(data == null)
                yield break;
            foreach(var p in typeof(T).GetProperties()) {
                if(!p.CanRead || ignoreProp(p))
                    continue;
                yield return new SqlParameter($"@{getPrarmName(p)}",p.GetValue(data));
            }
        }

        /// <summary>
        /// 是否忽略属性
        /// </summary>
        /// <param name="p">要检查的属性</param>
        private static bool ignoreProp(PropertyInfo p) {
            return p.GetCustomAttribute<FalconSPIgnoreAttribute>(true) != null;
        }

        /// <summary>
        /// 获取存储过程参数名称
        /// </summary>
        /// <param name="p">对应的属性</param>
        private static string getPrarmName(PropertyInfo p) {
            var np = p.GetCustomAttribute<FalconSPPrarmNameAttribute>(true);
            if(np != null && np is FalconSPPrarmNameAttribute na) {
                return na.Name;
            }
            return p.Name;
        }

        /// <summary>
        /// 获取存储过程名
        /// </summary>
        /// <typeparam name="T">参数模型</typeparam>
        private static string getProcuderName<T>() {
            var attr = typeof(T).GetCustomAttribute<FalconSPProcuderNameAttribute>(true);
            if(attr != null && attr is FalconSPProcuderNameAttribute pna && !string.IsNullOrEmpty(pna.ProcuderName)) {
                return pna.ProcuderName;
            }
            return typeof(T).Name;
        }
        /// <summary>
        /// 忽略大小写获取类型的属性
        /// </summary>
        /// <param name="t">类型</param>
        /// <param name="name">属性名称</param>
        /// <returns>属性</returns>
        private static PropertyInfo getProperty(Type t,string name) {
            foreach(var item in t.GetProperties()) {
                if(item.Name.ToLower() == name.ToLower()) {
                    return item;
                }
            }
            return null;
        }

    }
}
