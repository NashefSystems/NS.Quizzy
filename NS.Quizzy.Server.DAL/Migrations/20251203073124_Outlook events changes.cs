using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Outlookeventschanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartTime",
                table: "Exams",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionnaireId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AddColumn<string>(
                name: "OutlookCalendarId",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 6);

            migrationBuilder.CreateTable(
                name: "EventEmails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NewId())"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventEmails", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "IsSecured", "Key", "Target", "Value", "ValueType" },
                values: new object[,]
                {
                    { new Guid("3171793a-e0f3-4dae-a9f7-5a1117cc5fc2"), false, "GraphClientId", "Server", "0acb772f-e573-47b4-91b8-1d85a89bebc5", "String" },
                    { new Guid("5954ee58-2839-4113-8a0b-825f7e0f07e6"), false, "GraphRedirectUri", "Server", "http://localhost", "String" },
                    { new Guid("64be7431-20cc-446d-a5fb-d06f69ec8ec5"), false, "GraphTimeZone", "Server", "Israel Standard Time", "String" },
                    { new Guid("de672b80-c6f0-44bf-ad80-a482ff28545a"), false, "GraphScopes", "Server", "[ \"Calendars.ReadWrite\" ]", "Json" },
                    { new Guid("ebce4a11-04d3-4c1c-8685-e9ef8e572ee8"), false, "GraphAuthRecord", "Server", "{\"username\":\"examproduction@outlook.com\",\"authority\":\"login.microsoftonline.com\",\"homeAccountId\":\"00000000-0000-0000-1b54-45a3620d2ac5.9188040d-6c67-4c5b-b112-36a304b66dad\",\"tenantId\":\"9188040d-6c67-4c5b-b112-36a304b66dad\",\"clientId\":\"0acb772f-e573-47b4-91b8-1d85a89bebc5\",\"version\":\"1.0\"}", "String" },
                    { new Guid("fdcf995f-779a-4972-9850-4453633f475c"), false, "GraphTenantId", "Server", "consumers", "String" }
                });

            migrationBuilder.InsertData(
                table: "EventEmails",
                columns: new[] { "Id", "Email", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("0016a6d6-6164-4e3d-8c74-29053d1c041f"), "Rami33z@gmail.com", true, "רמי גבאלי" },
                    { new Guid("0c4805ca-3a80-4d25-b5be-1cbddc8ca21b"), "isradr4@gmail.com", true, "אסראא חג יחיא" },
                    { new Guid("1409a71a-9509-43f6-8697-ff5751a0f1f4"), "Manalazem@gmail.com", true, "מנאל עאזם" },
                    { new Guid("2b8df855-f9b1-47dd-aad5-31fcb2398f5b"), "amernj22@gmail.com", true, "אמיר גבאלי" }
                });

            migrationBuilder.InsertData(
                table: "EventEmails",
                columns: new[] { "Id", "Email", "Name" },
                values: new object[] { new Guid("cfa04e13-77eb-4905-a98f-0feda0244137"), "saji.nashef@gmail.com", "סאגי נאשף" });

            migrationBuilder.InsertData(
                table: "EventEmails",
                columns: new[] { "Id", "Email", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("d469f77e-be7e-4283-9e1d-bd8603d6d0b2"), "farida.j.81@gmail.com", true, "פרידה גאבר" },
                    { new Guid("f2488b03-254f-46b7-9fd4-aa47a3373ead"), "hilalmsa@gmail.com", true, "הלאל מסארוה" },
                    { new Guid("f4563844-b894-46c7-9d91-9ad48524635e"), "shotawil@gmail.com", true, "שרוק טויל" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventEmails_Email",
                table: "EventEmails",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventEmails");

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("3171793a-e0f3-4dae-a9f7-5a1117cc5fc2"));

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("5954ee58-2839-4113-8a0b-825f7e0f07e6"));

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("64be7431-20cc-446d-a5fb-d06f69ec8ec5"));

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("de672b80-c6f0-44bf-ad80-a482ff28545a"));

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("ebce4a11-04d3-4c1c-8685-e9ef8e572ee8"));

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("fdcf995f-779a-4972-9850-4453633f475c"));

            migrationBuilder.DropColumn(
                name: "OutlookCalendarId",
                table: "Exams");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartTime",
                table: "Exams",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionnaireId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 7);
        }
    }
}
