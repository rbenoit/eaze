CREATE TABLE [dbo].[Job]
(
	[RowId] BIGINT NOT NULL IDENTITY(1,1), 
    [JobId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED, 
    [JobType] INT NOT NULL, 
    [JobStatus] INT NOT NULL, 
	[JobCreated] DATETIME2 NULL,
	[CreatedBy] NVARCHAR(50) NOT NULL,
    [ProcessingPickUp] DATETIME2 NULL, 
    [ProcessingComplete] DATETIME2 NULL, 
    [ExecutionElapsed] BIGINT NULL, 
    [ProcessorKey] NVARCHAR(50) NULL, 
    [RetryCount] INT NOT NULL DEFAULT 0, 
    [ErrorInformation] NVARCHAR(MAX) NULL,

	CONSTRAINT [CIX_Job_RowId] UNIQUE CLUSTERED ([RowId]),

	CONSTRAINT [FK_Job_JobStatus] FOREIGN KEY ([JobStatus]) REFERENCES [dbo].[JobStatus] ([JobStatusId]),

	CONSTRAINT [FK_Job_JobType] FOREIGN KEY ([JobType]) REFERENCES [dbo].[JobType] ([JobTypeId])
)
