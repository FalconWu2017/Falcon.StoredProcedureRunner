using Falcon.StoredProcedureRunnerTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Falcon.StoredProcedureRunner.Tests
{
    [TestClass()]
    public class RunnerTests
    {
        [TestMethod("Runner创建测试")]
        public void RunTest()
        {
            var config = GetConfiguration();
            var services = CreateServiceScope(config);
            System.Console.WriteLine("数据注入完成");
            System.Console.WriteLine("sqlRunner开始测试");
            var sqlRunner = GetSqlRunner(services);
            Assert.IsNotNull(sqlRunner);
            Assert.IsNotNull(sqlRunner as ISqlServerRunner);
            Assert.IsNotNull(sqlRunner as SqlServerRunner);
            var sRunner = sqlRunner as SqlServerRunner;
            Assert.IsNotNull(sRunner.Options);
            System.Console.WriteLine("OracleRunner开始测试");
            var oracleRunner = GetOracleRunner(services);
            Assert.IsNotNull(oracleRunner);
            Assert.IsNotNull(oracleRunner as IOracleRunner);
            Assert.IsNotNull(oracleRunner as OracleRunner);
            var oRunner = oracleRunner as OracleRunner;
            Assert.IsNotNull(oRunner.Options);
            Assert.IsTrue(oRunner.Options.ReturnModel == ReturnModel.TempTable);

            //OracleTest();
            //System.Console.WriteLine("---------------------");
            //SqlserverTest();
        }


        [TestMethod("Sqlserver执行测试")]
        public void SqlserverTest()
        {
            var config = GetConfiguration();
            var services = CreateServiceScope(config);
            var db = CreateSqlDbContext(config);
            Assert.IsNotNull(db);
            var runner = GetSqlRunner(services);
            Assert.IsNotNull(runner);
            var result = runner.Run<SqlserverModel, SqlserverModelResult>(db, new SqlserverModel
            {
                p1 = 1,
                p2 = 2,
                p3 = "abc,"
            }).ToList();
            Assert.IsNotNull(result);
            System.Console.WriteLine("result ok");
            CollectionAssert.AllItemsAreNotNull(result);
            Assert.IsTrue(result.Count > 0);
            System.Console.WriteLine($"result count {result.Count}");
            System.Console.WriteLine("SqlserverTest over!");
        }

        [TestMethod("Oracle执行测试")]
        public void OracleTest()
        {
            var config = GetConfiguration();
            var services = CreateServiceScope(config);
            var db = CreateOracleDbContext(config);
            Assert.IsNotNull(db);
            var runner = GetOracleRunner(services);
            Assert.IsNotNull(runner);

            var result = runner.Run<Tjjk_Tjgh, Tjjk_TjghResult>(db, new Tjjk_Tjgh
            {
                v_yljgdm = "50e3d44d-9ca2-4fbd-9d5d-d32339b1b113",
                v_ghrq = "20211105",
                v_sfz = "610528199002165174",
            }).ToList();
            Assert.IsNotNull(result);
            System.Console.WriteLine("result ok");
            CollectionAssert.AllItemsAreNotNull(result);
            Assert.IsTrue(result.Count > 0);
            System.Console.WriteLine($"result count {result.Count}");
            System.Console.WriteLine("OracleTest over!");
        }

        [TestMethod("Oracle原始执行执行测试")]
        public void OracleRawTest()
        {
            var config = GetConfiguration();
            var services = CreateServiceScope(config);
            var db = CreateOracleDbContext(config);
            Assert.IsNotNull(db);
            var cmd = db.Database.GetDbConnection().CreateCommand() as OracleCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Tjjk_Tjgh";
            var a = new OracleParameter(":v_yljgdm", OracleDbType.Char, 36)
            {
                Value = "50e3d44d-9ca2-4fbd-9d5d-d32339b1b113"
            };
            cmd.Parameters.Add(a);

            var c = new OracleParameter(":v_sfz", OracleDbType.Varchar2, 36);
            c.Value = "610528199002165174";
            cmd.Parameters.Add(c);

            var b = new OracleParameter(":v_ghrq", OracleDbType.Char, 8);
            b.Value = "20211105";
            cmd.Parameters.Add(b);
            db.Database.GetDbConnection().Open();
            cmd.ExecuteNonQuery();
            cmd.CommandText = "select * from tt_tjjk_tjgh";
            cmd.CommandType = CommandType.Text;
            var da = new OracleDataAdapter(cmd);
            DataSet ds = new();
            da.Fill(ds);
            System.Console.WriteLine(ds.Tables[0].Rows.Count);
        }

        public DbContext CreateSqlDbContext(IConfiguration config)
        {
            var ora = config.GetSection("db:sqlserver").Value;
            var buider = new DbContextOptionsBuilder();
            return new DbContext(buider.UseSqlServer(ora).Options);
        }

        public DbContext CreateOracleDbContext(IConfiguration config)
        {
            var ora = config.GetSection("db:oracle").Value;
            var oraVer = config.GetSection("db:ver").Value;
            var buider = new DbContextOptionsBuilder();
            return new DbContext(buider.UseOracle(ora, o => o.UseOracleSQLCompatibility(oraVer)).Options);
        }

        ///// <summary>
        ///// 体检接口 挂号查询测试
        ///// </summary>
        //[TestMethod()]
        //public void Tjjk_TjghTest()
        //{
        //    System.Console.WriteLine("OracleTest start!");
        //    var configurationBuilder = new ConfigurationBuilder();
        //    configurationBuilder.AddJsonFile("AppSettings.json");
        //    configurationBuilder.AddUserSecrets(this.GetType().Assembly);
        //    var config = configurationBuilder.Build();
        //    var ora = config.GetSection("db:oracle").Value;
        //    var oraVer = config.GetSection("db:ver").Value;
        //    var buider = new DbContextOptionsBuilder();
        //    var db = new DbContext(buider.UseOracle(ora, o => o.UseOracleSQLCompatibility(oraVer)).Options);

        //    IRunner runner = new Runner();
        //    var result = runner.Run<Tjjk_Tjgh, Tjjk_TjghResult>(db, new Tjjk_Tjgh
        //    {
        //        v_ghrq = "20211105",
        //        v_sfz = "610528199002165174",
        //        v_yljgdm = "50e3d44d-9ca2-4fbd-9d5d-d32339b1b113",
        //    }).ToList();
        //    Assert.IsNotNull(result);
        //    System.Console.WriteLine("result ok");
        //    CollectionAssert.AllItemsAreNotNull(result);
        //    Assert.IsTrue(result.Count > 0);
        //    System.Console.WriteLine($"result count {result.Count}");
        //    System.Console.WriteLine("OracleTest over!");
        //}

        //[TestMethod()]
        //public void GetDoctorDataTest()
        //{
        //    System.Console.WriteLine("OracleTest start!");
        //    var configurationBuilder = new ConfigurationBuilder();
        //    configurationBuilder.AddJsonFile("AppSettings.json");
        //    configurationBuilder.AddUserSecrets(this.GetType().Assembly);
        //    var config = configurationBuilder.Build();
        //    var ora = config.GetSection("db:oracle").Value;
        //    var oraVer = config.GetSection("db:ver").Value;
        //    var buider = new DbContextOptionsBuilder();
        //    var db = new DbContext(buider.UseOracle(ora, o => o.UseOracleSQLCompatibility(oraVer)).Options);

        //    IRunner runner = new Runner();
        //    var result = runner.Run<GetDoctorData, GetDoctorDataResult>(db, new GetDoctorData
        //    {
        //        v_orgaid = "50e3d44d-9ca2-4fbd-9d5d-d32339b1b113",
        //    }).ToList();
        //    Assert.IsNotNull(result);
        //    System.Console.WriteLine("result ok");
        //    CollectionAssert.AllItemsAreNotNull(result);
        //    Assert.IsTrue(result.Count > 0);
        //    System.Console.WriteLine($"result count {result.Count}");
        //    System.Console.WriteLine("OracleTest over!");
        //}

        public static IConfiguration GetConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("AppSettings.json");
            configurationBuilder.AddUserSecrets(typeof(RunnerTests).Assembly);
            return configurationBuilder.Build();
        }


        private IServiceScope CreateServiceScope(IConfiguration config)
        {
            IServiceCollection services = new ServiceCollection();
            AddOptions(config, services);
            services.AddFalconSPRunner();
            return services.BuildServiceProvider().CreateScope();
        }

        public static void AddOptions(IConfiguration config, IServiceCollection services)
        {
            services.Configure<OracleRunnerOptions>(config.GetSection("OracleRunner"));
            services.Configure<SqlServerRunnerOtions>(config.GetSection("SqlRunner"));
        }

        public static IRunner? GetSqlRunner(IServiceScope services)
        {
            return services.ServiceProvider.GetService<ISqlServerRunner>() as IRunner;
        }

        public static IRunner? GetOracleRunner(IServiceScope services)
        {
            return services.ServiceProvider.GetService<IOracleRunner>() as IRunner;
        }
    }
}