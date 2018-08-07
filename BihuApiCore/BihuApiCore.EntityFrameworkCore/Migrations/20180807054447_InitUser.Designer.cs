﻿// <auto-generated />
using System;
using BihuApiCore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BihuApiCore.EntityFrameworkCore.Migrations
{
    [DbContext(typeof(EntityContext))]
    [Migration("20180807054447_InitUser")]
    partial class InitUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("BihuApiCore.EntityFrameworkCore.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint(20)");

                    b.Property<string>("CertificateNo")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<int>("IsVerify")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(4)")
                        .HasDefaultValueSql("'0'");

                    b.Property<long>("Mobile")
                        .HasColumnType("bigint(20)");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UserAccount")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<string>("UserPassWord")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("UserAccount")
                        .HasName("idx_account");

                    b.ToTable("user");
                });
#pragma warning restore 612, 618
        }
    }
}
