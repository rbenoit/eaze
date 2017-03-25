﻿CREATE PROCEDURE [dbo].[JobListSelect]
	@JobStatus int = NULL,
	@JobType int = NULL,
	@JobCreatedStart datetime2 = NULL,
	@JobCreatedEnd datetime2 = NULL,
	@PageSize int = 10,
	@PageIndex int = 0
AS
	
	/* There are several ways to handle multiple filters in a SELECT; I've gone a simple route for now, but this is a good case for parameterized dynamic sql, especially if we add a sort clause */
	SELECT [JobId], [JobType], [JobStatus], [JobStatus], [JobCreated], [ProcessingPickUp], [ProcessingComplete], [ExecutionElapsed], [ProcessorKey], [RetryCount]
	FROM [dbo].[Job]
	WHERE [JobType] = ISNULL(@JobType, [JobType]) AND [JobStatus] = ISNULL(@JobStatus, [JobStatus]) 
		AND [JobCreated] >= ISNULL(@JobCreatedStart, [JobCreated]) AND [JobCreated] <= ISNULL(@JobCreatedEnd, [JobCreated]) 
	ORDER BY [JobCreated]

RETURN 0