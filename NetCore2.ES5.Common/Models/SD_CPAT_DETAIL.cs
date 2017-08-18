using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common.Models
{
    [ElasticsearchType(Name = "sd_cpat_detail", IdProperty = "DETAIL_ID")]
    public class SD_CPAT_DETAIL
    {
        [JsonProperty("detail_id")]
        public int DETAIL_ID { get; set; }
        [JsonProperty("sd_cpat_no")]
        [Keyword]
        public string SD_CPAT_NO { get; set; }
        [JsonProperty("patient_no")]
        [Keyword]
        public string PATIENT_NO { get; set; }
        [JsonProperty("in_flag")]
        [Keyword]
        public string IN_FLAG { get; set; }
        [JsonProperty("base_flag")]
        public int BASE_FLAG { get; set; }
        [JsonProperty("patient_id")]
        [Keyword]
        public string PATIENT_ID { get; set; }
        [JsonProperty("upd_date")]
        public DateTime? UPD_DATE { get; set; }
    }
}
