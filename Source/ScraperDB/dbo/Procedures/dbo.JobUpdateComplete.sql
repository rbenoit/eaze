CREATE PROCEDURE [dbo].[JobUpdateComplete]
	@JobId uniqueidentifier,
	@ExecutionElapsed bigint
AS
	
	UPDATE [dbo].[Job] SET [JobStatus] = 2 /* Completed */, ProcessingComplete = GETUTCDATE(), [ExecutionElapsed] = @ExecutionElapsed
	WHERE [JobId] = @JobId

RETURN 0
