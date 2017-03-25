CREATE PROCEDURE [dbo].[ScrapeJobInsert]
	@JobId uniqueidentifier,
	@Url nvarchar(1000)
AS

	INSERT INTO [dbo].[Job] ( [JobId], [JobCreated], [JobStatus], [JobType] )
	VALUES ( @JobId, GETUTCDATE(), 0 /* Ready */, 1 /* ScrapeJob */ )

	INSERT INTO [dbo].[ScraperJob] ( [JobId], [Url] ) VALUES ( @JobId, @Url )


RETURN 0
