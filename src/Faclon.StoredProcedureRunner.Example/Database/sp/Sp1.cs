using System;
using System.Collections.Generic;
using System.Text;
using Falcon.StoredProcedureRunner;

namespace Faclon.StoredProcedureRunner.Example.Database.sp
{
    /// <summary>
    /// 测试用存储过程
    /// </summary>
    [FalconSPReturnType(typeof(Sp1_Result))]
    [FalconSPProcuderName("TestSp1")]
    public class Sp1
    {
        /// <summary>
        /// 整数1
        /// </summary>
        public int P1 { get; set; } = 1;
        /// <summary>
        /// 整数2
        /// </summary>
        public int P2 { get; set; } = 2;
    }
    /// <summary>
    /// 存储过程执行结果
    /// </summary>
    public class Sp1_Result
    {
        /// <summary>
        /// 结果行号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 求和
        /// </summary>
        public int Jia { get; set; }
        /// <summary>
        /// 求差
        /// </summary>
        public int Jian { get; set; }
        /// <summary>
        /// 求乘积
        /// </summary>
        public int Chen { get; set; }
        /// <summary>
        /// 求除法
        /// </summary>
        public double Chu { get; set; }
    }
}
