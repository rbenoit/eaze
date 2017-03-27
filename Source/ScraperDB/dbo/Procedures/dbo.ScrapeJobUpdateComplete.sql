CREATE PROCEDURE [dbo].[ScrapeJobUpdateComplete]
	@JobId uniqueidentifier,
	@ExecutionElapsed bigint,
	@HttpStatusCode int,
	@ResponseRaw nvarchar(MAX)
AS
	
	UPDATE [dbo].[Job] SET [JobStatus] = 2 /* Completed */, ProcessingComplete = GETUTCDATE(), [ExecutionElapsed] = @ExecutionElapsed
	WHERE [JobId] = @JobId

	UPDATE [dbo].[ScraperJob] SET [HttpStatusCode] = @HttpStatusCode, [ResponseRaw] = @ResponseRaw 
	WHERE [JobId] = @JobId

RETURN 0
