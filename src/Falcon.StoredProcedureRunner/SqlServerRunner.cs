using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 面向SqlServer的执行器
    /// </summary>
    public class SqlServerRunner : RunnerBase, ISqlServerRunner
    {
        /// <summary>
        /// 执行参数
        /// </summary>
        public SqlServerRunnerOtions Options { get; set; }
        /// <summary>
        /// 通过提供构造选项构造Oracle执行器
        /// </summary>
        /// <param name="option">选项</param>
        public SqlServerRunner(IOptions<SqlServerRunnerOtions> option = null)
        {
            this.Options = option?.Value ?? new SqlServerRunnerOtions();
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="tableName">表名称</param>
        /// <returns>存在True，不存在False</returns>
        protected override bool IsTableExists(DbContext db, string tableName)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<object> Run(DbContext db, Type prarmType, Type returnType, object data)
        {
            return RunByReturn(db, prarmType, returnType, data);
        }

        protected IEnumerable<object> RunByReturn(DbContext db, Type prarmType, Type returnType, object data)
        {
            var pm = GetProcuderName(prarmType);
            var paras = getParams(prarmType, data).ToArray();
            DbConnection connection = db.Database.GetDbConnection();
            DataSet ds = new();
            var conn = db.Database.GetDbConnection() as SqlConnection;
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = pm;
            cmd.Parameters.AddRange(paras);
            var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cmd.Parameters.Clear();
            return DataSetToIEnumerable(ds, returnType);
        }
        /// <summary>
        /// 获取SqlServer数据库链接
        /// </summary>
        protected override DbConnection GetDbConnection(DbContext context)
        {
            return context.Database.GetDbConnection();
        }
        /// <summary>
        /// 获取存储过程参数前缀
        /// </summary>
        /// <returns></returns>
        protected override string GetParamProfix()
        {
            return "@";
        }

        protected override IDataParameter CreateDbParameter(string name, object val, int? type, int? size)
        {
            DbParameter par =
                type.HasValue && size.HasValue ? new SqlParameter(name, (SqlDbType)type, size.Value) :
                type.HasValue && !size.HasValue ? new SqlParameter(name, (SqlDbType)type) :
                !type.HasValue && !size.HasValue ? new SqlParameter(name, val) :
                null;
            par.Value = val;
            return par as IDataParameter;
        }
    }
}
