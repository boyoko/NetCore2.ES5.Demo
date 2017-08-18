using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCore2.ES5.Common;
using NetCore2.ES5.Common.Models;
using System.Diagnostics;
using Nest;
using System.Text;

namespace NetCore2.ES5.WebApi.Demo.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SearchController : Controller
    {
        private ILogger<SearchController> _logger;
        private IOptions<Es5Settings> _es5Options;
        private static readonly string _indexName = "record-v2";

        public SearchController(ILogger<SearchController> logger, IOptions<Es5Settings> es5Options)
        {
            _logger = logger;
            _es5Options = es5Options;
        }

        [HttpPost]
        public async Task<IActionResult> SearchFromESV001([FromBody]SearchPatientListRequestDto requestDto)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var x = ElasticSearch5Wrapper.GetInstance(_indexName, _es5Options);
                _logger.LogInformation("SearchFromESV001------------- Query Start ******");
                var searchResponse = await x.SearchAsync<EmrRecordDto>(s=>s.Query(q=>{
                    var qu = BuildQueryFromFilter(requestDto) as QueryContainer;
                    //var serialized = Encoding.UTF8.GetString(x.Serializer.Serialize(qu));
                    return qu;
                }).Collapse(c=>c.Field(f=>f.PATIENT_NO)));
                var product = searchResponse.Documents;
                sw.Stop();
                _logger.LogInformation("SearchFromESV001------------- Query Use {0} Millisecords", sw.ElapsedMilliseconds);
                return Ok(new { Data = product, TotalRecord = searchResponse.Total });
            }
            catch(Exception e)
            {
                _logger.LogError("SearchFromESV001-------------- >>>>>>>>"+e.Message);

                throw e;
            }
            
        }


        Func<MatchPhraseQueryDescriptor<EmrRecordDto>, IMatchPhraseQuery> selector = q =>
        {
            return (IMatchPhraseQuery)q.Query(@"{
                      ""query"": {
                        ""match_phrase"": {
                                    ""rec_content"": ""巨结肠""
                        }
                            }
                        }");
        };


        private IQueryContainer BuildQueryFromFilter(SearchPatientListRequestDto filter)
        {
            QueryContainer termQuery = null;
            QueryContainer matchQuery = null;
            QueryContainer combineQuery = null;

            QueryContainer sdCodeTermQuery = null;
            QueryContainer baseFlagTermQuery = null;
            if (!string.IsNullOrWhiteSpace(filter.SdCode))
                sdCodeTermQuery = Query<EmrRecordDto>.Term(t => t.Field(f => f.SD_CODE)
                                                                .Value(filter.SdCode.ToLower()));

            baseFlagTermQuery = Query<EmrRecordDto>.Term(t => t.Field(f => f.BASE_FLAG)
                                                             .Value(1));

            if (filter.SearchList != null && filter.SearchList.Any())
            {
               foreach(var s in filter.SearchList)
               {
                   termQuery = Query<EmrRecordDto>.Term(t=>t.Field(f=>f.EMR_TYPE_CODE)
                                                                .Value(s.CONDITION_TYPE));
                    matchQuery = Query<EmrRecordDto>.MatchPhrase(m => m.Field(f => f.REC_CONTENT)
                                                                        .Query(s.CONDITION)
                    );

                    //matchQuery = Query<EmrRecordDto>.MatchPhrase(selector);
                    if(s.CONDITION_TYPE.Trim()=="*")
                    {
                        combineQuery |= matchQuery;
                    }else{
                        combineQuery |= termQuery && matchQuery;
                    }
                    

               }
            }

           
            return (sdCodeTermQuery!=null ? 
                        (sdCodeTermQuery && baseFlagTermQuery && combineQuery) :
                        (baseFlagTermQuery &&combineQuery)
                    );
        }

        

        // http://www.rubencanton.com/blog/2017/05/performing-queries-in-net-with-nest.html
        [HttpPost]
        public async Task<IActionResult> SearchFromESV6([FromBody]SearchPatientListRequestDto requestDto)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var x = ElasticSearch5Wrapper.GetInstance(_indexName, _es5Options);
                _logger.LogInformation("SearchFromESV6 Query Start ******");
                var searchResponse = await x.SearchAsync<EmrRecordDto>(CreateQueryFromFilter(requestDto));
                var product = searchResponse.Documents;
                sw.Stop();
                _logger.LogInformation("SearchFromESV6 Query Use {0} Millisecords", sw.ElapsedMilliseconds);
                return Ok(new { Data = product, TotalRecord = searchResponse.Total });
            }
            catch(Exception e)
            {
                _logger.LogError("SearchFromESV6 >>>>>>>>"+e.Message);

                throw e;
            }
            
        }


        [HttpPost]
        public async Task<IActionResult> SearchFromESV8([FromBody]SearchPatientListRequestDto requestDto)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var x = ElasticSearch5Wrapper.GetInstance(_indexName, _es5Options);
                _logger.LogInformation("SearchFromESV8 Query Start ***********");
                var searchResponse = await x.SearchAsync<PatientWithEmrDto>(s=>s
                                                                                .Query(q => q
                                                                                   .Nested(n => n
                                                                                       .Path(p => p.Emr_Record_List)
                                                                                        .Query(qq => qq
                                                                                            .Term(tt => tt
                                                                                                .Field(ff => ff.Emr_Record_List.First().EMR_TYPE_CODE)
                                                                                                .Value(requestDto.SearchList.First().CONDITION_TYPE)
                                                                                                )
                                                                                            && qq
                                                                                            .MatchPhrase(t => t
                                                                                                .Field(a => a.Emr_Record_List.First().REC_CONTENT)
                                                                                                .Query(requestDto.SearchList.First().CONDITION)
                                                                                                )
                                                                                            )
                                                                                        )
                                      ));
                var product = searchResponse.Documents;
                sw.Stop();
                _logger.LogInformation("SearchFromESV8 Query Use {0} Millisecords", sw.ElapsedMilliseconds);
                return Ok(new { Data = product, TotalRecord = searchResponse.Total });
            }
            catch (Exception e)
            {
                _logger.LogError("SearchFromESV8 >>>>>>>>>"+e.Message);

                throw e;
            }

        }


        private Func<SearchDescriptor<EmrRecordDto>, ISearchRequest> CreateQueryFromFilter(SearchPatientListRequestDto filter)
        {

            Func<SearchDescriptor<EmrRecordDto>, ISearchRequest> result;
            var mustQuerys = new List<Func<QueryContainerDescriptor<EmrRecordDto>, QueryContainer>>();

            if (filter.SearchList != null && filter.SearchList.Any())
            {
                foreach (var a in filter.SearchList)
                {
                    if (a.CONDITION_TYPE == "*")
                    {
                        mustQuerys.Add(t => t
                                          .MatchPhrase(f => f
                                                .Field(fd => fd.REC_CONTENT)
                                                .Query(a.CONDITION)
                                            )
                                   );
                    }
                    else
                    {
                        mustQuerys.Add(t => t
                                         .Term(f => f.EMR_TYPE_CODE, a.CONDITION_TYPE)
                                          &&
                                          t.MatchPhrase(f => f
                                                .Field(fd => fd.REC_CONTENT)
                                                .Query(a.CONDITION)
                                            )
                                   );
                    }
                }
            }

            Func<QueryContainerDescriptor<EmrRecordDto>, QueryContainer> condition =
                o => o.Bool(b => b.Should(
                                mustQuerys.ToArray()
                 ));
            result = o => o.Query(condition)
                            .From(0).Size(10000);
            return result;
        }
    }
}