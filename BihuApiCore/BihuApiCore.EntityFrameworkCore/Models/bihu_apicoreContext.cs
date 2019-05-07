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

        public virtual DbSet<Companies> Companies { get; set; }
        public virtual DbSet<CompanyModuleRelation> CompanyModuleRelation { get; set; }
        public virtual DbSet<DataExcel> DataExcel { get; set; }
        public virtual DbSet<Modules> Modules { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<RoleModuleRelation> RoleModuleRelation { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserConfig> UserConfig { get; set; }
        public virtual DbSet<UserExtent> UserExtent { get; set; }
        public virtual DbSet<ZsPiccCall> ZsPiccCall { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=localhost;User Id=root;Password=123456;Database= bihu_apicore ");
              
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Companies>(entity =>
            {
                entity.HasKey(e => e.CompId)
                    .HasName("PRIMARY");

                entity.ToTable("companies");

                entity.Property(e => e.CompId).HasColumnType("bigint(20)");

                entity.Property(e => e.ClientName).HasColumnType("varchar(30)");

                entity.Property(e => e.CompName)
                    .IsRequired()
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.CompanyType).HasColumnType("int(11)");

                entity.Property(e => e.CreatedEmp).HasColumnType("bigint(20)");

                entity.Property(e => e.Createdtime).HasDefaultValueSql("'CURRENT_TIMESTAMP(6)'");

                entity.Property(e => e.IsUsed).HasColumnType("bit(1)");

                entity.Property(e => e.LevelCode)
                    .IsRequired()
                    .HasColumnType("varchar(2000)");

                entity.Property(e => e.LevelNum).HasColumnType("int(4)");

                entity.Property(e => e.ParentCompId).HasColumnType("bigint(20)");

                entity.Property(e => e.PayType).HasColumnType("int(11)");

                entity.Property(e => e.PiccAccount).HasColumnType("int(11)");

                entity.Property(e => e.Region).HasColumnType("varchar(200)");

                entity.Property(e => e.SecretKey).HasColumnType("varchar(200)");

                entity.Property(e => e.TopAgentId).HasColumnType("bigint(20)");

                entity.Property(e => e.UpdatedEmp).HasColumnType("bigint(20)");

                entity.Property(e => e.Updatedtime)
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP(6)'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ZsType).HasColumnType("int(11)");
            });

            modelBuilder.Entity<CompanyModuleRelation>(entity =>
            {
                entity.ToTable("company_module_relation");

                entity.HasIndex(e => e.CompId)
                    .HasName("idx_compId");

                entity.HasIndex(e => e.ModuleCode)
                    .HasName("idx_moduleCode");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CompId).HasColumnType("bigint(20)");

                entity.Property(e => e.CreatedEmp).HasColumnType("bigint(20)");

                entity.Property(e => e.Createdtime).HasDefaultValueSql("'CURRENT_TIMESTAMP(6)'");

                entity.Property(e => e.ModuleCode)
                    .IsRequired()
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.UpdatedEmp).HasColumnType("bigint(20)");

                entity.Property(e => e.Updatedtime)
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP(6)'")
                    .ValueGeneratedOnAddOrUpdate();
            });

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

            modelBuilder.Entity<Modules>(entity =>
            {
                entity.HasKey(e => e.ModuleCode)
                    .HasName("PRIMARY");

                entity.ToTable("modules");

                entity.Property(e => e.ModuleCode).HasColumnType("varchar(255)");

                entity.Property(e => e.ActionUrl)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.CreatedEmp).HasColumnType("bigint(20)");

                entity.Property(e => e.Createdtime).HasDefaultValueSql("'CURRENT_TIMESTAMP(6)'");

                entity.Property(e => e.Icon).HasColumnType("varchar(300)");

                entity.Property(e => e.IsUserd).HasColumnType("int(11)");

                entity.Property(e => e.ModuleLevel).HasColumnType("int(11)");

                entity.Property(e => e.ModuleName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.ModuleType).HasColumnType("int(11)");

                entity.Property(e => e.OrderBy).HasColumnType("int(11)");

                entity.Property(e => e.ParentCode)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.PlatformType).HasColumnType("int(11)");

                entity.Property(e => e.UpdatedEmp).HasColumnType("bigint(20)");

                entity.Property(e => e.Updatedtime)
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP(6)'")
                    .ValueGeneratedOnAddOrUpdate();
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

            modelBuilder.Entity<RoleModuleRelation>(entity =>
            {
                entity.ToTable("role_module_relation");

                entity.HasIndex(e => e.RoleId)
                    .HasName("idx_roleId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CreatedEmp).HasColumnType("bigint(20)");

                entity.Property(e => e.Createdtime).HasDefaultValueSql("'CURRENT_TIMESTAMP(6)'");

                entity.Property(e => e.ModuleCode)
                    .IsRequired()
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.RoleId).HasColumnType("bigint(20)");

                entity.Property(e => e.UpdatedEmp).HasColumnType("bigint(20)");

                entity.Property(e => e.Updatedtime)
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP(6)'")
                    .ValueGeneratedOnAddOrUpdate();
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
