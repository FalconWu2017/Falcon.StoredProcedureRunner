using System;
using System.ComponentModel.DataAnnotations;
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
            Console.WriteLine("测试调用存储过程，获取返回值");
            var r = db.RunSp1(new Database.sp.Sp1());
            Console.WriteLine("返回记录数{0},应该为2",r.Count());
            var fir = r.First();
            Console.WriteLine("Id为{0},应该为1",fir.Id);
            Console.WriteLine("Jia{0},应该为3",fir.Jia);
            Console.WriteLine("Jian{0},应该为-1",fir.Jian);
            Console.WriteLine("Chen{0},应该为2",fir.Chen);
            Console.WriteLine("Chu{0},应该为0.5",fir.Chu);
            Console.WriteLine("s{0},应该为abc1",fir.s);

            Console.WriteLine("测试无返回值调用");
            var r1 = db.RunSp1No(new Database.sp.Sp1());
            Console.WriteLine("返回结果{0}",r1);
        }
    }
}
