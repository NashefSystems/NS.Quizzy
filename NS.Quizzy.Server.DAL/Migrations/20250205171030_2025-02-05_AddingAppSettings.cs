using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class _20250205_AddingAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AppSettings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "IsSecured", "Key", "Target", "Value", "ValueType" },
                values: new object[,]
                {
                    { new Guid("4aaa77c9-37e3-417d-8549-0c541869ca6c"), false, "ServerInfoTTLMin", "Server", "10080", "Integer" },
                    { new Guid("4d9c6810-644d-4ac4-b00f-245aa2b69086"), false, "CacheDataTTLMin", "Server", "300", "Integer" },
                    { new Guid("586641c6-6289-4dcf-bc0b-0f5513cbd911"), false, "CacheLoginsTTLMin", "Server", "10080", "Integer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("4aaa77c9-37e3-417d-8549-0c541869ca6c"));

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("4d9c6810-644d-4ac4-b00f-245aa2b69086"));

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("586641c6-6289-4dcf-bc0b-0f5513cbd911"));

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AppSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
