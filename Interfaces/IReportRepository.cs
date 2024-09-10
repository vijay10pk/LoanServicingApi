using System;
using LoanServicingApi.Models;
using LoanServicingApi.Models.Enums;

namespace LoanServicingApi.Interfaces
{
	public interface IReportRepository
	{
        Task<Report> GenerateReport(ReportType reportType, int userId);
        Task<Report> GetReportById(int id);
        Task<IEnumerable<Report>> GetReportsByUser(int userId);
        Task<IEnumerable<Report>> GetAllReports();
    }
}

