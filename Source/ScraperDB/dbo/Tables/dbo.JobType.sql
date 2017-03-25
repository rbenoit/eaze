CREATE TABLE [dbo].[JobType]
(
	[JobTypeId] INT NOT NULL PRIMARY KEY, 
    [JobTypeName] NVARCHAR(20) NOT NULL, 
    [Description] NVARCHAR(200) NULL
)
