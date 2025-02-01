using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitializationDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NewId())"),
                    IsSecured = table.Column<bool>(type: "bit", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Target = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamTypes",
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
                    table.PrimaryKey("PK_ExamTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NewId())"),
                    Code = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
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
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NewId())"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Student"),
                    TwoFactorSecretKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NewId())"),
                    Code = table.Column<long>(type: "bigint", nullable: false),
                    GradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questionnaires",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NewId())"),
                    Code = table.Column<long>(type: "bigint", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    DurationWithExtra = table.Column<TimeSpan>(type: "time", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questionnaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questionnaires_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NewId())"),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    DurationWithExtra = table.Column<TimeSpan>(type: "time", nullable: false),
                    ExamTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exams_ExamTypes_ExamTypeId",
                        column: x => x.ExamTypeId,
                        principalTable: "ExamTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exams_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassExams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NewId())"),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassExams_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassExams_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GradeExams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(NewId())"),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(SYSDATETIMEOFFSET())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradeExams_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GradeExams_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "IsSecured", "Key", "Target", "Value", "ValueType" },
                values: new object[] { new Guid("795b0b48-4238-4d27-be60-3dd2cb66e424"), false, "SavePasswordOnRememberMe", "Client", "true", "Boolean" });

            migrationBuilder.InsertData(
                table: "ExamTypes",
                columns: new[] { "Id", "ItemOrder", "Name" },
                values: new object[,]
                {
                    { new Guid("095ce522-087b-48c1-83ee-35541ee672f6"), 2, "מתכונת I" },
                    { new Guid("2e50b781-6dba-4f05-8888-3b16ef618b0b"), 3, "מתכונת II" },
                    { new Guid("9b936387-a52d-4256-8f44-c28f75fd15d5"), 6, "סימולציה" },
                    { new Guid("9ecb8ecd-4314-4a67-a843-756cdbc49296"), 4, "מתכונת III" },
                    { new Guid("dbdd71f4-d784-4ecd-b013-1d81a07c79ab"), 1, "מתכונת" },
                    { new Guid("fa73c215-82d0-4bcd-be22-b0eabb950315"), 5, "בגרות" }
                });

            migrationBuilder.InsertData(
                table: "Grades",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("2bddc474-2779-4bb3-a73d-df90cd8f4c24"), 14L, "שכבה י\"ד" },
                    { new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), 10L, "שכבה י'" },
                    { new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), 12L, "שכבה י\"ב" },
                    { new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), 11L, "שכבה י\"א" },
                    { new Guid("cde133c3-0e33-4bcb-bee0-cc7d93ae3cb9"), 13L, "שכבה י\"ג" }
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "ItemOrder", "Name" },
                values: new object[,]
                {
                    { new Guid("0157322f-40f3-4e07-b65a-ebc1b408e644"), 19, "חינוך תעבורתי" },
                    { new Guid("1adfd42f-aac6-4ca5-8458-cfeb2fd42903"), 16, "מערכות חשמל" },
                    { new Guid("2f51a0af-bbce-4292-98f6-283628079f26"), 3, "אנגלית" },
                    { new Guid("30c570b2-e338-4505-ab1a-480f1776f9b8"), 5, "אזרחות" },
                    { new Guid("4e092ba1-95f8-4df3-a8b8-34d75175f0a0"), 4, "מתמטיקה" },
                    { new Guid("4e5a656f-13dc-4ce7-8504-cf67954c95ae"), 15, "מדעי ההנדסה" },
                    { new Guid("55f0178a-8339-47a2-ae3f-4c2b859fd52d"), 6, "היסטוריה" },
                    { new Guid("5bd83139-bbdf-4d76-a289-baac22ca831c"), 11, "מדעי הסביבה" },
                    { new Guid("81e4f556-c783-4e66-a816-751c3765e501"), 9, "ביולוגיה" },
                    { new Guid("8659e673-60e2-4fe4-9d83-c3df37fa5b96"), 13, "מדעי המחשב" },
                    { new Guid("964e0673-821e-4f31-a29d-c9f587f8fb02"), 14, "פיזיקה" },
                    { new Guid("9671ed50-624e-4a3b-9e25-b22d4cbdd09d"), 10, "מדעי הבריאות" },
                    { new Guid("9b8f8b4e-19fe-4f98-a9e1-0234d8331f24"), 1, "ערבית" },
                    { new Guid("b91976cb-943a-4bd7-8177-508c72d0fb06"), 7, "דת האיסלם" },
                    { new Guid("c21fe042-8ac9-4832-8e5b-04bb7da03eed"), 2, "עברית" },
                    { new Guid("d506c818-3300-4783-bb2e-e7005eef269f"), 17, "הנדסאים" },
                    { new Guid("ebfa96fb-150d-49d3-a688-bfa1c8035014"), 12, "תקשוב" },
                    { new Guid("eff998d1-eb15-4c10-bd82-e7b861e71923"), 18, "טכנאים" },
                    { new Guid("f74c6ff8-47b1-4383-a932-735add064955"), 8, "כימיה" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role", "TwoFactorSecretKey" },
                values: new object[] { new Guid("2325b8ae-f12a-43d8-be46-7041e57c9283"), "saji.nashef@gmail.com", "Saji Nashef", "jXg9GRdykHODQX8Kzxs8kiiZ92sfPDa3kH+lHmwHq+Q=", "Admin", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "IsDeleted", "Password", "Role", "TwoFactorSecretKey" },
                values: new object[] { new Guid("4320e74b-5821-4a30-94b7-cc88dddc45ee"), "Nashef.Systems@Gmail.com", "System", true, "yYZo4vAwJn7BtJ1x3MvbIHoQul3eoSduPbhS+pkZL/8=", "SuperAdmin", "97F1AFCD316343B4B2D492A10B036680" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role", "TwoFactorSecretKey" },
                values: new object[] { new Guid("b900d543-90ab-4e7a-83ba-b961918dcc8c"), "Nashef90@Gmail.com", "Admin", "j9Q7vYJckIupzSmvlKwiPg==", "Developer", "XD2GB3DYXAGZGGOXG46TD3QKBQXQYYKO" });

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "Id", "Code", "GradeId", "Name" },
                values: new object[,]
                {
                    { new Guid("081057a2-16df-4e2b-9e90-0cdc0dfff8f9"), 9L, new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), "י\"א 9" },
                    { new Guid("167ce96a-a9f0-4910-862a-19bc0bcdf820"), 3L, new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), "י' 3" },
                    { new Guid("1a9c42c2-148b-4da9-8620-a4b70336f451"), 9L, new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), "י\"ב 9" },
                    { new Guid("21e2f08f-9793-40aa-98a0-bbf3a8759e40"), 4L, new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), "י\"א 4" },
                    { new Guid("2a3c8f01-ecc2-42c0-8251-16fcd68944f5"), 11L, new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), "י\"א 11" },
                    { new Guid("36b9d045-ac87-4c29-99af-ccc4755b13cc"), 6L, new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), "י\"א 6" },
                    { new Guid("46c5b044-5020-4f7f-900c-93623be8bbaf"), 6L, new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), "י\"ב 6" },
                    { new Guid("49211014-42b9-4486-b6a3-fb9cc7e57426"), 7L, new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), "י\"א 7" },
                    { new Guid("494920f9-f6d9-405c-9536-afe619749ccd"), 1L, new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), "י\"א 1" },
                    { new Guid("5b107d79-7df7-4f6d-8bd4-608230492e07"), 9L, new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), "י' 9" },
                    { new Guid("782cb34d-7091-4e9e-9bda-c30d11e1a64d"), 5L, new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), "י\"א 5" },
                    { new Guid("7a9808bd-2d4e-48e2-8692-67d44ef962ce"), 3L, new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), "י\"ב 3" },
                    { new Guid("7d88b6fe-f3f3-4282-b475-82b2908e9fe0"), 10L, new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), "י' 10" },
                    { new Guid("859c1031-c539-4deb-adff-93bcfec85774"), 11L, new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), "י' 11" },
                    { new Guid("8c70d35a-9a3a-4bc6-8412-778f72338c63"), 4L, new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), "י\"ב 4" },
                    { new Guid("8e1f2e5e-7597-494f-bde9-5b290918c202"), 7L, new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), "י' 7" },
                    { new Guid("91b2c738-8e6c-4334-bc20-9778ef792af4"), 7L, new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), "י\"ב 7" },
                    { new Guid("a238c990-8fef-4e94-8bf4-b1e9dc818dde"), 11L, new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), "י\"ב 11" },
                    { new Guid("b096c5e7-76c6-405c-a044-5fbdbe222a3c"), 8L, new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), "י' 8" },
                    { new Guid("b641e55e-5a1e-42e9-8acb-1356340c87f5"), 10L, new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), "י\"א 10" },
                    { new Guid("c3424585-63a2-4e52-940a-b8979557b6ce"), 2L, new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), "י\"א 2" },
                    { new Guid("c3f075c7-93ce-4122-ab7c-b989cf8e6786"), 3L, new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), "י\"א 3" },
                    { new Guid("ca5695ed-4d8a-485f-9072-29dcbc29bf84"), 1L, new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), "י\"ב 1" },
                    { new Guid("cbea4226-569b-4ec0-a330-26e2094fe2a7"), 4L, new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), "י' 4" },
                    { new Guid("ce0bd2ff-3444-435b-a17d-03ae94936904"), 2L, new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), "י' 2" },
                    { new Guid("d22d38b9-0398-4aae-8ea7-a13237cace64"), 8L, new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), "י\"ב 8" },
                    { new Guid("d514ef35-7024-4836-be29-02e5f5b3bbcb"), 8L, new Guid("bd904a61-1129-42fb-aab4-a6b7c3dbc037"), "י\"א 8" },
                    { new Guid("e5928d27-abbf-49b9-a34b-0329b754cd76"), 1L, new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), "י' 1" },
                    { new Guid("ebb4d04c-e099-48c8-ae3c-b123354e250c"), 10L, new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), "י\"ב 10" },
                    { new Guid("ebbb2b2e-4833-4860-8843-40f9cf852be7"), 2L, new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), "י\"ב 2" },
                    { new Guid("f093a8a6-91d2-46f4-bc8d-42a489fc75f6"), 6L, new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), "י' 6" },
                    { new Guid("f48dd2f8-9a66-4052-b31c-793e60df5004"), 5L, new Guid("9c9f2aac-66e7-4ac0-a407-23698cced44a"), "י\"ב 5" },
                    { new Guid("f6845989-9704-4a39-a5d2-31f6a0df9cb3"), 5L, new Guid("438ec849-71cb-46b0-93b6-0147b85c5564"), "י' 5" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Key",
                table: "AppSettings",
                column: "Key",
                unique: true,
                filter: "IsDeleted = '0'");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_GradeId",
                table: "Classes",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_Name_GradeId",
                table: "Classes",
                columns: new[] { "Name", "GradeId" },
                unique: true,
                filter: "IsDeleted = '0'");

            migrationBuilder.CreateIndex(
                name: "IX_ClassExams_ClassId",
                table: "ClassExams",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassExams_ExamId",
                table: "ClassExams",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ExamTypeId",
                table: "Exams",
                column: "ExamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_QuestionnaireId",
                table: "Exams",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTypes_Name",
                table: "ExamTypes",
                column: "Name",
                unique: true,
                filter: "IsDeleted = '0'");

            migrationBuilder.CreateIndex(
                name: "IX_GradeExams_ExamId",
                table: "GradeExams",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_GradeExams_GradeId",
                table: "GradeExams",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_Name",
                table: "Grades",
                column: "Name",
                unique: true,
                filter: "IsDeleted = '0'");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_Code",
                table: "Questionnaires",
                column: "Code",
                unique: true,
                filter: "IsDeleted = '0'");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_SubjectId",
                table: "Questionnaires",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Name",
                table: "Subjects",
                column: "Name",
                unique: true,
                filter: "IsDeleted = '0'");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "IsDeleted = '0'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "ClassExams");

            migrationBuilder.DropTable(
                name: "GradeExams");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "ExamTypes");

            migrationBuilder.DropTable(
                name: "Questionnaires");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
