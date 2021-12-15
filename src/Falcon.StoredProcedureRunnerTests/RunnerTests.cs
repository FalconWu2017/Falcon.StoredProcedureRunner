using Falcon.StoredProcedureRunnerTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Falcon.StoredProcedureRunner.Tests
{
    [TestClass()]
    public class RunnerTests
    {
        [TestMethod("通用测试")]
        public void RunTest()
        {
            OracleTest();
            System.Console.WriteLine("---------------------");
            SqlserverTest();
        }

        [TestMethod()]
        public void SqlserverTest()
        {
            System.Console.WriteLine("SqlserverTest start!");
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("AppSettings.json");
            configurationBuilder.AddUserSecrets(this.GetType().Assembly);
            var config = configurationBuilder.Build();
            var sql = config.GetSection("db:sqlserver").Value;
            var buider = new DbContextOptionsBuilder();
            var db = new DbContext(buider.UseSqlServer(sql).Options);
            IRunner runner = new Runner();

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

        [TestMethod()]
        public void OracleTest()
        {
            System.Console.WriteLine("OracleTest start!");
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("AppSettings.json");
            configurationBuilder.AddUserSecrets(this.GetType().Assembly);
            var config = configurationBuilder.Build();
            var ora = config.GetSection("db:oracle").Value;
            var oraVer = config.GetSection("db:ver").Value;
            var buider = new DbContextOptionsBuilder();
            var db = new DbContext(buider.UseOracle(ora, o => o.UseOracleSQLCompatibility(oraVer)).Options);

            IRunner runner = new Runner();
            var result = runner.Run<GetEmployee, GetEmployeeResult>(db, new GetEmployee
            {
                v_orgaid = "50e3d44d-9ca2-4fbd-9d5d-d32339b1b113",
            }).ToList();
            Assert.IsNotNull(result);
            System.Console.WriteLine("result ok");
            CollectionAssert.AllItemsAreNotNull(result);
            Assert.IsTrue(result.Count > 0);
            System.Console.WriteLine($"result count {result.Count}");
            System.Console.WriteLine("OracleTest over!");
        }

        /// <summary>
        /// 体检接口 挂号查询测试
        /// </summary>
        [TestMethod()]
        public void Tjjk_TjghTest()
        {
            System.Console.WriteLine("OracleTest start!");
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("AppSettings.json");
            configurationBuilder.AddUserSecrets(this.GetType().Assembly);
            var config = configurationBuilder.Build();
            var ora = config.GetSection("db:oracle").Value;
            var oraVer = config.GetSection("db:ver").Value;
            var buider = new DbContextOptionsBuilder();
            var db = new DbContext(buider.UseOracle(ora, o => o.UseOracleSQLCompatibility(oraVer)).Options);

            IRunner runner = new Runner();
            var result = runner.Run<Tjjk_Tjgh, Tjjk_TjghResult>(db, new Tjjk_Tjgh
            {
                v_ghrq = "20211105",
                v_sfz = "610528199002165174",
                v_yljgdm = "50e3d44d-9ca2-4fbd-9d5d-d32339b1b113",
            }).ToList();
            Assert.IsNotNull(result);
            System.Console.WriteLine("result ok");
            CollectionAssert.AllItemsAreNotNull(result);
            Assert.IsTrue(result.Count > 0);
            System.Console.WriteLine($"result count {result.Count}");
            System.Console.WriteLine("OracleTest over!");
        }

        [TestMethod()]
        public void GetDoctorDataTest()
        {
            System.Console.WriteLine("OracleTest start!");
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("AppSettings.json");
            configurationBuilder.AddUserSecrets(this.GetType().Assembly);
            var config = configurationBuilder.Build();
            var ora = config.GetSection("db:oracle").Value;
            var oraVer = config.GetSection("db:ver").Value;
            var buider = new DbContextOptionsBuilder();
            var db = new DbContext(buider.UseOracle(ora, o => o.UseOracleSQLCompatibility(oraVer)).Options);

            IRunner runner = new Runner();
            var result = runner.Run<GetDoctorData, GetDoctorDataResult>(db, new GetDoctorData
            {
                v_orgaid = "50e3d44d-9ca2-4fbd-9d5d-d32339b1b113",
            }).ToList();
            Assert.IsNotNull(result);
            System.Console.WriteLine("result ok");
            CollectionAssert.AllItemsAreNotNull(result);
            Assert.IsTrue(result.Count > 0);
            System.Console.WriteLine($"result count {result.Count}");
            System.Console.WriteLine("OracleTest over!");
        }

    }
}