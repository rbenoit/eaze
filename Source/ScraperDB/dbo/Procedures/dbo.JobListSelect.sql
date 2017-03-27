CREATE PROCEDURE [dbo].[JobListSelect]
	@JobStatus int = NULL,
	@JobType int = NULL,
	@CreatedBy nvarchar(50) = NULL,
	@JobCreatedStart datetime2 = NULL,
	@JobCreatedEnd datetime2 = NULL,
	@PageSize int = 10,
	@PageIndex int = 0
AS
	
	/* There are several ways to handle multiple filters in a SELECT; I've gone a simple route for now, but this is a good case for parameterized dynamic sql, especially if we add a sort clause */
	SELECT [JobId], [JobType], [JobStatus], [JobCreated], [CreatedBy], [ProcessingPickUp], [ProcessingComplete], [ExecutionElapsed], [ProcessorKey], [RetryCount], [ErrorInformation]
	FROM [dbo].[Job]
	WHERE [JobType] = ISNULL(@JobType, [JobType]) 
		AND [JobStatus] = ISNULL(@JobStatus, [JobStatus])
		AND [JobCreated] >= ISNULL(@JobCreatedStart, [JobCreated]) AND [JobCreated] <= ISNULL(@JobCreatedEnd, [JobCreated])
		AND [CreatedBy] = ISNULL(@CreatedBy, [CreatedBy])
	ORDER BY [JobCreated]
	OFFSET (@PageSize * @PageIndex) ROWS FETCH NEXT @PageSize ROWS ONLY

RETURN 0
