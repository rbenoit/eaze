MERGE INTO [dbo].[JobStatus] AS Target  
USING ( VALUES (0, 'Ready', 'Default status, job has been created and needs processing.'),
		(1, 'Processing', 'Job has been picked up for processing.'),
		(2, 'Completed', 'Job has completed processing successfully.'),
		(3, 'Cancelled', 'Job has been cancelled and will not be processed.'),
		(4, 'Error', 'Job has encountered an error and will not be processed.')
) AS Source (JobStatusId, JobStatusName, Description)
ON (target.JobStatusId = source.JobStatusId)  
WHEN MATCHED 
    THEN UPDATE SET Target.JobStatusName = Source.JobStatusName, Target.Description = Source.Description
WHEN NOT MATCHED BY Target
    THEN INSERT ( [JobStatusId], [JobStatusName], [Description] ) VALUES ( [JobStatusId], [JobStatusName], [Description] )
WHEN NOT MATCHED BY Source
	THEN DELETE;
GO  
