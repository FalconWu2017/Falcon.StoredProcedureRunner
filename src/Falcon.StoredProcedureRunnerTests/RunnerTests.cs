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
        [TestMethod()]
        public void RunTest()
        {
            OracleTest();
            SqlserverTest();
        }

        public void SqlserverTest()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("AppSettings.json");
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
            CollectionAssert.AllItemsAreNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        public void OracleTest()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("AppSettings.json");
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
            CollectionAssert.AllItemsAreNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
    }
}