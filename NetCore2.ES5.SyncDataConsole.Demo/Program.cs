using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nest;
using NetCore2.ES5.Common;
using NetCore2.ES5.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NetCore2.ES5.SyncDataConsole.Demo
{
    class Program
    {
        private static ElasticClient _esClient = null;
        private static readonly string indexName = "record-v2";
        public static IConfigurationRoot Configuration { get; set; }
        private static DbContextOptions<NetCoreDbContext> options;
        private static IOptions<Es5Settings> op;
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // 设置控制台编码
            try
            {
                System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

                Configuration = builder.Build();

                var optionsBuilder = new DbContextOptionsBuilder<NetCoreDbContext>();
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection"));

                options = optionsBuilder.Options;

                var cc = Configuration.GetSection("Es5Settings");
                var aa = cc.GetChildren();

                op = Options.Create<Es5Settings>(new Es5Settings
                {
                    NodeUrls = Configuration.GetSection("Es5Settings:NodeUrls").Value,
                    NumberOfReplicas = Convert.ToInt16(aa.FirstOrDefault(a => a.Key == "NumberOfReplicas").Value),
                    NumberOfShards = Convert.ToInt16(aa.FirstOrDefault(a => a.Key == "NumberOfShards").Value),
                });

                //同步数据
                SetData2();

                Console.WriteLine("数据同步成功！");
                Console.Read();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
           
        }

        static void SetData()
        {
            try
            {
                _esClient = ElasticSearch5Wrapper.GetInstance(op);

                if (!_esClient.IndexExists(indexName).Exists)
                {
                    var descriptor = new CreateIndexDescriptor(indexName)
                                                        .InitializeUsing(ElasticSearch5Wrapper.IndexStata)
                                                        .Mappings(ms => ms
                                                        .Map<EmrRecordDto>(m => m
                                                            .AutoMap()
                                                        )
                                                        );
                    ICreateIndexResponse response = _esClient.CreateIndex(indexName, c => c = descriptor);

                    if (response.IsValid)
                    {
                        string msg = string.Format("索引" + indexName + "创建成功！");
                        Console.WriteLine(msg);
                    }
                    else
                    {
                        string msg = string.Format("索引" + indexName + "创建失败！");
                        Console.WriteLine(msg);
                    }

                }
                else
                {
                    Console.WriteLine("索引已经存在！");
                }

                var sw = new Stopwatch();
                sw.Start();

                using (var db = new NetCoreDbContext(options))
                {
                    var patientList = (from emr in db.CPAT_EMR_RECORD.AsNoTracking()
                                       join p in db.CPAT_IN_PATIENT.AsNoTracking()
                                       on emr.PATIENT_NO equals p.PATIENT_NO
                                       join d in db.SD_CPAT_DETAIL.AsNoTracking()
                                       on emr.PATIENT_NO equals d.PATIENT_NO
                                       join c in db.SD_CPATS.AsNoTracking()
                                       on d.SD_CPAT_NO equals c.SD_CPAT_NO
                                       select new EmrRecordDto
                                       {
                                           PATIENT_NO = p.PATIENT_NO,
                                           PATIENT_ID = p.PATIENT_ID,
                                           //EMR_REC_ID = emr.EMR_REC_ID,
                                           EMR_TYPE_CODE = emr.EMR_TYPE_CODE,
                                           //EMR_TYPE_NAME = emr.EMR_TYPE_NAME,
                                           REC_CONTENT = emr.REC_CONTENT,
                                           CONDITION_NAME = emr.EMR_TYPE_NAME,

                                           PATIENT_NAME = p.PATIENT_NAME,
                                           BIRTH_DATE = p.BIRTH_DATE,
                                           GENDER_CODE = p.GENDER_CODE,
                                           COUNTRY_NAME = p.COUNTRY_NAME,
                                           IN_DATE = p.IN_DATE,
                                           OUT_DATE = p.OUT_DATE,
                                           OUT_DEPT_NAME = p.OUT_DEPT_NAME,

                                           SD_CPAT_NO = d.SD_CPAT_NO,
                                           BASE_FLAG = d.BASE_FLAG,

                                           SD_CODE = c.SD_CODE
                                       }
                                   )
                                   .ToList();

                    sw.Stop();
                    Console.WriteLine("Get All message take {0} Milliseconds", sw.ElapsedMilliseconds);

                    sw.Restart();


                    var len = Math.Ceiling(Convert.ToDecimal(patientList.Count() * 0.01));

                    for(var i = 0; i < len; i++)
                    {
                        BulkDescriptor descriptor = new BulkDescriptor();
                        var tmpList = patientList.Skip(i * 100).Take(100);
                        foreach (var doc in tmpList)
                        {
                            descriptor.Index<EmrRecordDto>(k => k
                                .Index(indexName)
                                .Type("emr_record_dto")
                                //.Id(doc.EMR_REC_ID)
                                .Document(doc));
                        }

                        _esClient.Bulk(descriptor);
                    }

                    

                    sw.Stop();

                    Console.WriteLine("Sync All Data to ES take {0} Milliseconds", sw.ElapsedMilliseconds);
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            
        }

        static void SetData2()
        {
            try
            {
                _esClient = ElasticSearch5Wrapper.GetInstance(op);

                if (!_esClient.IndexExists(indexName).Exists)
                {
                    var descriptor = new CreateIndexDescriptor(indexName)
                                                        .InitializeUsing(ElasticSearch5Wrapper.IndexStata)
                                                        .Mappings(ms => ms
                                                        .Map<PatientWithEmrDto>(m => m
                                                            .AutoMap()
                                                            .Properties(p => p
                                                                                   .Nested<CPAT_EMR_RECORD>(n => n
                                                                                           .Name(c => c.Emr_Record_List)
                                                                                           .AutoMap())
                                                                                    .Nested<CPAT_CHECK_RECORD>(n => n
                                                                                            .Name(c => c.Check_Record_List)
                                                                                            .AutoMap()
                                                                                    )

                                                                             )
                                                        )
                                                        );
                    ICreateIndexResponse response = _esClient.CreateIndex(indexName, c => c = descriptor);

                    if (response.IsValid)
                    {
                        string msg = string.Format("索引" + indexName + "创建成功！");
                        Console.WriteLine(msg);
                    }
                    else
                    {
                        string msg = string.Format("索引" + indexName + "创建失败！");
                        Console.WriteLine(msg);
                    }

                }
                else
                {
                    Console.WriteLine("索引已经存在！");
                }


                //var a = new PutMappingDescriptor<PatientWithEmrDto>(indexName)
                //                                                            .Type("patient_with_emr_dto")
                //                                                            .AutoMap()
                //                                                            .Properties(p => p
                //                                                                   .Nested<CPAT_EMR_RECORD>(n => n
                //                                                                           .Name(c => c.Emr_Record_List)
                //                                                                           .AutoMap())
                //                                                                    .Nested<CPAT_CHECK_RECORD>(n=>n
                //                                                                            .Name(c => c.Check_Record_List)
                //                                                                            .AutoMap()
                //                                                                    )
                                                                                           
                //                                                             );

                //IPutMappingResponse response = _esClient.Map<PatientWithEmrDto>(c => a);

                //if (response.IsValid)
                //{
                //    string msg = string.Format("索引" + indexName + "创建成功！");
                //    Console.WriteLine(msg);
                //}
                //else
                //{
                //    string msg = string.Format("索引" + indexName + "创建失败！");
                //    Console.WriteLine(msg);
                //}
                var sw = new Stopwatch();
                sw.Start();

                using (var db = new NetCoreDbContext(options))
                {
                    var patientList =
                                       (from p in db.CPAT_IN_PATIENT.AsNoTracking()
                                        select new PatientWithEmrDto
                                        {
                                            PATIENT_NO = p.PATIENT_NO,
                                            PATIENT_ID = p.PATIENT_ID,
                                            PATIENT_NAME = p.PATIENT_NAME,
                                            BIRTH_DATE = p.BIRTH_DATE,
                                            GENDER_CODE = p.GENDER_CODE,
                                            COUNTRY_NAME = p.COUNTRY_NAME,
                                            Emr_Record_List = (from e in db.CPAT_EMR_RECORD.AsNoTracking()
                                                               where e.PATIENT_NO == p.PATIENT_NO
                                                               select e).ToList(),
                                            Check_Record_List =  (from c in db.CPAT_CHECK_RECORD.AsNoTracking()
                                                                  where c.PATIENT_NO == p.PATIENT_NO
                                                                  select c).ToList()
                                        }
                                   )
                                   .Take(100)
                                   .ToList();

                    //foreach (var p in patientList)
                    //{
                    //    var emrList = (from emr in db.CPAT_EMR_RECORD.AsNoTracking()
                    //                   where emr.PATIENT_NO == p.PATIENT_NO
                    //                   select emr).ToList();
                    //    p.Emr_Record_List = emrList;
                    //}


               //     var patientList = (from p in db.CPAT_IN_PATIENT.AsNoTracking()
               //                        join emr in db.CPAT_EMR_RECORD.AsNoTracking()
               //                        on p.PATIENT_NO equals emr.PATIENT_NO
               //                        join c in db.CPAT_CHECK_RECORD.AsNoTracking()
               //                        on p.PATIENT_NO equals c.PATIENT_NO
               //                        select new PatientWithEmrDto
               //                        {
               //                            PATIENT_NO = p.PATIENT_NO,
               //                            PATIENT_ID = p.PATIENT_ID,
               //                            PATIENT_NAME = p.PATIENT_NAME,
               //                            BIRTH_DATE = p.BIRTH_DATE,
               //                            GENDER_CODE = p.GENDER_CODE,
               //                            COUNTRY_NAME = p.COUNTRY_NAME,
               //                            Emr_Record_List = new List<CPAT_EMR_RECORD>
               //                            {
                                               
               //                                 new CPAT_EMR_RECORD
               //                                 {
               //                                     EMR_REC_ID = emr.EMR_REC_ID,
               //                                     PATIENT_NO = p.PATIENT_NO,
               //                                     PATIENT_ID = p.PATIENT_ID,
               //                                     EMR_TYPE_CODE = emr.EMR_TYPE_CODE,
               //                                     EMR_TYPE_NAME = emr.EMR_TYPE_NAME,
               //                                     REC_CONTENT = emr.REC_CONTENT
               //                                 }
               //                            },
               //                            Check_Record_List = new List<CPAT_CHECK_RECORD>
               //                            {
               //                                new CPAT_CHECK_RECORD
               //                                {
               //                                    CHECK_ID = c.CHECK_ID,
               //                                    PATIENT_NO = p.PATIENT_NO,
               //                                    CHECK_ITEM_CODE = c.CHECK_ITEM_CODE,
               //                                    CHECK_ITEM_NAME = c.CHECK_ITEM_NAME,
               //                                    CHECK_TYPE = c.CHECK_TYPE,
               //                                    EXEC_DEPT_CODE = c.EXEC_DEPT_CODE,
               //                                    EXEC_DEPT_NAME = c.EXEC_DEPT_NAME,
               //                                    REPORT_OBJECTIVE = c.REPORT_OBJECTIVE,
               //                                    REPORT_RESULT = c.REPORT_RESULT,
               //                                    REPORT_SUBJECTIVE = c.REPORT_SUBJECTIVE
               //                                }
               //                            }
               //                        }
               //)
               //.ToList();


                    sw.Stop();
                    Console.WriteLine("Get {0} message take {1} Milliseconds", patientList.Count, sw.ElapsedMilliseconds);

                    sw.Restart();


                    var len = Math.Ceiling(Convert.ToDecimal(patientList.Count() * 0.1));

                    for (var i = 0; i < len; i++)
                    {
                        BulkDescriptor bulkDescriptor = new BulkDescriptor();
                        var tmpList = patientList.Skip(i * 10).Take(10);
                        foreach (var doc in tmpList)
                        {
                            bulkDescriptor.Index<PatientWithEmrDto>(k => k
                                .Index(indexName)
                                .Type("patient_with_emr_dto")
                                .Id(doc.PATIENT_NO)
                                .Document(doc));
                        }
                        _esClient.Bulk(bulkDescriptor);
                    }



                    sw.Stop();

                    Console.WriteLine("Sync All Data to ES take {0} Milliseconds", sw.ElapsedMilliseconds);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        static void SetData3()
        {
            try
            {
                _esClient = ElasticSearch5Wrapper.GetInstance(op);
                var sw = new Stopwatch();
                sw.Start();

                using (var db = new NetCoreDbContext(options))
                {

                    var dictList = (from d in db.DP_DICT_DETAIL.AsNoTracking()
                                        select d)
                                   .ToList();
                    sw.Stop();
                    Console.WriteLine("Get {0} message take {1} Milliseconds", dictList.Count, sw.ElapsedMilliseconds);

                    sw.Restart();


                    BulkDescriptor bulkDescriptor = new BulkDescriptor();
                    foreach (var doc in dictList)
                    {
                        bulkDescriptor.Index<DP_DICT_DETAIL>(k => k
                            .Index(indexName)
                            .Type("dp_dict_detail")
                            .Document(doc));
                    }
                    _esClient.Bulk(bulkDescriptor);
                    sw.Stop();

                    Console.WriteLine("Sync All Data to ES take {0} Milliseconds", sw.ElapsedMilliseconds);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }








    


    





}
