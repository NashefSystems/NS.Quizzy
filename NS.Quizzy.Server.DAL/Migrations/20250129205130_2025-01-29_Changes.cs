using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class _20250129_Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_Number",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Questionnaires");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Questionnaires",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)")
                .Annotation("Relational:ColumnOrder", 4)
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DurationWithExtra",
                table: "Questionnaires",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time")
                .Annotation("Relational:ColumnOrder", 3)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "Questionnaires",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time")
                .Annotation("Relational:ColumnOrder", 2)
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<long>(
                name: "Code",
                table: "Questionnaires",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_Code",
                table: "Questionnaires",
                column: "Code",
                unique: true,
                filter: "IsDeleted = '0'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_Code",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Questionnaires");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Questionnaires",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)")
                .Annotation("Relational:ColumnOrder", 3)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DurationWithExtra",
                table: "Questionnaires",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time")
                .Annotation("Relational:ColumnOrder", 2)
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "Questionnaires",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time")
                .Annotation("Relational:ColumnOrder", 1)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Questionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_Number",
                table: "Questionnaires",
                column: "Number",
                unique: true,
                filter: "IsDeleted = '0'");
        }
    }
}
