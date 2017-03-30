Greetings Eaze developers -

I thought a few quick notes might explain a bit about my solution to this project ahead of any further discussion:

1) While I was tempted to explore a non DB centric solution, concurrency in the job processing aspect led me to develop with what I was more comfortable with. So this system does use a Sql DB to hold job and processing state - a serializable transaction scope is used so that no matter how many threads are polling for jobs to pick up, no jobs will get picked up multiple times. With too many requesting threads deadlocks are a possibility, and would need to be handled appropriately (or a better / different solution investigated of course).

2) I got excited about making the whole system ultimately extensible for multiple job types, so there is a fair amount of work done in preparation for that.  The only two job types supported by the business layer are No-op jobs for testing and WebScrape jobs.  The API is built primarily around WebScrape jobs, so I didn't include a POST option for No-Op jobs.  The Get endpoint (no id, list view) will return all job types however.  The Get endpoint (specific id) will only return a WebScrape job.

3) API: 
The Get endpoint which returns a list would quickly return far too much data. So I added severeal filter parameters:
 - Type: 'WebScrape', 'None'
 - Status: 'Ready', 'Completed', 'Cancelled', 'Error', 'Processing'
 - CreatedBy: (Value passed in as 'requestedBy' in POST package)
 - CreatedStart / CreatedEnd: UTC date range for when the jobs were requested
 - PageSize: Maximum number of items per page
 - PageIndex: Which page index you are requesting (requesting past the number of available items returns an empty set)

4) Testing: 
I am a big fan of automated testing, and while I don't always follow test driven development to the letter I believe this solution shows the type of testing I'd be looking at in a real world application. There are gaps, notably around the service which was the last piece to be completed. 
 - Integration tests: These tests work against the concrete SQL implemenations of the DAO objects
 - Unit tests: These tests inject mock DAO and never touch the DB

5) Service: 
Most places I've worked with have rolled their own task schedular / job schedular service from scratch. I hadn't used Quartz before but was able to make it work - it's definitely something I'll play around with at home more.  There is no testing of the service in the test project due to time constraints.
 - Service is set up to be installed as a Windows Service, but I did not include the installer due to time constraints
 - Service has 3 app settings that can be adjusted to try different polling / max jobs / intervals
 - To run service in console mode execute with argument: console and then hit [Enter] to stop
  >Interview.Green.Web.Scrapper.Service.exe console

6) Database: 
The database project included should work - point to any sql server instance or local express, and publish. Publish will set up the DB options, deploy all schema, and merge the seed data. The web configs are set up for the following databases:
ScraperDB (Service and API point to this one)
ScraperDB-Test (Test project points to this ont)

IMPORTANT! It's not recommended to point the Test Project to a running DB as each test cancels out any Ready or Processing jobs.

That's about it, I've been running the solution locally and testing with some bad urls and valid ones, and everything looks in order here.  I look forward to discussing anything else in person.

Thanks,
Randall Benoit
(407) 729-2627
randall.benoit@gmail.com