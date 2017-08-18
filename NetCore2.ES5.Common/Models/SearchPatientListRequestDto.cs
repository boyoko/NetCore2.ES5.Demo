using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common.Models
{
    public class SearchPatientListRequestDto
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 每页数据条数
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// 可选参数 默认120
        /// </summary>
        public int MaxLength { get; set; } = 120;
        /// <summary>
        /// 科研code [dbo].[SCQ_CPATS] 表  [SCQ_ID] 字段
        /// </summary>
        public int ScqId { get; set; }
        /// <summary>
        /// false表示全部队列,   true未入科研队列 可选参数 默认false
        /// 当未true时，需要过滤搜索根据检索条件查到的数据，不能存在于[dbo].[SCQ_CPATS]表中
        /// 需要自动分页，如果满足条件的数据大于等于PageSize，则直接返回
        /// 如果数据小于PageSize，则自动翻页直到所有满足条件的数据进行筛选
        /// </summary>
        public bool IsJoinSC { get; set; }
        /// <summary>
        /// 病种编号 [dbo].[SD_CPATS] 表主键
        /// </summary>
        public string SdCode { get; set; }
        /// <summary>
        /// 检索条件
        /// </summary>
        public IList<SearchConditon> SearchList { get; set; }
    }

    public class SearchConditon
    {
        /// <summary>
        /// [dbo].[CPAT_EMR_RECORD]  表中 [EMR_TYPE_CODE] 字段
        /// </summary>
        public string CONDITION_TYPE { get; set; }
        /// <summary>
        /// 全文检索 [dbo].[CPAT_EMR_RECORD]  表中 [REC_CONTENT] 字段
        /// </summary>
        public string CONDITION { get; set; }
    }
}
