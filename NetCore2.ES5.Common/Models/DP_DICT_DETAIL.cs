using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common.Models
{
    [ElasticsearchType(Name = "dp_dict_detail")]
    public class DP_DICT_DETAIL
    {
        [JsonProperty("dp_class_code")]
        [Keyword]
        public string DP_CLASS_CODE { get; set; }
        [JsonProperty("dp_item_code")]
        [Keyword]
        public string DP_ITEM_CODE { get; set; }
        [JsonProperty("dp_item_name")]
        [Keyword]
        public string DP_ITEM_NAME { get; set; }
        [JsonProperty("order_no")]
        public int? ORDER_NO { get; set; }
        [JsonProperty("memo")]
        [Keyword]
        public string MEMO { get; set; }
        [JsonProperty("attr")]
        [Keyword]
        public string ATTR { get; set; }

    }
}
