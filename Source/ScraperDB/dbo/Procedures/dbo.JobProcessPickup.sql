CREATE PROCEDURE [dbo].[JobProcessPickup]
	@ProcessorKey nvarchar(50),
	@MaximumRows int = 10,
	@JobType int = NULL
AS
	
	DECLARE @JobProcessResults TABLE ( [JobId] UNIQUEIDENTIFIER, [JobType] INT, [RetryCount] INT ) 

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	BEGIN TRANSACTION
	
	UPDATE [dbo].[Job] SET [ProcessingPickUp] = GETUTCDATE(), [JobStatus] = 1 /*Processing*/, [ProcessorKey] = @ProcessorKey
		OUTPUT [Inserted].[JobId], [Inserted].[JobType], [Inserted].[RetryCount] INTO @JobProcessResults
	FROM (SELECT TOP(@MaximumRows) [JobId] FROM [dbo].[Job] 
		WHERE [JobStatus] = 0 /*Ready*/ AND [JobType] = ISNULL(@JobType, [JobType])
		ORDER BY [RowId] DESC) AS j
	WHERE [Job].[JobId] = j.[JobId]	

	COMMIT TRANSACTION
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	SELECT [JobId], [JobType], [RetryCount]
	FROM @JobProcessResults


RETURN 0
