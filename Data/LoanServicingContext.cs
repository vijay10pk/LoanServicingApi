using System;
using LoanServicingApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanServicingApi.Data
{
    public class LoanServicingContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanModification> LoanModifications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<LoanOfficerPerformance> LoanOfficerPerformances { get; set; }

        public LoanServicingContext(DbContextOptions<LoanServicingContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Fullname);

                // Only configure the Role property if it needs special handling
                entity.Property(e => e.Role)
                    .HasConversion<string>();
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasOne(l => l.Borrower)
                .WithMany()
                .HasForeignKey(l => l.BorrowerId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.LoanOfficer)
                .WithMany()
                .HasForeignKey(l => l.LoanOfficerId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Status)
                      .HasConversion<string>();
            });

            modelBuilder.Entity<LoanModification>(entity =>
            {
                entity.ToTable("LoanModifications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ModifiedByUserId).HasColumnName("ModifiedByUserId");
                entity.HasOne(d => d.ModifiedBy)
                    .WithMany()
                    .HasForeignKey(d => d.ModifiedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.Property(e => e.ReportType)
                    .HasConversion<string>()
                    .HasMaxLength(50);
            });
        }

    }
}