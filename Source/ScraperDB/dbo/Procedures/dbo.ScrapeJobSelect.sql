CREATE PROCEDURE [dbo].[ScrapeJobSelect]
	@JobId uniqueidentifier
AS

	SELECT j.[JobId], j.[JobType], j.[JobStatus], j.[JobStatus], j.[JobCreated], j.[ProcessingPickUp], j.[ProcessingComplete], j.[ExecutionElapsed], j.[ProcessorKey], j.[RetryCount],
		sj.[Url], sj.[HttpStatusCode], sj.[ResponseRaw]
	FROM [dbo].[Job] j
	INNER JOIN [dbo].[ScraperJob] sj ON j.[JobId] = sj.[JobId]
	WHERE j.[JobId] = @JobId

RETURN 0
