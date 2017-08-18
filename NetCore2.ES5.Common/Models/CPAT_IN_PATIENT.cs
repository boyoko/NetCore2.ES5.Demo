using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common.Models
{
    [ElasticsearchType(Name = "cpat_in_patient", IdProperty = "PATIENT_NO")]
    public partial class CPAT_IN_PATIENT
    {
        [JsonProperty("patient_no")]
        [Keyword]
        public string PATIENT_NO { get; set; }
        [JsonProperty("patient_id")]
        [Keyword]
        public string PATIENT_ID { get; set; }
        [JsonProperty("in_times")]
        public Nullable<int> IN_TIMES { get; set; }
        [JsonProperty("case_no")]
        [Keyword]
        public string CASE_NO { get; set; }
        [JsonProperty("card_no")]
        [Keyword]
        public string CARD_NO { get; set; }
        [JsonProperty("patient_name")]
        [Keyword]
        public string PATIENT_NAME { get; set; }
        [JsonProperty("id_no")]
        [Keyword]
        public string ID_NO { get; set; }
        [JsonProperty("birth_date")]
        public Nullable<System.DateTime> BIRTH_DATE { get; set; }
        [JsonProperty("gender_code")]
        public Nullable<int> GENDER_CODE { get; set; }
        [JsonProperty("country_name")]
        [Keyword]
        public string COUNTRY_NAME { get; set; }
        [JsonProperty("nation_name")]
        [Keyword]
        public string NATION_NAME { get; set; }
        [JsonProperty("mari_stat_code")]
        public Nullable<int> MARI_STAT_CODE { get; set; }
        [JsonProperty("charge_type_code")]
        [Keyword]
        public string CHARGE_TYPE_CODE { get; set; }
        [JsonProperty("change_type_name")]
        [Keyword]
        public string CHARGE_TYPE_NAME { get; set; }
        [JsonProperty("in_date")]
        public System.DateTime IN_DATE { get; set; }
        [JsonProperty("out_date")]
        public System.DateTime OUT_DATE { get; set; }
        [JsonProperty("in_dept_code")]
        [Keyword]
        public string IN_DEPT_CODE { get; set; }
        [JsonProperty("in_dept_name")]
        public string IN_DEPT_NAME { get; set; }
        [JsonProperty("area_code")]
        [Keyword]
        public string AREA_CODE { get; set; }
        [JsonProperty("area_name")]
        public string AREA_NAME { get; set; }
        [JsonProperty("out_dept_code")]
        [Keyword]
        public string OUT_DEPT_CODE { get; set; }
        [JsonProperty("out_dept_name")]
        public string OUT_DEPT_NAME { get; set; }
        [JsonProperty("in_dept_date")]
        public Nullable<System.DateTime> IN_DEPT_DATE { get; set; }
        [JsonProperty("out_dept_date")]
        public Nullable<System.DateTime> OUT_DEPT_DATE { get; set; }
        [JsonProperty("doctor_code")]
        [Keyword]
        public string DOCTOR_CODE { get; set; }
        [JsonProperty("doctor_name")]
        [Keyword]
        public string DOCTOR_NAME { get; set; }
        [JsonProperty("in_source_code")]
        public Nullable<int> IN_SOURCE_CODE { get; set; }
        [JsonProperty("in_status_code")]
        public Nullable<int> IN_STATUS_CODE { get; set; }
        [JsonProperty("out_way_code")]
        public Nullable<int> OUT_WAY_CODE { get; set; }
        [JsonProperty("out_status_code")]
        public Nullable<int> OUT_STATUS_CODE { get; set; }
        [JsonProperty("in_diag_code")]
        [Keyword]
        public string IN_DIAG_CODE { get; set; }
        [JsonProperty("in_diag_name")]
        [Keyword]
        public string IN_DIAG_NAME { get; set; }
        [JsonProperty("out_diag_code")]
        [Keyword]
        public string OUT_DIAG_CODE { get; set; }
        [JsonProperty("out_diag_name")]
        [Keyword]
        public string OUT_DIAG_NAME { get; set; }
        [JsonProperty("upd_date")]
        public Nullable<System.DateTime> UPD_DATE { get; set; }
        [JsonProperty("emr_record_list")]
        [Nested]
        public List<CPAT_EMR_RECORD> Emr_Record_List { get; set; }
    }
}
