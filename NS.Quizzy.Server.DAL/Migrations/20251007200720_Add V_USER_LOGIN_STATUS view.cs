using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NS.Quizzy.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddV_USER_LOGIN_STATUSview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
				CREATE VIEW [V_USER_LOGIN_STATUS] 
				AS
					WITH cInfo AS (
						SELECT 
							C.ID, 
							(G.Code*100+C.Code) AS FullCode
						FROM Classes C
						JOIN Grades G ON C.GradeId = G.ID
					),
					lastLoginInfo AS (
						SELECT 
							UserId, 
							MAX(CreatedTime) AS lastLogin 
						FROM LoginHistory
						GROUP BY UserId
					)
					SELECT 
						U.IdNumber,
						U.FullName,
						CASE 
							WHEN u.Role = 0 THEN N'תלמיד' 
							ELSE N'מורה' 
						END AS [Role],
						CI.FullCode AS Class, 
						LL.LastLogin,
						CASE  
							WHEN LL.LastLogin IS NULL THEN NULL 
							WHEN U.NotificationToken IS NULL THEN 'No' 
							ELSE 'Yes' 
						END AS [IsAllowNotifications]
					FROM Users u
					LEFT JOIN cInfo CI ON CI.Id = u.ClassId
					LEFT JOIN lastLoginInfo LL ON LL.UserId = u.ID
					WHERE u.[Role] <= 1 AND u.IsDeleted = 0;
          ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [V_USER_LOGIN_STATUS];");
        }
    }
}
