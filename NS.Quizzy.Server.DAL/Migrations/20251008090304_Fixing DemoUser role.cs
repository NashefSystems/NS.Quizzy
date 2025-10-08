using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixingDemoUserrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f8178f51-6d73-45e1-a4e7-1b01baf9884d"),
                column: "Role",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f8178f51-6d73-45e1-a4e7-1b01baf9884d"),
                column: "Role",
                value: 0);
        }
    }
}
