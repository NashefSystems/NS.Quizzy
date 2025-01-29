using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addcodecolumntoclassesandgradestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Grades",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("Relational:ColumnOrder", 2)
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<long>(
                name: "Code",
                table: "Grades",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Classes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("Relational:ColumnOrder", 3)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<Guid>(
                name: "GradeId",
                table: "Classes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 2)
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<long>(
                name: "Code",
                table: "Classes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("081057a2-16df-4e2b-9e90-0cdc0dfff8f9"),
                column: "Code",
                value: 9L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("167ce96a-a9f0-4910-862a-19bc0bcdf820"),
                column: "Code",
                value: 3L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("1a9c42c2-148b-4da9-8620-a4b70336f451"),
                columns: new[] { "Code", "Name" },
                values: new object[] { 9L, "י\"ב 9" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("21e2f08f-9793-40aa-98a0-bbf3a8759e40"),
                column: "Code",
                value: 4L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("2a3c8f01-ecc2-42c0-8251-16fcd68944f5"),
                column: "Code",
                value: 11L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("36b9d045-ac87-4c29-99af-ccc4755b13cc"),
                column: "Code",
                value: 6L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("46c5b044-5020-4f7f-900c-93623be8bbaf"),
                columns: new[] { "Code", "Name" },
                values: new object[] { 6L, "י\"ב 6" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("49211014-42b9-4486-b6a3-fb9cc7e57426"),
                column: "Code",
                value: 7L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("494920f9-f6d9-405c-9536-afe619749ccd"),
                column: "Code",
                value: 1L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("5b107d79-7df7-4f6d-8bd4-608230492e07"),
                column: "Code",
                value: 9L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("782cb34d-7091-4e9e-9bda-c30d11e1a64d"),
                column: "Code",
                value: 5L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("7a9808bd-2d4e-48e2-8692-67d44ef962ce"),
                columns: new[] { "Code", "Name" },
                values: new object[] { 3L, "י\"ב 3" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("7d88b6fe-f3f3-4282-b475-82b2908e9fe0"),
                column: "Code",
                value: 10L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("859c1031-c539-4deb-adff-93bcfec85774"),
                column: "Code",
                value: 11L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("8c70d35a-9a3a-4bc6-8412-778f72338c63"),
                columns: new[] { "Code", "Name" },
                values: new object[] { 4L, "י\"ב 4" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("8e1f2e5e-7597-494f-bde9-5b290918c202"),
                column: "Code",
                value: 7L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("91b2c738-8e6c-4334-bc20-9778ef792af4"),
                columns: new[] { "Code", "Name" },
                values: new object[] { 7L, "י\"ב 7" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("a238c990-8fef-4e94-8bf4-b1e9dc818dde"),
                column: "Code",
                value: 11L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("b096c5e7-76c6-405c-a044-5fbdbe222a3c"),
                column: "Code",
                value: 8L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("b641e55e-5a1e-42e9-8acb-1356340c87f5"),
                column: "Code",
                value: 10L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("c3424585-63a2-4e52-940a-b8979557b6ce"),
                column: "Code",
                value: 2L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("c3f075c7-93ce-4122-ab7c-b989cf8e6786"),
                column: "Code",
                value: 3L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("ca5695ed-4d8a-485f-9072-29dcbc29bf84"),
                columns: new[] { "Code", "Name" },
                values: new object[] { 1L, "י\"ב 1" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("cbea4226-569b-4ec0-a330-26e2094fe2a7"),
                column: "Code",
                value: 4L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("ce0bd2ff-3444-435b-a17d-03ae94936904"),
                column: "Code",
                value: 2L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("d22d38b9-0398-4aae-8ea7-a13237cace64"),
                columns: new[] { "Code", "Name" },
                values: new object[] { 8L, "י\"ב 8" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("d514ef35-7024-4836-be29-02e5f5b3bbcb"),
                column: "Code",
                value: 8L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("e5928d27-abbf-49b9-a34b-0329b754cd76"),
                column: "Code",
                value: 1L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("ebb4d04c-e099-48c8-ae3c-b123354e250c"),
                column: "Code",
                value: 10L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("ebbb2b2e-4833-4860-8843-40f9cf852be7"),
                columns: new[] { "Code", "Name" },
                values: new object[] { 2L, "י\"ב 2" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("f093a8a6-91d2-46f4-bc8d-42a489fc75f6"),
                column: "Code",
                value: 6L);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("f48dd2f8-9a66-4052-b31c-793e60df5004"),
                columns: new[] { "Code", "Name" },
                values: new object[] { 5L, "י\"ב 5" });

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("f6845989-9704-4a39-a5d2-31f6a0df9cb3"),
                column: "Code",
                value: 5L);

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: new Guid("2bddc474-2779-4bb3-a73d-df90cd8f4c24"),
                column: "Code",
                value: 14L);

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"),
                column: "Code",
                value: 10L);

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"),
                column: "Code",
                value: 12L);

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"),
                column: "Code",
                value: 11L);

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: new Guid("cde133c3-0e33-4bcb-bee0-cc7d93ae3cb9"),
                column: "Code",
                value: 13L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Classes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Grades",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("Relational:ColumnOrder", 1)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Classes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("Relational:ColumnOrder", 2)
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<Guid>(
                name: "GradeId",
                table: "Classes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 1)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("1a9c42c2-148b-4da9-8620-a4b70336f451"),
                column: "Name",
                value: "י\"ב 09");

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("46c5b044-5020-4f7f-900c-93623be8bbaf"),
                column: "Name",
                value: "י\"ב 06");

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("7a9808bd-2d4e-48e2-8692-67d44ef962ce"),
                column: "Name",
                value: "י\"ב 03");

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("8c70d35a-9a3a-4bc6-8412-778f72338c63"),
                column: "Name",
                value: "י\"ב 04");

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("91b2c738-8e6c-4334-bc20-9778ef792af4"),
                column: "Name",
                value: "י\"ב 07");

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("ca5695ed-4d8a-485f-9072-29dcbc29bf84"),
                column: "Name",
                value: "י\"ב 01");

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("d22d38b9-0398-4aae-8ea7-a13237cace64"),
                column: "Name",
                value: "י\"ב 08");

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("ebbb2b2e-4833-4860-8843-40f9cf852be7"),
                column: "Name",
                value: "י\"ב 02");

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("f48dd2f8-9a66-4052-b31c-793e60df5004"),
                column: "Name",
                value: "י\"ב 05");
        }
    }
}
