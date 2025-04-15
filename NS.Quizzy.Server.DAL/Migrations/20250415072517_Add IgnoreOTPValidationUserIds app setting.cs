using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIgnoreOTPValidationUserIdsappsetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "IsSecured", "Key", "Target", "Value", "ValueType" },
                values: new object[] { new Guid("648308a0-690f-45e7-b4a8-c0f0680625ac"), false, "IgnoreOTPValidationUserIds", "Server", "[]", "Json" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("648308a0-690f-45e7-b4a8-c0f0680625ac"));
        }
    }
}
