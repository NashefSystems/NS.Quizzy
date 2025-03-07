using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addemailtoappsettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "IsSecured", "Key", "Target", "Value", "ValueType" },
                values: new object[] { new Guid("4f43fb54-4bbd-4f04-8dfe-14bd5078946e"), false, "Email", "All", "Quizzy@ExamProduction.com", "String" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("4f43fb54-4bbd-4f04-8dfe-14bd5078946e"));
        }
    }
}
