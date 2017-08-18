using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common.Models
{
    [ElasticsearchType(Name = "sd_cpats", IdProperty = "SD_CPAT_NO")]
    public class SD_CPATS
    {
        [JsonProperty("sd_cpat_no")]
        [Keyword]
        public string SD_CPAT_NO { get; set; }
        [JsonProperty("sd_code")]
        [Keyword]
        public string SD_CODE { get; set; }
        [JsonProperty("sd_cpat_date")]
        public DateTime SD_CPAT_DATE { get; set; }
        [JsonProperty("patient_id")]
        [Keyword]
        public string PATIENT_ID { get; set; }
        [JsonProperty("upd_date")]
        public DateTime? UPD_DATE { get; set; }
    }
}
