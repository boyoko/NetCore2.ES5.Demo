using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common.Models
{
    [ElasticsearchType(Name = "cpat_check_record", IdProperty = "CHECK_ID")]
    public class CPAT_CHECK_RECORD
    {
        [JsonProperty("check_id")]
        public int CHECK_ID { get; set; }

        [JsonProperty("patient_no")]
        [Keyword]
        public string PATIENT_NO { get; set; }


        [JsonProperty("check_type")]
        [Keyword]
        public string CHECK_TYPE { get; set; }

        [JsonProperty("check_item_code")]
        [Keyword]
        public string CHECK_ITEM_CODE { get; set; }

        [JsonProperty("check_item_name")]
        [Keyword]
        public string CHECK_ITEM_NAME { get; set; }

        [JsonProperty("exec_dept_code")]
        [Keyword]
        public string EXEC_DEPT_CODE { get; set; }

        [JsonProperty("exec_dept_name")]
        [Keyword]
        public string EXEC_DEPT_NAME { get; set; }

        [JsonProperty("report_objective")]
        [Text(Analyzer = "ik_smart", SearchAnalyzer = "ik_smart")]
        public string REPORT_OBJECTIVE { get; set; }

        [JsonProperty("report_subjective")]
        [Text(Analyzer = "ik_smart", SearchAnalyzer = "ik_smart")]
        public string REPORT_SUBJECTIVE { get; set; }

        [JsonProperty("report_result")]
        [Text(Analyzer = "ik_smart", SearchAnalyzer = "ik_smart")]
        public string REPORT_RESULT { get; set; }

    }
}
