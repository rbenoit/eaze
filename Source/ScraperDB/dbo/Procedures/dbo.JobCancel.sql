CREATE PROCEDURE [dbo].[JobCancel]
	@JobId uniqueidentifier,
	@Result int = NULL OUTPUT 
AS
	
	UPDATE [dbo].[Job] SET [JobStatus] = 3 /*Cancelled*/ WHERE [JobId] = @JobId AND [JobStatus] = 0 /* Ready */

	IF (@@ROWCOUNT = 0)
		SET @Result = 1
	ELSE
		SET @Result = 0

RETURN 0
