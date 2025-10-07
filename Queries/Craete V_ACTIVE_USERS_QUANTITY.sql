CREATE VIEW [dbo].[V_ACTIVE_USERS_QUANTITY]
AS
	SELECT
		YEAR(CreatedTime) AS 'Year', 
		MONTH(CreatedTime) AS 'Month', 
		DAY(CreatedTime) AS 'Day', 
		COUNT(DISTINCT UserId) AS Quantity
	FROM dbo.LoginHistory
	WHERE CreatedTime > GETDATE() - 14
	GROUP BY YEAR(CreatedTime), MONTH(CreatedTime), DAY(CreatedTime)
GO