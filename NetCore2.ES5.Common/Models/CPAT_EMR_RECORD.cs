using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common.Models
{
    [ElasticsearchType(Name = "cpat_emr_record", IdProperty = "EMR_REC_ID")]
    public partial class CPAT_EMR_RECORD
    {
        [JsonProperty("emr_rec_id")]
        [Keyword]
        public int EMR_REC_ID { get; set; }
        [JsonProperty("patient_id")]
        [Keyword]
        public string PATIENT_ID { get; set; }
        [JsonProperty("patient_no")]
        [Keyword]
        public string PATIENT_NO { get; set; }
        [JsonProperty("emr_type_code")]
        [Keyword]
        public string EMR_TYPE_CODE { get; set; }
        [JsonProperty("emr_type_name")]
        [Keyword]
        public string EMR_TYPE_NAME { get; set; }
        [JsonProperty("lgcy_type_code")]
        [Keyword]
        public string LGCY_TYPE_CODE { get; set; }
        [JsonProperty("lgcy_type_name")]
        [Keyword]
        public string LGCY_TYPE_NAME { get; set; }
        [JsonProperty("lgcy_sub_type_code")]
        [Keyword]
        public string LGCY_SUB_TYPE_CODE { get; set; }
        [JsonProperty("lgcy_sub_type_name")]
        [Keyword]
        public string LGCY_SUB_TYPE_NAME { get; set; }
        [JsonProperty("rec_content")]
        //[Text(Analyzer = "ik_max_word", SearchAnalyzer = "ik_max_word")]
        [Text(Analyzer = "ik_smart", SearchAnalyzer = "ik_smart")]
        public string REC_CONTENT { get; set; }
        [JsonProperty("rec_content_fm")]
        [Keyword]
        public string REC_CONTENT_FM { get; set; }
        [JsonProperty("rec_type")]
        public Nullable<int> REC_TYPE { get; set; }
        [JsonProperty("creator_code")]
        [Keyword]
        public string CREATOR_CODE { get; set; }
        [JsonProperty("creator_name")]
        [Keyword]
        public string CREATOR_NAME { get; set; }
        [JsonProperty("create_date")]
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        [JsonProperty("rec_psn_code")]
        [Keyword]
        public string REC_PSN_CODE { get; set; }
        [JsonProperty("rec_psn_name")]
        [Keyword]
        public string REC_PSN_NAME { get; set; }
        [JsonProperty("rec_date")]
        public Nullable<System.DateTime> REC_DATE { get; set; }
        [JsonProperty("rec_dept_code")]
        [Keyword]
        public string REC_DEPT_CODE { get; set; }
        [JsonProperty("rec_dept_name")]
        [Keyword]
        public string REC_DEPT_NAME { get; set; }
        [JsonProperty("parent_code")]
        [Keyword]
        public string PARENT_CODE { get; set; }
        [JsonProperty("parent_name")]
        [Keyword]
        public string PARENT_NAME { get; set; }
        [JsonProperty("director_code")]
        [Keyword]
        public string DIRECTOR_CODE { get; set; }
        [JsonProperty("director_name")]
        [Keyword]
        public string DIRECTOR_NAME { get; set; }
        [JsonProperty("upd_date")]
        public Nullable<System.DateTime> UPD_DATE { get; set; }

    }
}
