using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAppBuildNumbertodevicestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppVersionName",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "Devices",
                newName: "ID");

            migrationBuilder.AlterColumn<string>(
                name: "UniqueId",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)")
                .Annotation("Relational:ColumnOrder", 16)
                .OldAnnotation("Relational:ColumnOrder", 15);

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)")
                .Annotation("Relational:ColumnOrder", 15)
                .OldAnnotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<string>(
                name: "OSVersion",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)")
                .Annotation("Relational:ColumnOrder", 14)
                .OldAnnotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<string>(
                name: "OS",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)")
                .Annotation("Relational:ColumnOrder", 13)
                .OldAnnotation("Relational:ColumnOrder", 12);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastHeartBeat",
                table: "Devices",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "(SYSDATETIMEOFFSET())",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "(SYSDATETIMEOFFSET())")
                .Annotation("Relational:ColumnOrder", 12)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<bool>(
                name: "IsWindows",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<bool>(
                name: "IsWeb",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTesting",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTV",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMacOS",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<bool>(
                name: "IsIOS",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAndroid",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "Devices",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "(SYSDATETIMEOFFSET())",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "(SYSDATETIMEOFFSET())")
                .Annotation("Relational:ColumnOrder", 4)
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AddColumn<string>(
                name: "AppBuildNumber",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AddColumn<string>(
                name: "AppVersion",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppBuildNumber",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "AppVersion",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Devices",
                newName: "Key");

            migrationBuilder.AlterColumn<string>(
                name: "UniqueId",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 15)
                .OldAnnotation("Relational:ColumnOrder", 16);

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 14)
                .OldAnnotation("Relational:ColumnOrder", 15);

            migrationBuilder.AlterColumn<string>(
                name: "OSVersion",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 13)
                .OldAnnotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<string>(
                name: "OS",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 12)
                .OldAnnotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastHeartBeat",
                table: "Devices",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "(SYSDATETIMEOFFSET())",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "(SYSDATETIMEOFFSET())")
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 12);

            migrationBuilder.AlterColumn<bool>(
                name: "IsWindows",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<bool>(
                name: "IsWeb",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTesting",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTV",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMacOS",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<bool>(
                name: "IsIOS",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAndroid",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 4)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "Devices",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "(SYSDATETIMEOFFSET())",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "(SYSDATETIMEOFFSET())")
                .Annotation("Relational:ColumnOrder", 3)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AddColumn<string>(
                name: "AppVersionName",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 2);
        }
    }
}
