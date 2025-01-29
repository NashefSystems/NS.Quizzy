using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTwoFactorSecretKeyvalueforadminuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b900d543-90ab-4e7a-83ba-b961918dcc8c"),
                column: "TwoFactorSecretKey",
                value: "XD2GB3DYXAGZGGOXG46TD3QKBQXQYYKO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b900d543-90ab-4e7a-83ba-b961918dcc8c"),
                column: "TwoFactorSecretKey",
                value: null);
        }
    }
}
