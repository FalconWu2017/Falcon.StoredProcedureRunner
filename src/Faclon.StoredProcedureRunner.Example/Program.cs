using System;
using System.Linq;
using Faclon.StoredProcedureRunner.Example.Database;
using Falcon.StoredProcedureRunner;
using Microsoft.EntityFrameworkCore;

namespace Faclon.StoredProcedureRunner.Example
{
    class Program
    {
        static void Main(string[] args) {
            IRunner runner = new Runner();
            var conStr = "Server =.\\SQLSERVER2008R2;Database = test;User ID = sa;Password = 111";
            var opb = new DbContextOptionsBuilder();
            opb.UseSqlServer(conStr);
            var db = new MyDb(opb.Options,runner);
            var r = db.RunSp1(new Database.sp.Sp1());
            Console.WriteLine("返回记录数{0},应该为2",r.Count());
            var fir = r.First();
            Console.WriteLine("Id为{0},应该为1",fir.Id);
            Console.WriteLine("Jia{0},应该为3",fir.Jia);
            Console.WriteLine("Jian{0},应该为-1",fir.Jian);
            Console.WriteLine("Chen{0},应该为2",fir.Chen);
            Console.WriteLine("Chu{0},应该为0.5",fir.Chu);
        }
    }
}
