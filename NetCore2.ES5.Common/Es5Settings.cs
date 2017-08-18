using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common
{
    public class Es5Settings
    {
        /// <summary>
        /// es5 uris
        /// </summary>
        public string NodeUrls { get; set; }
        /// <summary>
        /// 副本数
        /// </summary>
        public int NumberOfReplicas { get; set; }
        /// <summary>
        /// 分片数
        /// </summary>
        public int NumberOfShards { get; set; }
    }
}
