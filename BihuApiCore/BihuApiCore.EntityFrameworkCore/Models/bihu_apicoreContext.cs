using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public partial class bihu_apicoreContext : DbContext
    {
        public bihu_apicoreContext()
        {
        }

        public bihu_apicoreContext(DbContextOptions<bihu_apicoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DataExcel> DataExcel { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserConfig> UserConfig { get; set; }
        public virtual DbSet<UserExtent> UserExtent { get; set; }
        public virtual DbSet<ZsPiccCall> ZsPiccCall { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=localhost;User Id=root;Password=123456;Database=bihu_apicore;sslmode=none;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataExcel>(entity =>
            {
                entity.ToTable("data_excel");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.CallExtNumber)
                    .IsRequired()
                    .HasColumnName("call_ext_number")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.CallNumber)
                    .IsRequired()
                    .HasColumnName("call_number")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.CallPassword)
                    .IsRequired()
                    .HasColumnName("call_password")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.DirectNumber)
                    .HasColumnName("direct_number")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasColumnType("varchar(200)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Price).HasColumnType("decimal(8,2)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.UserAccount)
                    .HasName("idx_account");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CertificateNo)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.IsVerify)
                    .HasColumnType("int(4)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Mobile).HasColumnType("bigint(20)");

                entity.Property(e => e.UpdateTime)
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

            modelBuilder.Entity<UserConfig>(entity =>
            {
                entity.ToTable("user_config");

                entity.HasIndex(e => e.UserId)
                    .HasName("idx_userId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UserGrade).HasColumnType("int(5)");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");

                entity.Property(e => e.UserLevel).HasColumnType("int(5)");
            });

            modelBuilder.Entity<UserExtent>(entity =>
            {
                entity.ToTable("user_extent");

                entity.HasIndex(e => e.UserId)
                    .HasName("idx_userId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UserHobby)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");

                entity.Property(e => e.UserOccupation)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<ZsPiccCall>(entity =>
            {
                entity.ToTable("zs_picc_call");

                entity.HasIndex(e => e.UserAgentId)
                    .HasName("idx_agent_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.CallExtNumber)
                    .IsRequired()
                    .HasColumnName("call_ext_number")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.CallId)
                    .HasColumnName("call_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.CallNumber)
                    .IsRequired()
                    .HasColumnName("call_number")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.CallPassword)
                    .IsRequired()
                    .HasColumnName("call_password")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.CallState)
                    .HasColumnName("call_state")
                    .HasColumnType("int(4)");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.DirectNumber)
                    .HasColumnName("direct_number")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UserAgentId)
                    .HasColumnName("user_agent_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("varchar(50)");
            });
        }
    }
}
