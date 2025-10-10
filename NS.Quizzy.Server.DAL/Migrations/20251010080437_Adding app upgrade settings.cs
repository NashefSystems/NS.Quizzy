using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addingappupgradesettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "IsSecured", "Key", "Target", "Value", "ValueType" },
                values: new object[,]
                {
                    { new Guid("7a0f5b70-1b02-4e4b-898f-7c69b577d1c6"), false, "StoreUrlAndroid", "Client", "market://details?id=com.nashefsys.quizzy", "String" },
                    { new Guid("aa59c143-8dfe-4c7b-a95a-aedb9b823318"), false, "MinAppBuildNumberAndroid", "Client", "1000000040", "Long" },
                    { new Guid("c0f44347-da88-4bda-a872-f467cdc3de1b"), false, "MinAppBuildNumberIOS", "Client", "1000000040", "Long" },
                    { new Guid("e7ea810b-7b7f-429e-a46b-a2c6e85ff595"), false, "StoreUrlIOS", "Client", "itms-apps://apps.apple.com/app/id6745685962", "String" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("7a0f5b70-1b02-4e4b-898f-7c69b577d1c6"));

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("aa59c143-8dfe-4c7b-a95a-aedb9b823318"));

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("c0f44347-da88-4bda-a872-f467cdc3de1b"));

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("e7ea810b-7b7f-429e-a46b-a2c6e85ff595"));
        }
    }
}
