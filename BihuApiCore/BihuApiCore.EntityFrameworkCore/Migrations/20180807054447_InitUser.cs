using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BihuApiCore.EntityFrameworkCore.Migrations
{
    public partial class InitUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(type: "varchar(30)", nullable: false),
                    UserAccount = table.Column<string>(type: "varchar(20)", nullable: false),
                    UserPassWord = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    CertificateNo = table.Column<string>(type: "varchar(20)", nullable: false),
                    Mobile = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsVerify = table.Column<int>(type: "int(4)", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_account",
                table: "user",
                column: "UserAccount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
