using System;
using System.Globalization;
using LoanServicingApi.Models;
using LoanServicingApi.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace LoanServicingApi.Data
{
    public class Seed
    {
        private readonly LoanServicingContext _context;

        public Seed(LoanServicingContext context)
        {
            _context = context;
        }

        private readonly Random random = new Random();

        public void SeedDataContext()
        {
            if (!_context.Users.Any())
            {
                // Seed Users
                var users = new List<User>();
                for (int i = 1; i <= 20; i++)
                {
                    UserRole role = i <= 2 ? UserRole.Admin :
                                    i <= 7 ? UserRole.LoanOfficer :
                                    UserRole.Borrower;
                    users.Add(new User
                    {
                        Fullname = $"{role}{i}",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword($"password{i}"),
                        Email = $"{role.ToString().ToLower()}{i}@example.com",
                        Role = role,
                        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(365, 730))
                    });
                }
                _context.Users.AddRange(users);
                _context.SaveChanges();

                // Seed Loans
                var loans = new List<Loan>();
                var loanOfficerIds = users.Where(u => u.Role == UserRole.LoanOfficer).Select(u => u.Id).ToList();
                var borrowerIds = users.Where(u => u.Role == UserRole.Borrower).Select(u => u.Id).ToList();
                for (int i = 1; i <= 50; i++)
                {
                    var startDate = DateTime.UtcNow.AddDays(-random.Next(365, 1095));
                    loans.Add(new Loan
                    {
                        BorrowerId = borrowerIds[random.Next(borrowerIds.Count)],
                        LoanOfficerId = loanOfficerIds[random.Next(loanOfficerIds.Count)],
                        Amount = random.Next(50000, 500000),
                        InterestRate = (decimal)(random.Next(300, 600) / 100.0),
                        StartDate = startDate,
                        EndDate = startDate.AddYears(30),
                        Status = (LoanStatus)random.Next(0, 5)
                    });
                }
                _context.Loans.AddRange(loans);
                _context.SaveChanges();

                // Seed LoanModifications
                var modifications = new List<LoanModification>();
                foreach (var loan in loans.Where(l => l.Status == LoanStatus.Modified))
                {
                    modifications.Add(new LoanModification
                    {
                        LoanId = loan.Id,
                        ModificationDate = loan.StartDate.AddMonths(random.Next(1, 12)),
                        NewInterestRate = loan.InterestRate - 0.5m,
                        NewAmount = loan.Amount - random.Next(5000, 20000),
                        NewEndDate = loan.EndDate.AddYears(5),
                        ModifiedByUserId = loanOfficerIds[random.Next(loanOfficerIds.Count)]
                    });
                }
                _context.LoanModifications.AddRange(modifications);
                _context.SaveChanges();

                // Seed Payments
                var payments = new List<Payment>();
                foreach (var loan in loans)
                {
                    int paymentCount = random.Next(1, 24);
                    for (int i = 0; i < paymentCount; i++)
                    {
                        payments.Add(new Payment
                        {
                            LoanId = loan.Id,
                            Amount = loan.Amount / 360, // Assuming 30-year loan
                            PaymentDate = loan.StartDate.AddMonths(i)
                        });
                    }
                }
                _context.Payments.AddRange(payments);
                _context.SaveChanges();

                // Seed Reports
                var reports = new List<Report>();
                var reportTypes = Enum.GetValues(typeof(ReportType)).Cast<ReportType>().ToList();
                for (int i = 1; i <= 20; i++)
                {
                    reports.Add(new Report
                    {
                        ReportType = reportTypes[random.Next(reportTypes.Count)],
                        GeneratedBy = users.First(u => u.Role == UserRole.Admin).Id,
                        GenerationDate = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                        ReportData = $"{{ \"metric1\": {random.Next(1000, 10000)}, \"metric2\": {random.Next(50, 100)} }}"
                    });
                }
                _context.Reports.AddRange(reports);
                _context.SaveChanges();

                // Seed LoanOfficerPerformance
                var performances = new List<LoanOfficerPerformance>();
                foreach (var officerId in loanOfficerIds)
                {
                    for (int i = 1; i <= 12; i++) // for Last 12 months
                    {
                        performances.Add(new LoanOfficerPerformance
                        {
                            LoanOfficerId = officerId,
                            PerformanceDate = DateTime.UtcNow.AddMonths(-i).Date,
                            LoansManaged = random.Next(5, 20),
                            CollectionRate = (decimal)(random.Next(9000, 10000) / 100.0),
                            CustomerSatisfactionScore = random.Next(7, 11)
                        });
                    }
                }
                _context.LoanOfficerPerformances.AddRange(performances);
                _context.SaveChanges();
            }
        }
    }
}
