using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Transactions;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 面向Oracle的执行器
    /// </summary>
    public class OracleRunner : RunnerBase, IOracleRunner
    {
        /// <summary>
        /// 执行参数
        /// </summary>
        public OracleRunnerOptions Options { get; set; }
        /// <summary>
        /// 通过提供构造选项构造Oracle执行器
        /// </summary>
        /// <param name="option">选项</param>
        public OracleRunner(IOptions<OracleRunnerOptions> option = null)
        {
            this.Options = option?.Value ?? new OracleRunnerOptions();
        }

        /// <summary>
        /// 创建存储过程参数
        /// </summary>
        protected override IDataParameter CreateDbParameter(string name, object val, int? type, int? size)
        {
            DbParameter par =
                type.HasValue && size.HasValue ? new OracleParameter(name, (OracleDbType)type, size.Value) :
                type.HasValue && !size.HasValue ? new OracleParameter(name, (OracleDbType)type) :
                !type.HasValue && !size.HasValue ? new OracleParameter(name, val) :
                null;
            par.Value = val;
            return par as IDataParameter;
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

        /// <summary>
        /// 获取Oracle数据库链接
        /// </summary>
        protected override DbConnection GetDbConnection(DbContext context)
        {
            return context.Database.GetDbConnection() as OracleConnection;
        }

        /// <summary>
        /// 获取数据库前缀
        /// </summary>
        /// <returns>前缀</returns>
        protected override string GetParamProfix()
        {
            return ":";
        }

        public override IEnumerable<object> Run(DbContext db, Type prarmType, Type returnType, object data)
        {
            return this.Options.ReturnModel switch
            {
                ReturnModel.Return => RunByReturn(db, prarmType, returnType, data),
                ReturnModel.TempTable => RunByTempTable(db, prarmType, returnType, data),
                _ => throw new NotSupportedException(),
            };
        }

        protected IEnumerable<object> RunByReturn(DbContext db, Type prarmType, Type returnType, object data)
        {
            var pm = GetProcuderName(prarmType);
            var paras = getParams(prarmType, data).ToArray();
            DataSet ds = new();
            var conn = db.Database.GetDbConnection() as OracleConnection;
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = pm;
            cmd.Parameters.AddRange(paras);
            var da = new OracleDataAdapter(cmd);
            da.Fill(ds);
            cmd.Parameters.Clear();
            return DataSetToIEnumerable(ds, returnType);
        }

        protected IEnumerable<object> RunByTempTable(DbContext db, Type prarmType, Type returnType, object data)
        {
            var pm = GetProcuderName(prarmType);
            var paras = getParams(prarmType, data).ToArray();
            var tempTable = GetTempTableName(returnType);
            using TransactionScope tran = new(TransactionScopeOption.Required);
            using var cmd = db.Database.GetDbConnection().CreateCommand() as OracleCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = pm;
            cmd.Parameters.AddRange(paras);
            //cmd.BindByName = true;
            db.Database.GetDbConnection().Open();
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            cmd.CommandText= $"select * from {tempTable}";
            cmd.CommandType = CommandType.Text;
            var da = new OracleDataAdapter(cmd);
            DataSet ds = new();
            da.Fill(ds);
            tran.Complete();
            return DataSetToIEnumerable(ds, returnType);
        }

    }
}
