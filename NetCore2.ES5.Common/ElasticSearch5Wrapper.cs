using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common
{
    public class ElasticSearch5Wrapper
    {
        private static readonly object objLock = new object();
        private static ElasticClient _client;
        private static Es5Settings _es5Settings;

        public ElasticSearch5Wrapper(IOptions<Es5Settings> es5Settings)
        {
            _es5Settings = es5Settings.Value;
        }

        private static List<Uri> GetUris(IOptions<Es5Settings> options)
        {
            if (_es5Settings == null)
                _es5Settings = options.Value;
            var urls = _es5Settings.NodeUrls;
            var xx = urls.Split(',');
            List<Uri> uris = new List<Uri>();
            if (xx.Length > 0)
            {
                foreach (var x in xx)
                {
                    uris.Add(new Uri(x));
                }
            }
            else
            {
                uris.Add(new Uri(urls));
            }
            return uris;
        }

        public static IIndexState IndexStata
        {
            get { return GetIndexState(); }
            private set { }
        }
        public static ElasticClient GetInstance(IOptions<Es5Settings> options)
        {
            var connectionPool = new SniffingConnectionPool(GetUris(options));
            var settings = new ConnectionSettings(connectionPool);
            if (_client == null)
            {
                lock (objLock)
                {
                    if (_client == null)
                    {
                        _client = new ElasticClient(settings);
                    }
                }
            }
            return _client;
        }
        public static ElasticClient GetInstance(string defaultIndex, IOptions<Es5Settings> options)
        {
            var connectionPool = new SniffingConnectionPool(GetUris(options));

            var settings = new ConnectionSettings(connectionPool)
                .DefaultIndex(defaultIndex);
            //使用单利，节约内存开销
            if (_client == null)
            {
                lock (objLock)
                {
                    if (_client == null)
                        _client = new ElasticClient(settings);
                }
            }
            return _client;

        }
        private static IIndexState GetIndexState()
        {
            var x = new IndexState()
            {
                Settings = GetIndexSettings()

            };
            return x;
        }
        private static IIndexSettings GetIndexSettings()
        {
            var x = new IndexSettings { NumberOfReplicas = 5, NumberOfShards = 5 };
            return x;
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public static IDeleteIndexResponse DeleteIndex(string indexName)
        {
            return _client.DeleteIndex(indexName);
        }

    }
}
