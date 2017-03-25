CREATE PROCEDURE [dbo].[ScrapeJobInsert]
	@JobId uniqueidentifier,
	@CreatedBy nvarchar(50),
	@Url nvarchar(1000)
AS

	INSERT INTO [dbo].[Job] ( [JobId], [JobCreated], [CreatedBy], [JobStatus], [JobType] )
	VALUES ( @JobId, GETUTCDATE(), @CreatedBy, 0 /* Ready */, 1 /* ScrapeJob */ )

	INSERT INTO [dbo].[ScraperJob] ( [JobId], [Url] ) VALUES ( @JobId, @Url )


RETURN 0
