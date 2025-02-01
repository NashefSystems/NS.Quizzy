using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddingMoed : Migration
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
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionnaireId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AddColumn<Guid>(
                name: "MoedId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.CreateTable(
                name: "Moeds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NewId())"),
                    ItemOrder = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moeds", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Moeds",
                columns: new[] { "Id", "ItemOrder", "Name" },
                values: new object[,]
                {
                    { new Guid("159bb5e6-9460-4e43-ba3c-b5967cf99f4e"), 3, "קיץ" },
                    { new Guid("1c3a5be0-9727-4a9d-b525-1461f33ded8f"), 4, "קיץ מועד ב'" },
                    { new Guid("1e33f673-6773-4db1-b6cf-909586a6544b"), 2, "אביב" },
                    { new Guid("72cde11d-b3f6-47f2-9909-51bcd30ff086"), 1, "חורף" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2325b8ae-f12a-43d8-be46-7041e57c9283"),
                column: "TwoFactorSecretKey",
                value: "L2PUPNK2U5SIIDHZUPWW6HHYRY7ZQSYX");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_MoedId",
                table: "Exams",
                column: "MoedId");

            migrationBuilder.CreateIndex(
                name: "IX_Moeds_Name",
                table: "Moeds",
                column: "Name",
                unique: true,
                filter: "IsDeleted = '0'");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Moeds_MoedId",
                table: "Exams",
                column: "MoedId",
                principalTable: "Moeds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Moeds_MoedId",
                table: "Exams");

            migrationBuilder.DropTable(
                name: "Moeds");

            migrationBuilder.DropIndex(
                name: "IX_Exams_MoedId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "MoedId",
                table: "Exams");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartTime",
                table: "Exams",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionnaireId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 4)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2325b8ae-f12a-43d8-be46-7041e57c9283"),
                column: "TwoFactorSecretKey",
                value: null);
        }
    }
}
