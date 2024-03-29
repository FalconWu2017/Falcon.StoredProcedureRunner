--SqlServer测试用存储过程
USE [test]
GO
/****** Object:  StoredProcedure [dbo].[TestSp1]    Script Date: 12/04/2021 12:37:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
-- Author:		falcon
-- Create date: 2020年12月13日
-- Description:	测试用存储
exec [dbo].[TestSp1] 1,2
-- =============================================*/
ALTER PROCEDURE [dbo].[TestSp1] 
	-- Add the parameters for the stored procedure here
	@p1 int = 0, 
	@p2 int = 0,
	@p3 VARCHAR(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT 1 Id,@p3+'1' s,@p1+@p2 jia,@p1-@p2 jian,@p1*@p2 chen,CAST(@p1 AS FLOAT)/@p2 chu
    UNION ALL
    SELECT 2 Id,@p3+'1' s,@p1+@p2 jia,@p1-@p2 jian,@p1*@p2 chen,CAST(@p1 AS FLOAT)/@p2 chu


END
