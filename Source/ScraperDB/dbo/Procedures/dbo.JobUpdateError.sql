CREATE PROCEDURE [dbo].[JobUpdateError]
	@JobId uniqueidentifier,
	@ErrorInformation nvarchar(MAX)
AS

	UPDATE [dbo].[Job] SET [JobStatus] = 4 /* Error */, [ErrorInformation] = @ErrorInformation WHERE [JobId] = @JobId

RETURN 0
