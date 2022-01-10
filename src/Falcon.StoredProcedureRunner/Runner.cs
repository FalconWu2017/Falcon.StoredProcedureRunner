using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Falcon.StoredProcedureRunner
{
    /// <summary>
    /// 通用存储过程执行器
    /// </summary>
    public class Runner : IRunner
    {
        /// <summary>
        /// 注册的所有服务
        /// </summary>
        public IServiceProvider Service { get; }
        /// <summary>
        /// 通过注册的服务实例化执行器
        /// </summary>
        /// <param name="service">服务集合</param>
        public Runner(IServiceProvider service)
        {
            Service = service;
        }

        public int Execute<TPrarmType>(DbContext db, TPrarmType data)
        {
            return GetRunner(db).Execute<TPrarmType>(db, data);
        }

        public IEnumerable<object> Run(DbContext db, Type prarmType, Type returnType, object data)
        {
            return GetRunner(db).Run(db, prarmType, returnType, data);
        }

        public string RunRaw(DbContext db, string sql)
        {
            return GetRunner(db).RunRaw(db, sql);
        }
        /// <summary>
        /// 获取特定类型的数据库runnner
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <returns>特定类型的执行器</returns>
        /// <exception cref="DatabaseNotSupportedException"></exception>
        protected IRunner GetRunner(DbContext db)
        {
            return
                db.Database.IsOracle() ? Service.GetService<IOracleRunner>() :
                db.Database.IsSqlServer() ? Service.GetService<IOracleRunner>() :
                throw new DatabaseNotSupportedException();
        }
    }
}
