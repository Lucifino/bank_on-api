using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace bank_on_api.Models.Entities.BankOn
{
    public partial class BankOn : DbContext
    {

        public BankOn(DbContextOptions<BankOn> options)
            : base(options)
        {
        }

        public virtual DbSet<BlackListGroup> BlackListGroup { get; set; } = null!;
        public virtual DbSet<FinanceProduct> FinanceProduct { get; set; } = null!;
        public virtual DbSet<FinanceRequest> FinanceRequest { get; set; } = null!;
        public virtual DbSet<FinanceRequestLog> FinanceRequestLog { get; set; } = null!;
        public virtual DbSet<FinanceRequestStatus> FinanceRequestStatus { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlackListGroup>(entity =>
            {
                entity.ToTable("BlackListGroup", "bank_on");

                entity.Property(e => e.BlackListGroupId).ValueGeneratedNever();

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e._Deleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FinanceProduct>(entity =>
            {
                entity.ToTable("FinanceProduct", "bank_on");

                entity.Property(e => e.FinanceProductId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AmountMin).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.EstablishmentRate).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.InterestRate).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e._Deleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FinanceRequest>(entity =>
            {
                entity.ToTable("FinanceRequest", "bank_on");

                entity.HasIndex(e => e.FinanceProductId, "IX_FinanceRequest_FinanceProductId");

                entity.HasIndex(e => e.FinanceRequestStatusId, "IX_FinanceRequest_FinanceRequestStatusId");

                entity.Property(e => e.FinanceRequestId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AmountRequired).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Mobile).HasMaxLength(15);

                entity.Property(e => e.MonthlyRepayment).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.ReferenceNo).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(10);

                entity.Property(e => e.TotalRepayment).HasColumnType("decimal(18, 6)");

                entity.Property(e => e._BlackListDomainFlag).HasDefaultValueSql("((0))");

                entity.Property(e => e._BlackListMobileFlag).HasDefaultValueSql("((0))");

                entity.Property(e => e._Deleted).HasDefaultValueSql("((0))");

                entity.Property(e => e._UnderAgeFlag).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.FinanceProduct)
                    .WithMany(p => p.FinanceRequest)
                    .HasForeignKey(d => d.FinanceProductId)
                    .HasConstraintName("fk_finance_product_finance_request_1");

                entity.HasOne(d => d.FinanceRequestStatus)
                    .WithMany(p => p.FinanceRequest)
                    .HasForeignKey(d => d.FinanceRequestStatusId)
                    .HasConstraintName("fk_finance_request_status_finance_request_1");
            });

            modelBuilder.Entity<FinanceRequestLog>(entity =>
            {
                entity.ToTable("FinanceRequestLog", "bank_on");

                entity.HasIndex(e => e.FinanceRequestId, "IX_FinanceRequestLog_FinanceRequestId");

                entity.Property(e => e.FinanceRequestLogId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e._Deleted).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.FinanceRequest)
                    .WithMany(p => p.FinanceRequestLog)
                    .HasForeignKey(d => d.FinanceRequestId)
                    .HasConstraintName("fk_finance_request_finance_request_log_1");
            });

            modelBuilder.Entity<FinanceRequestStatus>(entity =>
            {
                entity.ToTable("FinanceRequestStatus", "bank_on");

                entity.Property(e => e.FinanceRequestStatusId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e._Deleted).HasDefaultValueSql("((0))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
