CREATE VIEW [dbo].[V_USER_LOGIN_STATUS] 
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
GO