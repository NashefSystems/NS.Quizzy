using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddnewAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "IsSecured", "Key", "Target", "Value", "ValueType" },
                values: new object[] { new Guid("795b147f-6de4-44e1-bd09-6b2b778fc2d3"), false, "IdNumberEmailDomain", "All", "Quizzy.ExamProduction.com", "String" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("795b147f-6de4-44e1-bd09-6b2b778fc2d3"));
        }
    }
}
