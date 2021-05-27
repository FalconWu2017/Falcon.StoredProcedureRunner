using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
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
            var parms = getParams(typeof(TPrarmType),data).ToArray();
            var pName = getProcuderName<TPrarmType>();
            var paramStr = getParamStr(typeof(TPrarmType),data);
            var str = $"exec {pName} {paramStr}";
            return db.Database.ExecuteSqlRaw(str,parms);
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
            var paras = getParams(prarmType,data).ToArray();
            var connection = db.Database.GetDbConnection();
            using(var cmd = connection.CreateCommand()) {
                cmd.CommandText = pm;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);
                connection.Open();
                var dr = cmd.ExecuteReader();
                var result = new List<object>();
                if(!dr.CanGetColumnSchema())
                    return result;
                int rowId = 0;
                while(dr.Read()) {
                    var item = returnType.Assembly.CreateInstance(returnType.FullName);
                    var columnSchema = dr.GetColumnSchema();
                    for(var i = 0;i < columnSchema.Count;i++) {
                        var name = dr.GetName(i);
                        var value = dr.IsDBNull(i) ? null : dr.GetValue(i);
                        var pi = getProperty(returnType,name);
                        if(pi == null || !pi.CanWrite)
                            continue;
                        try {
                            pi.SetValue(item,value);
                        } catch(Exception ex) {
                            throw new ReturnDataSetValueException(rowId,name,pi,value,ex);
                        }
                    }
                    result.Add(item);
                    rowId++;
                }
                connection.Close();
                return result;
            }
        }

        /// <summary>
        /// （存在sql注入风险）执行Sql语句，并将数据库返回结果以json数据对象返回。
        /// </summary>
        /// <param name="db">数据上下文</param>
        /// <param name="sql">要执行的sql语句</param>
        /// <returns>数据库返回值json格式</returns>
        public string RunRaw(DbContext db,string sql) {
            var connection = db.Database.GetDbConnection();
            using(var cmd = connection.CreateCommand()) {
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                connection.Open();
                var dr = cmd.ExecuteReader();
                var result = new StringBuilder();
                if(!dr.CanGetColumnSchema())
                    return "";
                while(dr.Read()) {
                    var item = new StringBuilder();
                    var columnSchema = dr.GetColumnSchema();
                    for(var i = 0;i < columnSchema.Count;i++) {
                        var name = dr.GetName(i);
                        var value = dr.IsDBNull(i) ? null : dr.GetValue(i);
                        item.Append($"\"{name}\":\"{value}\",");
                    }
                    result.Append($"{{{item.ToString().TrimEnd(',')}}},");
                }
                connection.Close();
                return "[" + result.ToString().TrimEnd(',') + "]";
            }
        }
    }

    /// <summary>
    /// 内部保护方法
    /// </summary>
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
        /// 获取存储过程参数枚举
        /// </summary>
        ///<param name="type">数据的类型</param>
        /// <param name="data">参数实例</param>
        private static IEnumerable<SqlParameter> getParams(Type type,object data) {
            if(data == null)
                yield break;
            foreach(var p in type.GetProperties()) {
                if(!p.CanRead || ignoreProp(p))
                    continue;
                var pt = getPrarmType(p);
                if(pt.HasValue) {
                    var np = new SqlParameter($"@{getPrarmName(p)}",pt.Value);
                    np.Value = p.GetValue(data);
                    yield return np;
                } else {
                    yield return new SqlParameter($"@{getPrarmName(p)}",p.GetValue(data));
                }
            }
        }

        /// <summary>
        /// 获取存储过程参数类型
        /// </summary>
        /// <param name="p">对应的属性</param>
        private static SqlDbType? getPrarmType(PropertyInfo p) {
            var np = p.GetCustomAttribute<FalconSPPrarmTypeAttribute>(true);
            if(np != null && np is FalconSPPrarmTypeAttribute na) {
                return na.PType;
            }
            return null;
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

        /// <summary>
        /// 生成存储过程参数字符串
        /// </summary>
        /// <param name="type">参数类型</param>
        /// <param name="data">参数对象</param>
        /// <returns>一个参数字符串。比如@p2=@p4,@p1=@p3</returns>
        private static string getParamStr(Type type,object data) {
            var paras = getParams(type,data).ToArray();
            var result = " ";
            for(int i = 0;i < paras.Count();i++) {
                result += $"{paras[i].ParameterName}={{{i}}},";
            }
            return result.TrimEnd(',');
        }

    }
}
