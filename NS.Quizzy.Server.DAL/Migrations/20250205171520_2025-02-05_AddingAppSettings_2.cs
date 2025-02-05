using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class _20250205_AddingAppSettings_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("4aaa77c9-37e3-417d-8549-0c541869ca6c"),
                column: "Value",
                value: "20160");

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("586641c6-6289-4dcf-bc0b-0f5513cbd911"),
                column: "Value",
                value: "20160");

            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "IsSecured", "Key", "Target", "Value", "ValueType" },
                values: new object[] { new Guid("c34bf183-1ff4-4b96-8524-076243aad56b"), false, "CacheOTPTTLMin", "Server", "60", "Integer" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("c34bf183-1ff4-4b96-8524-076243aad56b"));

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("4aaa77c9-37e3-417d-8549-0c541869ca6c"),
                column: "Value",
                value: "10080");

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("586641c6-6289-4dcf-bc0b-0f5513cbd911"),
                column: "Value",
                value: "10080");
        }
    }
}
