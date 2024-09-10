using System;
using System.Text.Json;
using LoanServicingApi.Data;
using LoanServicingApi.Interfaces;
using LoanServicingApi.Models;
using LoanServicingApi.Models.Enums;
using Microsoft.EntityFrameworkCore;
using static LoanServicingApi.Exceptions.LoanServicingExceptions;

namespace LoanServicingApi.Repositories
{
	public class ReportRepository : IReportRepository
	{
        private readonly LoanServicingContext _context;

        public ReportRepository(LoanServicingContext context)
        {
            _context = context;
        }

        public async Task<Report> GenerateReport(ReportType reportType, int userId)
        {
            string reportData = await GenerateReportData(reportType);

            var report = new Report
            {
                ReportType = reportType,
                GeneratedBy = userId,
                GenerationDate = DateTime.UtcNow.Date,
                ReportData = reportData
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        private async Task<string> GenerateReportData(ReportType reportType)
        {
            switch (reportType)
            {
                case ReportType.Internal:
                    return await GenerateInternalReport();
                case ReportType.Regulatory:
                    return await GenerateRegulatoryReport();
                case ReportType.Performance:
                    return await GeneratePerformanceReport();
                default:
                    throw new ArgumentException("Invalid report type");
            }
        }

        private async Task<string> GenerateInternalReport()
        {
            var totalLoans = await _context.Loans.CountAsync();
            var activeLoans = await _context.Loans.CountAsync(l => l.Status == LoanStatus.Active);
            var modifiedLoans = await _context.Loans.CountAsync(l => l.Status == LoanStatus.Modified);
            var totalAmount = await _context.Loans.SumAsync(l => l.Amount);
            var averageInterestRate = await _context.Loans.AverageAsync(l => l.InterestRate);

            var reportData = new
            {
                title = "Internal Loan Performance Report",
                totalLoans,
                activeLoans,
                modifiedLoans,
                totalAmount = Math.Round(totalAmount, 2),
                averageInterestRate = Math.Round(averageInterestRate, 2),
                generationDate = DateTime.UtcNow.Date
            };

            return JsonSerializer.Serialize(reportData);
        }

        private async Task<string> GenerateRegulatoryReport()
        {
            var totalLoans = await _context.Loans.CountAsync();
            var activeLoans = await _context.Loans.CountAsync(l => l.Status == LoanStatus.Active);
            var modifiedLoans = await _context.Loans.CountAsync(l => l.Status == LoanStatus.Modified);

            // Assuming compliance means the loan is either active or has been properly modified
            var compliantLoans = activeLoans + modifiedLoans;
            var complianceRate = totalLoans > 0 ? (double)compliantLoans / totalLoans * 100 : 100;

            var reportData = new
            {
                title = "Regulatory Compliance Report",
                totalLoans,
                activeLoans,
                modifiedLoans,
                compliantLoans,
                complianceRate = Math.Round(complianceRate, 2),
                generationDate = DateTime.UtcNow.Date
            };

            return JsonSerializer.Serialize(reportData);
        }

        private async Task<string> GeneratePerformanceReport()
        {
            var totalLoanOfficers = await _context.Users.CountAsync(u => u.Role == UserRole.LoanOfficer);
            var totalLoans = await _context.Loans.CountAsync();
            var averageLoansPerOfficer = totalLoanOfficers > 0 ? (double)totalLoans / totalLoanOfficers : 0;

            var topPerformer = await _context.Loans
                .GroupBy(l => l.LoanOfficerId)
                .Select(g => new {
                    LoanOfficerId = g.Key,
                    LoansProcessed = g.Count()
                })
                .OrderByDescending(x => x.LoansProcessed)
                .FirstOrDefaultAsync();

            string topPerformerName = "N/A";
            if (topPerformer != null)
            {
                var topPerformerUser = await _context.Users.FindAsync(topPerformer.LoanOfficerId);
                topPerformerName = topPerformerUser?.Fullname ?? "N/A";
            }

            var reportData = new
            {
                title = "Loan Officer Performance Report",
                totalLoanOfficers,
                totalLoans,
                averageLoansPerOfficer = Math.Round(averageLoansPerOfficer, 2),
                topPerformer = new
                {
                    name = topPerformerName,
                    loansProcessed = topPerformer?.LoansProcessed ?? 0
                },
                generationDate = DateTime.UtcNow.Date
            };

            return JsonSerializer.Serialize(reportData);
        }

        public async Task<Report> GetReportById(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                throw new NotFoundException($"Report with ID {id} not found");
            }
            return report;
        }

        public async Task<IEnumerable<Report>> GetReportsByUser(int userId)
        {
            return await _context.Reports.Where(r => r.GeneratedBy == userId).ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetAllReports()
        {
            return await _context.Reports.ToListAsync();
        }
    }
}

