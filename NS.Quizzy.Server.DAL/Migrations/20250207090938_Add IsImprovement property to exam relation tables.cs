using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsImprovementpropertytoexamrelationtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsImprovement",
                table: "GradeExams",
                type: "bit",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AddColumn<bool>(
                name: "IsImprovement",
                table: "ClassExams",
                type: "bit",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsImprovement",
                table: "GradeExams");

            migrationBuilder.DropColumn(
                name: "IsImprovement",
                table: "ClassExams");
        }
    }
}
