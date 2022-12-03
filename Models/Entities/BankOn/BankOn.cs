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

        public virtual DbSet<finance_product> finance_product { get; set; } = null!;
        public virtual DbSet<finance_request> finance_request { get; set; } = null!;
        public virtual DbSet<finance_request_log> finance_request_log { get; set; } = null!;
        public virtual DbSet<finance_request_status> finance_request_status { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<finance_product>(entity =>
            {
                entity.HasKey(e => e.finance_product_id)
                    .HasName("PK_finance_product_finance_product_id");

                entity.ToTable("finance_product", "bank_on");

                entity.Property(e => e.finance_product_id).ValueGeneratedNever();

                entity.Property(e => e.amount_min).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.description).HasMaxLength(500);

                entity.Property(e => e.interest_rate).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.title).HasMaxLength(100);
            });

            modelBuilder.Entity<finance_request>(entity =>
            {
                entity.HasKey(e => e.finance_request_id)
                    .HasName("PK_finance_request_finance_request_id");

                entity.ToTable("finance_request", "bank_on");

                entity.Property(e => e.finance_request_id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AmountRequired).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Mobile).HasMaxLength(15);

                entity.Property(e => e.ReferenceNo).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(10);

                entity.HasOne(d => d.finance_product)
                    .WithMany(p => p.finance_request)
                    .HasForeignKey(d => d.finance_product_id)
                    .HasConstraintName("fk_finance_product_finance_request_1");

                entity.HasOne(d => d.finance_request_status)
                    .WithMany(p => p.finance_request)
                    .HasForeignKey(d => d.finance_request_status_id)
                    .HasConstraintName("fk_finance_request_status_finance_request_1");
            });

            modelBuilder.Entity<finance_request_log>(entity =>
            {
                entity.HasKey(e => e.finance_request_log_id)
                    .HasName("PK_finance_request_log_id");

                entity.ToTable("finance_request_log", "bank_on");

                entity.Property(e => e.finance_request_log_id).ValueGeneratedNever();

                entity.Property(e => e._case).HasColumnName("case");

                entity.Property(e => e.date_created).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.description).HasMaxLength(500);

                entity.Property(e => e.title).HasMaxLength(100);

                entity.HasOne(d => d.finance_request)
                    .WithMany(p => p.finance_request_log)
                    .HasForeignKey(d => d.finance_request_id)
                    .HasConstraintName("fk_finance_request_finance_request_log_1");
            });

            modelBuilder.Entity<finance_request_status>(entity =>
            {
                entity.HasKey(e => e.finance_request_status_id)
                    .HasName("PK_finance_request_status_id");

                entity.ToTable("finance_request_status", "bank_on");

                entity.Property(e => e.finance_request_status_id).ValueGeneratedNever();

                entity.Property(e => e.description).HasMaxLength(500);

                entity.Property(e => e.title).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
