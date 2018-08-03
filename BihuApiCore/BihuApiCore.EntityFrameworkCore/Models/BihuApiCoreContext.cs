using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public partial class BihuApiCoreContext : DbContext
    {
        public BihuApiCoreContext()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public BihuApiCoreContext(DbContextOptions<BihuApiCoreContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public virtual DbSet<User> User { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseMySql("Server=localhost;User Id=root;Password=123456;Database= bihu_apicore");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.UserAccount)
                    .HasName("idx_account");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CertificateNo)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("'-1'");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.IsVerify)
                    .HasColumnType("int(4)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Mobile).HasColumnType("bigint(20)");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("UpdateTIme")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UserAccount)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.UserPassWord)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });
        }
    }
}
