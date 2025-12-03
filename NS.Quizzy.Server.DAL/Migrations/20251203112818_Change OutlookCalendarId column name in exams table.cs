using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOutlookCalendarIdcolumnnameinexamstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.RenameColumn(
                name: "OutlookCalendarId",
                table: "Exams",
                newName: "CalendarEventId");

            migrationBuilder.AlterColumn<Guid>(
                name: "MoedId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<bool>(
                name: "IsVisible",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false)
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExamTypeId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 4)
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DurationWithExtra",
                table: "Exams",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time")
                .Annotation("Relational:ColumnOrder", 3)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "Exams",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time")
                .Annotation("Relational:ColumnOrder", 2)
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "CalendarEventId",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 1)
                .OldAnnotation("Relational:ColumnOrder", 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CalendarEventId",
                table: "Exams",
                newName: "OutlookCalendarId");

            migrationBuilder.AlterColumn<Guid>(
                name: "MoedId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<bool>(
                name: "IsVisible",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false)
                .Annotation("Relational:ColumnOrder", 4)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExamTypeId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 3)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DurationWithExtra",
                table: "Exams",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time")
                .Annotation("Relational:ColumnOrder", 2)
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "Exams",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time")
                .Annotation("Relational:ColumnOrder", 1)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "OutlookCalendarId",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 1);

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
        }
    }
}
