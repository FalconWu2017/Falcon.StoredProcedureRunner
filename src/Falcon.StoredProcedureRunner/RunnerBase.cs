using Falcon.StoredProcedureRunner.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
    public abstract class RunnerBase : IRunner
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
        public abstract IEnumerable<object> Run(DbContext db, Type prarmType, Type returnType, object data);


        /// <summary>
        /// 通过数据库上下文执行存储过程，并返回查询结果
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="prarmType">参数类型</param>
        /// <param name="returnType">返回值类型</param>
        /// <param name="data">存储过程参数</param>
        /// <returns>查询结果枚举</returns>
        protected virtual IEnumerable<object> RunByReader(DbContext db, Type prarmType, Type returnType, object data)
        {
            var pm = GetProcuderName(prarmType);
            var paras = getParams( prarmType, data).ToArray();
            var conn = GetDbConnection(db);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = pm;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(paras);
            conn.Open();
            using var dr = cmd.ExecuteReader();
            cmd.Parameters.Clear();
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
            conn.Close();
            return result;
        }

        /// <summary>
        /// 获取数据库链接
        /// </summary>
        /// <param name="context">数据库上下文</param>
        /// <returns>数据链接</returns>
        protected abstract DbConnection GetDbConnection(DbContext context);

        /// <summary>
        /// 将Dataset转换为IEnumerable
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="returnType">枚举元素类型</param>
        /// <returns>枚举类型</returns>
        /// <exception cref="ReturnDataSetValueException">数据赋值异常</exception>
        public IEnumerable<object> DataSetToIEnumerable(DataSet ds, Type returnType)
        {
            var result = new List<object>();
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
            using var cmd = connection.CreateCommand();
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

        /// <summary>
        /// 获取返回类型临时表名称
        /// </summary>
        /// <param name="returnType">返回类型</param>
        /// <returns></returns>
        protected virtual string GetTempTableName(Type returnType)
        {
            var attr = returnType.GetCustomAttribute<FalconSPTempTableAttribute>();
            return attr?.TableName ?? returnType.Name;
        }

        /// <summary>
        /// 获取存储过程名
        /// </summary>
        /// <param name="pramType">参数类型</param>
        protected virtual string GetProcuderName(Type pramType)
        {
            var attr = pramType.GetCustomAttribute<FalconSPProcuderNameAttribute>();
            return attr?.ProcuderName ?? pramType.Name;
        }

        /// <summary>
        /// 获取存储过程参数枚举
        /// </summary>
        ///<param name="type">数据的类型</param>
        /// <param name="data">参数实例</param>
        protected virtual IEnumerable<IDataParameter> getParams(Type type, object data)
        {
            if (data == null)
                yield break;
            var fixC = GetParamProfix();
            foreach (var p in type.GetProperties())
            {
                if (!p.CanRead || IsIgnoreProp(p))
                    continue;
                var name = GetPrarmName(p, fixC);
                var val = p.GetValue(data);
                var pt = getPrarmType(p);
                var para = CreateDbParameter(name, val, pt.type, pt.size);
                para.Direction = GetParaDirection(p);
                yield return para;
            }
        }

        /// <summary>
        /// 获取数据库参数前缀
        /// </summary>
        /// <returns>前缀字符串</returns>
        protected abstract string GetParamProfix();

        /// <summary>
        /// 获取存储过程参数类型
        /// </summary>
        /// <param name="p">对应的属性</param>
        private (int? type, int? size) getPrarmType(PropertyInfo p)
        {
            var np = p.GetCustomAttribute<FalconSPPrarmTypeAttribute>();
            if (np != null && np is FalconSPPrarmTypeAttribute na)
            {
                return ((int)np.PType, np.Size);
            }
            return (null, null);
        }

        /// <summary>
        /// 创建存储过程参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="val">参数值</param>
        /// <param name="type">存储过程参数数据库类型</param>
        /// <param name="size">数据长度</param>
        /// <returns>参数</returns>
        protected abstract IDataParameter CreateDbParameter(string name, object val, int? type, int? size);

        /// <summary>
        /// 获取参数的方向。默认Input
        /// </summary>
        /// <param name="p">参数映射属性</param>
        /// <returns>方向或null</returns>
        protected virtual ParameterDirection GetParaDirection(PropertyInfo p)
        {
            var attr = p.GetCustomAttribute<FalconSPPrarmDirectionAttribute>();
            return attr == null ? ParameterDirection.Input : attr.Direction;
        }

        /// <summary>
        /// 是否忽略属性
        /// </summary>
        /// <param name="p">要检查的属性</param>
        protected virtual bool IsIgnoreProp(PropertyInfo p)
        {
            return p.GetCustomAttribute<FalconSPIgnoreAttribute>(true) != null;
        }

        /// <summary>
        /// 获取存储过程参数名称
        /// </summary>
        /// <param name="p">对应的属性</param>
        /// <param name="prefixChar">存储过程参数前缀</param>
        protected virtual string GetPrarmName(PropertyInfo p, string prefixChar)
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
        protected virtual PropertyInfo getProperty(Type t, string name)
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
        /// 表是否存在
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="tableName">表名称</param>
        /// <returns>存在True，不存在False</returns>
        protected abstract bool IsTableExists(DbContext db, string tableName);
    }
}
