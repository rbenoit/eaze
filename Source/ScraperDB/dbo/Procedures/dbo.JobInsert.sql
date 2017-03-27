CREATE PROCEDURE [dbo].[JobInsert]
	@JobId uniqueidentifier,
	@CreatedBy nvarchar(50),
	@JobType int = 0
AS
	
	INSERT INTO [dbo].[Job] ( [JobId], [JobCreated], [CreatedBy], [JobStatus], [JobType] )
	VALUES ( @JobId, GETUTCDATE(), @CreatedBy, 0 /* Ready */, @JobType )

RETURN 0
