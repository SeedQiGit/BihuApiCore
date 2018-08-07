using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public string UserPassWord { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string CertificateNo { get; set; }
        public long Mobile { get; set; }
        public int IsVerify { get; set; }

    }
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("user");

            entity.HasIndex(e => e.UserAccount)
                .HasName("idx_account");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");

            entity.Property(e => e.CertificateNo)
                .IsRequired()
                .HasColumnType("varchar(20)");

            entity.Property(e => e.CreateTime)
                 .IsRequired().HasColumnType("datetime");

            entity.Property(e => e.IsVerify)
                .HasColumnType("int(4)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Mobile).HasColumnType("bigint(20)");

            entity.Property(e => e.UpdateTime)
                .IsRequired()
                .HasColumnType("datetime");

            entity.Property(e => e.UserAccount)
                .IsRequired()
                .HasColumnType("varchar(20)");

            entity.Property(e => e.UserName)
                .IsRequired()
                .HasColumnType("varchar(30)");

            entity.Property(e => e.UserPassWord)
                .IsRequired()
                .HasColumnType("varchar(100)");
        }
    }


}
