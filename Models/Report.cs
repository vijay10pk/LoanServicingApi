using System;
using LoanServicingApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace LoanServicingApi.Models
{
	public class Report
	{
        [Key]
        public int Id { get; set; }
        public ReportType ReportType { get; set; }
        public int GeneratedBy { get; set; }
        public DateTime GenerationDate { get; set; }
        public string ReportData { get; set; }
    }
}

