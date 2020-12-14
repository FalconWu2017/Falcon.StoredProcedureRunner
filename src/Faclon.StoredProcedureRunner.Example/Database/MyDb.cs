using System;
using System.Collections.Generic;
using System.Text;
using Faclon.StoredProcedureRunner.Example.Database.sp;
using Microsoft.EntityFrameworkCore;
using Falcon.StoredProcedureRunner;

namespace Faclon.StoredProcedureRunner.Example.Database
{
    public class MyDb:DbContext
    {
        public IRunner SpRunner { get; set; }

        public MyDb(DbContextOptions options,IRunner spRunner) : base(options) {
            this.SpRunner = spRunner;
        }

        public IEnumerable<Sp1_Result> RunSp1(Sp1 data) {
            return this.SpRunner.Run<Sp1,Sp1_Result>(this,data);
        }
    }
}
