CREATE TABLE [dbo].[JobStatus]
(
	[JobStatusId] INT NOT NULL PRIMARY KEY, 
    [JobStatusName] NVARCHAR(20) NOT NULL, 
    [Description] NVARCHAR(200) NULL
)
