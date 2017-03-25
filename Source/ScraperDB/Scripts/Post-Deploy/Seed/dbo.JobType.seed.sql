MERGE INTO [dbo].[JobType] AS Target  
USING ( VALUES (0, 'None', 'A no-op job type that can be used for testing.'),
		(1, 'WebScrape', 'A job that scrapes a url for a response.')
) AS Source (JobTypeId, JobTypeName, Description)
ON (target.JobTypeId = source.JobTypeId)  
WHEN MATCHED 
    THEN UPDATE SET Target.JobTypeName = Source.JobTypeName, Target.Description = Source.Description
WHEN NOT MATCHED BY Target
    THEN INSERT ( [JobTypeId], [JobTypeName], [Description] ) VALUES ( [JobTypeId], [JobTypeName], [Description] )
WHEN NOT MATCHED BY Source
	THEN DELETE;
GO  
