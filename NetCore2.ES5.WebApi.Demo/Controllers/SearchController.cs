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
using NetCore2.ES5.Common.Extensions;

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
                var searchResponse = await x.SearchAsync<EmrRecordDto>(s=>s
                .From(0)
                .Size(10000)
                .Query(q=>{
                    var qu = BuildQueryFromFilter(requestDto) as QueryContainer;
                    //var serialized = Encoding.UTF8.GetString(x.Serializer.Serialize(qu));
                    return qu;
                })
                //.Collapse(c=>c.Field(f=>f.PATIENT_NO))
                );
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
                //var searchResponse = await x.SearchAsync<PatientWithEmrDto>(s=>s
                //                                                                .Query(q => q
                //                                                                   .Nested(n => n
                //                                                                       .Path(p => p.Emr_Record_List)
                //                                                                        .Query(qq => qq
                //                                                                            .Term(tt => tt
                //                                                                                .Field(ff => ff.Emr_Record_List.First().EMR_TYPE_CODE)
                //                                                                                .Value(requestDto.SearchList.First().CONDITION_TYPE)
                //                                                                                )
                //                                                                            && qq
                //                                                                            .MatchPhrase(t => t
                //                                                                                .Field(a => a.Emr_Record_List.First().REC_CONTENT)
                //                                                                                .Query(requestDto.SearchList.First().CONDITION)
                //                                                                                )
                //                                                                            )
                //                                                                        )
                //                      ));
                //var product = searchResponse.Documents;

                var innerHitsResponse = await x.SearchAsync<PatientWithEmrDto>(s => s
                                                                                .Source(false)
                                                                                .From(0)
                                                                                .Size(10000)
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
                                                                                            ).InnerHits(a=>a.Name("emr_record_list"))
                                                                                        )
                                      ));

                List<CPAT_EMR_RECORD> emrList = new List<CPAT_EMR_RECORD>();
                List<CPAT_CHECK_RECORD> checkList = new List<CPAT_CHECK_RECORD>();
                foreach (var hit in innerHitsResponse.Hits)
                {
                    var emr = hit.InnerHits["emr_record_list"].Documents<CPAT_EMR_RECORD>();
                    var check = hit.InnerHits["emr_record_list"].Documents<CPAT_EMR_RECORD>();
                    emrList.AddRange(emr);
                };

                emrList  = emrList.DistinctBy(a=>a.PATIENT_NO).ToList();


                sw.Stop();
                _logger.LogInformation("SearchFromESV8 Query Use {0} Millisecords", sw.ElapsedMilliseconds);
                //return Ok(new { Data = product, TotalRecord = searchResponse.Total });
                return Ok(new { Data = emrList, TotalRecord = emrList.Count });
            }
            catch (Exception e)
            {
                _logger.LogError("SearchFromESV8 >>>>>>>>>"+e.Message);

                throw e;
            }

        }

        [HttpPost]
        public async Task<IActionResult> SearchFromESV9([FromBody]AdviceRequestDto requestDto)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var x = ElasticSearch5Wrapper.GetInstance(_indexName, _es5Options);
                _logger.LogInformation("SearchFromESV9 Query Start ***********");


                //var innerHitsResponse = await x.SearchAsync<PatientWithEmrDto>(s => s
                //                                                                .Source(false)
                //                                                                .From(0)
                //                                                                .Size(10000)
                //                                                                .Query(q => q
                //                                                                   .Nested(n => n
                //                                                                       .Path(p => p.Emr_Record_List)
                //                                                                        .Query(qq => qq
                //                                                                            .Term(tt => tt
                //                                                                                .Field(ff => ff.Emr_Record_List.First().EMR_TYPE_CODE)
                //                                                                                .Value(requestDto.SearchList.First().CONDITION_TYPE)
                //                                                                                )
                //                                                                            && qq
                //                                                                            .MatchPhrase(t => t
                //                                                                                .Field(a => a.Emr_Record_List.First().REC_CONTENT)
                //                                                                                .Query(requestDto.SearchList.First().CONDITION)
                //                                                                                )
                //                                                                            ).InnerHits(a => a.Name("emr_record_list"))
                //                                                                        )
                //                      ));

                var innerHitsResponse = await x.SearchAsync<PatientWithEmrDto>(s => s
                                                                                //.Source(false)
                                                                                .From(0)
                                                                                .Size(10000)
                                                                                .Query(q => {
                                                                                        var query = BuildQueryFromFilter(requestDto) as QueryContainer;
                                                                                        return query;
                                                                                    })
                                                                                );


                var response = await x.SearchAsync<DP_DICT_DETAIL>(s => s
                                                                        .From(0)
                                                                        .Size(10000)
                                                                        .Query(q => q
                                                                            .MatchAll()
                                                                                )
                                                                    );

                var emrTypeList = response.Documents.Where(c => c.DP_CLASS_CODE == "CPAT_EMR_TYPE").ToList();

                //List<CPAT_EMR_RECORD> emrList = new List<CPAT_EMR_RECORD>();
                //List<CPAT_CHECK_RECORD> checkList = new List<CPAT_CHECK_RECORD>();
                List<AdviceQueryResponseDto> responseList = new List<AdviceQueryResponseDto>();
                foreach (var hit in innerHitsResponse.Hits)
                {
                    var emr = hit.InnerHits["emr_record_list"].Documents<CPAT_EMR_RECORD>();

                    var firstEmrDoc = (from e in emr
                                       join o in emrTypeList
                                       on e.EMR_TYPE_CODE equals o.DP_ITEM_CODE
                                       select new
                                       {
                                           PARENT_NAME = hit.Source.PATIENT_NAME,
                                           e.PATIENT_NO,
                                           e.REC_CONTENT,
                                           e.EMR_REC_ID,
                                           e.EMR_TYPE_CODE,
                                           e.EMR_TYPE_NAME,
                                           o.ORDER_NO
                                       }).OrderBy(b=>b.ORDER_NO).FirstOrDefault();
                    var check = hit.InnerHits["check_record_list"].Documents<CPAT_CHECK_RECORD>();
                    //emrList.AddRange(emr);
                    //checkList.AddRange(check);

                    //var ret = new AdviceQueryResponseDto
                    //{
                    //    EmrId = emr.FirstOrDefault()?.EMR_REC_ID.ToString(),
                    //    PatientName = emr.FirstOrDefault()?.PARENT_NAME,
                    //    PatientNO = emr.FirstOrDefault()?.PATIENT_NO,
                    //    CheckType = check.FirstOrDefault()?.CHECK_TYPE,
                    //    EmrType = emr.FirstOrDefault()?.EMR_TYPE_NAME,
                    //    EmrContext = emr.FirstOrDefault()?.REC_CONTENT,
                    //    ReportContext = check.FirstOrDefault()?.REPORT_RESULT
                    //};

                    var ret = new AdviceQueryResponseDto
                    {
                        EmrId = firstEmrDoc?.EMR_REC_ID.ToString(),
                        PatientName = firstEmrDoc?.PARENT_NAME,
                        PatientNO = firstEmrDoc?.PATIENT_NO,
                        CheckType = check.FirstOrDefault()?.CHECK_TYPE,
                        EmrType = firstEmrDoc?.EMR_TYPE_NAME,
                        EmrContext = firstEmrDoc?.REC_CONTENT,
                        ReportContext = check.FirstOrDefault()?.REPORT_RESULT
                    };


                    responseList.Add(ret);
                };

                //emrList = emrList.DistinctBy(a => a.PATIENT_NO).ToList();
                //checkList = checkList.DistinctBy(a => a.PATIENT_NO).ToList();


                sw.Stop();
                _logger.LogInformation("SearchFromESV9 Query Use {0} Millisecords", sw.ElapsedMilliseconds);
                //return Ok(new { Data = product, TotalRecord = searchResponse.Total });
                return Ok(new { Data = responseList, TotalRecord = responseList.Count });
            }
            catch (Exception e)
            {
                _logger.LogError("SearchFromESV9 >>>>>>>>>" + e.Message);

                throw e;
            }

        }

        private IQueryContainer BuildQueryFromFilter(AdviceRequestDto filter)
        {
            QueryContainer patientQuery = null;
            QueryContainer emrTermQuery = null;
            //QueryContainer emrMatchQuery = null;
            QueryContainer checkTermQuery = null;
           // QueryContainer checkMatchQuery = null;
            QueryContainer combineQuery = null;

            if (filter.patientSearchCondition!=null && !string.IsNullOrWhiteSpace(filter.patientSearchCondition.PatientName))
                patientQuery = Query<PatientWithEmrDto>.Term(t => t.Field(f => f.PATIENT_NAME)
                                                                .Value(filter.patientSearchCondition.PatientName));

            if (filter.emrSearchCondition != null && filter.emrSearchCondition.Any())
            {
                foreach (var s in filter.emrSearchCondition)
                {

                    if (s.EmrTypeCode.Trim()=="*" || string.IsNullOrWhiteSpace(s.EmrTypeCode))
                    {
                        emrTermQuery = Query<PatientWithEmrDto>.Nested(n => n
                                                            .Path(pt => pt.Emr_Record_List)
                                                            .Query(q => q
                                                                    .MatchPhrase(m => m
                                                                        .Field(f => f.Emr_Record_List.First().REC_CONTENT)
                                                                        .Query(s.EmrContext))
                                                                ).InnerHits()

                                                            );
                    }
                    else
                    {
                        emrTermQuery = Query<PatientWithEmrDto>.Nested(n => n
                                                            .Path(pt => pt.Emr_Record_List)
                                                            .Query(q => q
                                                                .Term(t => t
                                                                    .Field(f => f.Emr_Record_List.First().EMR_TYPE_CODE)
                                                                    .Value(s.EmrTypeCode)
                                                                    )
                                                                && q.MatchPhrase(m => m
                                                                    .Field(f => f.Emr_Record_List.First().REC_CONTENT)
                                                                    .Query(s.EmrContext))
                                                                ).InnerHits()

                                                            );
                    }

                    //emrMatchQuery = Query<PatientWithEmrDto>.Nested(n => n
                    //                                        .Path(pt => pt.Emr_Record_List)
                    //                                        .Query(q => q
                    //                                            .MatchPhrase(m=>m
                    //                                                .Field(f=>f.Emr_Record_List.First().REC_CONTENT)
                    //                                                .Query(s.EmrContext))
                    //                                            ).InnerHits()

                    //                                        );

                    //combineQuery &= emrTermQuery && emrMatchQuery;
                    combineQuery &= emrTermQuery;


                }
            }

            if (filter.checkSearchCondition != null && filter.checkSearchCondition.Any())
            {
                foreach (var s in filter.checkSearchCondition)
                {
                    checkTermQuery = Query<PatientWithEmrDto>.Nested(n => n
                                                            .Path(pt => pt.Check_Record_List)
                                                            .Query(q => q
                                                                .Term(t => t
                                                                        .Field(f => f.Check_Record_List.First().CHECK_TYPE)
                                                                        .Value(s.CheckType)
                                                                      )
                                                                 && q.MatchPhrase(m => m
                                                                        .Field(f => f.Check_Record_List.First().REPORT_RESULT)
                                                                        .Query(s.ReportResult))
                                                                   ).InnerHits()

                                                            );


                    //checkMatchQuery = Query<PatientWithEmrDto>.Nested(n => n
                    //                                        .Path(pt => pt.Check_Record_List)
                    //                                        .Query(q => q
                    //                                            .MatchPhrase(m => m
                    //                                                .Field(f => f.Check_Record_List.First().REPORT_RESULT)
                    //                                                .Query(s.ReportResult))
                    //                                            )

                    //                                        );

                    //combineQuery &= checkTermQuery && checkMatchQuery;
                    combineQuery &= checkTermQuery;


                }
            }

            return combineQuery;
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