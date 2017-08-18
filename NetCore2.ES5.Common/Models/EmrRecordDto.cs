using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common.Models
{
    [ElasticsearchType(Name = "emr_record_dto")]
    public class EmrRecordDto
    {
        //---------------------------电子病历表[CPAT_EMR_RECORD]信息---------------------------------//
        //[JsonProperty("emr_rec_id")]
        //[Keyword]
        //public int EMR_REC_ID { get; set; }
        [JsonProperty("patient_id")]
        [Keyword]
        public string PATIENT_ID { get; set; }
        [JsonProperty("patient_no")]
        [Keyword]
        public string PATIENT_NO { get; set; }
        [JsonProperty("emr_type_code")]
        [Keyword]
        public string EMR_TYPE_CODE { get; set; }
        //[JsonProperty("emr_type_name")]
        //[Keyword]
        //public string EMR_TYPE_NAME { get; set; }
        [JsonProperty("rec_content")]
        [Text(Analyzer = "ik_smart", SearchAnalyzer = "ik_smart")]
        public string REC_CONTENT { get; set; }

        /// <summary>
        /// [CPAT_EMR_RECORD].EMR_TYPE_NAME
        /// </summary>
        [JsonProperty("condition_name")]
        [Keyword]
        public string CONDITION_NAME { get; set; }

        //----------------主表[CPAT_IN_PATIENT]信息--------------------------------//
        [JsonProperty("patient_name")]
        [Keyword]
        public string PATIENT_NAME { get; set; }
        [JsonProperty("birth_date")]
        public Nullable<System.DateTime> BIRTH_DATE { get; set; }
        [JsonProperty("gender_code")]
        public Nullable<int> GENDER_CODE { get; set; }
        [JsonProperty("country_name")]
        [Keyword]
        public string COUNTRY_NAME { get; set; }
        [JsonProperty("in_date")]
        public DateTime IN_DATE { get; set; }
        [JsonProperty("out_date")]
        public DateTime OUT_DATE { get; set; }
        [JsonProperty("out_dept_name")]
        [Keyword]
        public string OUT_DEPT_NAME { get; set; }


        //-------------------------[SD_CPAT_DETAIL]信息-----------------------------------//
        [JsonProperty("sd_cpat_no")]
        [Keyword]
        public string SD_CPAT_NO { get; set; }
        [JsonProperty("base_flag")]
        public int BASE_FLAG { get; set; }
        //-------------------------[SD_CPATS]信息-----------------------------------//
        [JsonProperty("sd_code")]
        [Keyword]
        public string SD_CODE { get; set; }
    }
}
