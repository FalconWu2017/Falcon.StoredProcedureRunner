using Falcon.StoredProcedureRunner;
using Falcon.StoredProcedureRunner.Attributes;
using System.Data;

namespace Falcon.StoredProcedureRunnerTests
{
    internal class GetEmployee
    {
        public string v_orgaid { get; set; }

        [FalconSPPrarmDirection(ParameterDirection.Output)]
        [FalconSPPrarmType(FalconSPDbType.OracleRefCursor)]
        public object v_data { get; set; }
    }
    internal class GetEmployeeResult
    {
        public string empcode { get; set; }
        public string empname { get; set; }
        public string empactive { get; set; }
    }


    /// <summary>
    /// 体检接口 挂号查询
    /// </summary>
    public class Tjjk_Tjgh
    {
        /// <summary>
        /// 医疗机构代码
        /// </summary>
        public string v_yljgdm { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string v_sfz { get; set; }
        /// <summary>
        /// 挂号日期 格式yyyyMMdd
        /// </summary>
        public string v_ghrq { get; set; }

    }
    /// <summary>
    /// 挂号查询结果
    /// </summary>
    [FalconSPTempTable("TT_Tjjk_Tjgh")]
    public class Tjjk_TjghResult
    {
        /// <summary>
        /// 卡号
        /// </summary>
        public string kh { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string xm { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string sfzh { get; set; }
        /// <summary>
        /// 科室编号
        /// </summary>
        public string ksbh { get; set; }
        /// <summary>
        /// 科室名称
        /// </summary>
        public string ksmc { get; set; }
        /// <summary>
        /// 区县标志 本区 外区
        /// </summary>
        public string qxbz { get; set; }
        /// <summary>
        /// 挂号编号
        /// </summary>
        public string ghbh { get; set; }
        /// <summary>
        /// 挂号状态.已挂号 未挂号 已退费
        /// </summary>
        public string state { get; set; }
    }


    internal class GetDoctorData
    {
        public string v_orgaid { get; set; }

        [FalconSPPrarmDirection(ParameterDirection.Output)]
        [FalconSPPrarmType(FalconSPDbType.OracleRefCursor)]
        public object v_data { get; set; }
    }
    internal class GetDoctorDataResult
    {
        public string Name_ { get; set; }
        public string Dept_Name { get; set; }
        public string Consulting_Room { get; set; }
    }
}
