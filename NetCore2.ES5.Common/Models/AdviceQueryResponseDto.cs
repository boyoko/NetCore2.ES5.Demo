using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common.Models
{
    public class AdviceQueryResponseDto
    {
        public string EmrId { get; set; }
        public string PatientNO { get; set; }
        public string PatientName { get; set; }
        public string CheckType { get; set; }
        public string EmrType { get; set; }
        public string EmrContext { get; set; }
        public string ReportContext { get; set; }
    }
}
