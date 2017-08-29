using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common.Models
{
    public class AdviceRequestDto
    {
        public PatientSearchCondition patientSearchCondition { get; set; }
        public IList<EmrSearchCondition> emrSearchCondition { get; set; }
        public IList<CheckSearchCondition> checkSearchCondition { get; set; }
    }

    public class PatientSearchCondition
    {
        /// <summary>
        /// 患者姓名，患者信息检索条件
        /// </summary>
        public string PatientName { get; set; }
    }

    public class CheckSearchCondition
    {
        /// <summary>
        /// 检查项 如：放射、超声
        /// </summary>
        public string CheckType { get; set; }
        /// <summary>
        /// 检查结果,进行全文检索字段
        /// </summary>
        public string ReportResult { get; set; }
    }
    public class EmrSearchCondition
    {
        /// <summary>
        /// emr 类型编码 如：入院记录：21601
        /// </summary>
        public string EmrTypeCode { get; set; }
        /// <summary>
        /// emr 内容 ，包含主诉，等信息
        /// </summary>
        public string EmrContext { get; set; }
    }
}
