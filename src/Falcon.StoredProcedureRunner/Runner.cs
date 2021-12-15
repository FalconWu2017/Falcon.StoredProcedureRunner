using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 存储过程执行
    /// </summary>
    public partial class Runner : IRunner
    {
        /// <summary>
        /// 执行无返回值的存储过程
        /// </summary>
        /// <typeparam name="TPrarmType">参数类型</typeparam>
        /// <param name="db">数据上下文</param>
        /// <param name="data">参数数据</param>
        public int Execute<TPrarmType>(DbContext db, TPrarmType data)
        {
            throw new NotSupportedException();
            //var parms = getParams(db, typeof(TPrarmType), data).ToArray();
            //var pName = getProcuderName<TPrarmType>();
            //var conn = db.Database.GetDbConnection();


            //var paramStr = getParamStr(typeof(TPrarmType), data);
            //var str = $"exec {pName} {paramStr}";
            //return db.Database.ExecuteSqlRaw(str, parms);
        }

        /// <summary>
        /// 通过数据库上下文执行存储过程，并返回查询结果
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="prarmType">参数类型</param>
        /// <param name="returnType">返回值类型</param>
        /// <param name="data">存储过程参数</param>
        /// <returns>查询结果枚举</returns>
        public IEnumerable<object> Run(DbContext db, Type prarmType, Type returnType, object data)
        {
            return RunByReader(db, prarmType, returnType, data);
        }


        /// <summary>
        /// 通过数据库上下文执行存储过程，并返回查询结果
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="prarmType">参数类型</param>
        /// <param name="returnType">返回值类型</param>
        /// <param name="data">存储过程参数</param>
        /// <returns>查询结果枚举</returns>
        public IEnumerable<object> RunByReader(DbContext db, Type prarmType, Type returnType, object data)
        {
            var pm = getProcuderName(prarmType);
            var paras = getParams(db, prarmType, data).ToArray();
            var connection = db.Database.GetDbConnection();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = pm;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(paras);
            connection.Open();
            using var dr = cmd.ExecuteReader();
            var columnSchema = dr.GetColumnSchema();
            var result = new List<object>();
            if (!dr.CanGetColumnSchema())
                return result;
            int rowId = 0;
            while (dr.Read())
            {
                var item = returnType.Assembly.CreateInstance(returnType.FullName);
                for (var i = 0; i < columnSchema.Count; i++)
                {
                    var name = dr.GetName(i);
                    var value = dr.IsDBNull(i) ? null : dr.GetValue(i);
                    var pi = getProperty(returnType, name);
                    if (pi == null || !pi.CanWrite)
                        continue;
                    try
                    {
                        pi.SetValue(item, value);
                    }
                    catch (Exception ex)
                    {
                        throw new ReturnDataSetValueException(rowId, name, pi, value, ex);
                    }
                }
                result.Add(item);
                rowId++;
            }
            connection.Close();
            return result;
        }

        /// <summary>
        /// 通过数据库上下文执行存储过程，并返回查询结果
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="prarmType">参数类型</param>
        /// <param name="returnType">返回值类型</param>
        /// <param name="data">存储过程参数</param>
        /// <returns>查询结果枚举</returns>
        public IEnumerable<object> RunByDbset(DbContext db, Type prarmType, Type returnType, object data)
        {
            var result = new List<object>();
            var pm = getProcuderName(prarmType);
            var paras = getParams(db, prarmType, data).ToArray();
            DbConnection connection = db.Database.GetDbConnection();
            DataSet ds = new DataSet();
            if (db.Database.IsOracle())
            {
                var conn = db.Database.GetDbConnection() as OracleConnection;
                using var cmd = new OracleCommand(pm, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);
                var da = new OracleDataAdapter(cmd);
                da.Fill(ds);
            }
            else if (db.Database.IsSqlServer())
            {
                var oracleConn = db.Database.GetDbConnection() as SqlConnection;
                using var oracleCmd = new SqlCommand(pm, oracleConn);
                oracleCmd.CommandType = CommandType.StoredProcedure;
                oracleCmd.Parameters.AddRange(paras);
                var da = new SqlDataAdapter(oracleCmd);
                da.Fill(ds);
            }
            else
            {
                throw new DatabaseNotSupportedException();
            }
            var rows = ds.Tables[0].Rows;
            var cols = ds.Tables[0].Columns;
            for (int i = 0; i < rows.Count; i++)
            {
                var item = returnType.Assembly.CreateInstance(returnType.FullName);
                var row = rows[i];
                for (int y = 0; y < cols.Count; y++)
                {
                    var name = cols[y].ColumnName;
                    var val = row[y] is DBNull ? null : row[y];
                    var pi = getProperty(returnType, name);
                    if (pi == null || !pi.CanWrite)
                        continue;
                    try
                    {
                        pi.SetValue(item, val);
                    }
                    catch (Exception ex)
                    {
                        throw new ReturnDataSetValueException(i, name, pi, val, ex);
                    }
                }
                result.Add(item);
            }
            connection.Close();
            return result;
        }




        /// <summary>
        /// （存在sql注入风险）执行Sql语句，并将数据库返回结果以json数据对象返回。
        /// </summary>
        /// <param name="db">数据上下文</param>
        /// <param name="sql">要执行的sql语句</param>
        /// <returns>数据库返回值json格式</returns>
        public string RunRaw(DbContext db, string sql)
        {
            var connection = db.Database.GetDbConnection();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                connection.Open();
                var dr = cmd.ExecuteReader();
                var result = new StringBuilder();
                if (!dr.CanGetColumnSchema())
                    return "";
                while (dr.Read())
                {
                    var item = new StringBuilder();
                    var columnSchema = dr.GetColumnSchema();
                    for (var i = 0; i < columnSchema.Count; i++)
                    {
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
        private static string getProcuderName(Type pramType)
        {
            var attr = pramType.GetCustomAttribute<FalconSPProcuderNameAttribute>();
            return attr?.ProcuderName ?? pramType.Name;
        }

        /// <summary>
        /// 获取存储过程参数枚举
        /// </summary>
        ///<param name="type">数据的类型</param>
        /// <param name="data">参数实例</param>
        private static IEnumerable<IDataParameter> getParams(DbContext db, Type type, object data)
        {
            if (data == null)
                yield break;
            var fixC =
                db.Database.IsOracle() ? ":" :
                db.Database.IsSqlServer() ? "@" :
                throw new DatabaseNotSupportedException();

            foreach (var p in type.GetProperties())
            {
                if (!p.CanRead || isIgnoreProp(p))
                    continue;
                var name = getPrarmName(p, fixC);
                var val = p.GetValue(data);
                var pt = getPrarmType(p);
                var para = CreateDbParameter(db, name, val, pt);
                para.Direction = GetParaDirection(p);
                yield return para;
            }
        }

        /// <summary>
        /// 获取存储过程参数类型
        /// </summary>
        /// <param name="p">对应的属性</param>
        private static int? getPrarmType(PropertyInfo p)
        {
            var np = p.GetCustomAttribute<FalconSPPrarmTypeAttribute>();
            if (np != null && np is FalconSPPrarmTypeAttribute na)
            {
                return (int)np.PType;
            }
            return null;
        }

        /// <summary>
        /// 创建存储过程参数
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="name">参数名称</param>
        /// <param name="val">参数值</param>
        /// <returns>参数</returns>
        private static IDataParameter CreateDbParameter(DbContext db, string name, object val, int? type)
        {
            var dbf = db.Database;
            if (type.HasValue)
            {
                IDataParameter pra =
                    dbf.IsOracle() ? new OracleParameter(name, (OracleDbType)type) :
                    dbf.IsSqlServer() ? new SqlParameter(name, (SqlDbType)type) :
                    throw new DatabaseNotSupportedException();
                pra.Value = val;
                return pra;
            }
            else
            {
                return
                    dbf.IsSqlServer() ? new SqlParameter(name, val) :
                    dbf.IsOracle() ? new OracleParameter(name, val) :
                    throw new DatabaseNotSupportedException();
            }
        }

        /// <summary>
        /// 获取参数的方向。默认Input
        /// </summary>
        /// <param name="p">参数映射属性</param>
        /// <returns>方向或null</returns>
        private static ParameterDirection GetParaDirection(PropertyInfo p)
        {
            var attr = p.GetCustomAttribute<FalconSPPrarmDirectionAttribute>();
            return attr == null ? ParameterDirection.Input : attr.Direction;
        }

        /// <summary>
        /// 是否忽略属性
        /// </summary>
        /// <param name="p">要检查的属性</param>
        private static bool isIgnoreProp(PropertyInfo p)
        {
            return p.GetCustomAttribute<FalconSPIgnoreAttribute>(true) != null;
        }

        /// <summary>
        /// 获取存储过程参数名称
        /// </summary>
        /// <param name="p">对应的属性</param>
        private static string getPrarmName(PropertyInfo p, string prefixChar)
        {
            var np = p.GetCustomAttribute<FalconSPPrarmNameAttribute>(true);
            if (np != null && np is FalconSPPrarmNameAttribute na)
            {
                var name = na.Name;
                return name.StartsWith(prefixChar) ? name : prefixChar + name;
            }
            return prefixChar + p.Name;
        }

        /// <summary>
        /// 忽略大小写获取类型的属性
        /// </summary>
        /// <param name="t">类型</param>
        /// <param name="name">属性名称</param>
        /// <returns>属性</returns>
        private static PropertyInfo getProperty(Type t, string name)
        {
            foreach (var item in t.GetProperties())
            {
                if (item.Name.ToLower() == name.ToLower())
                {
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
        //private static string getParamStr(Type type,object data) {
        //    var paras = getParams(type,data).ToArray();
        //    var result = " ";
        //    for(int i = 0;i < paras.Count();i++) {
        //        result += $"{paras[i].ParameterName}={{{i}}},";
        //    }
        //    return result.TrimEnd(',');
        //}

    }
}
