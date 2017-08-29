using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common.Models
{
    [ElasticsearchType(Name = "patient_with_emr_dto", IdProperty = "PATIENT_NO")]
    public class PatientWithEmrDto
    {
        [JsonProperty("patient_no")]
        [Keyword]
        public string PATIENT_NO { get; set; }
        [JsonProperty("patient_id")]
        [Keyword]
        public string PATIENT_ID { get; set; }
        [JsonProperty("patient_name1")]
        [Keyword]
        public string PATIENT_NAME { get; set; }
        [JsonProperty("birth_date")]
        public Nullable<System.DateTime> BIRTH_DATE { get; set; }
        [JsonProperty("gender_code_1")]
        public Nullable<int> GENDER_CODE { get; set; }
        [JsonProperty("country_name_1")]
        [Keyword]
        public string COUNTRY_NAME { get; set; }

        [JsonProperty("emr_record_list")]
        [Nested]
        public List<CPAT_EMR_RECORD> Emr_Record_List { get; set; }

        [JsonProperty("check_record_list")]
        [Nested]
        public List<CPAT_CHECK_RECORD> Check_Record_List { get; set; }

    }
}
