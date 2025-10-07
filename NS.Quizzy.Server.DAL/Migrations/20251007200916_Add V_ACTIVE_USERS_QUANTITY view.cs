using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddV_ACTIVE_USERS_QUANTITYview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
				CREATE VIEW [V_ACTIVE_USERS_QUANTITY]
                AS
	                SELECT
		                YEAR(CreatedTime) AS 'Year', 
		                MONTH(CreatedTime) AS 'Month', 
		                DAY(CreatedTime) AS 'Day', 
		                COUNT(DISTINCT UserId) AS Quantity
	                FROM dbo.LoginHistory
	                WHERE CreatedTime > GETDATE() - 30
	                GROUP BY YEAR(CreatedTime), MONTH(CreatedTime), DAY(CreatedTime);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [V_ACTIVE_USERS_QUANTITY];");
        }
    }
}
