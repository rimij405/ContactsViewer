CREATE TABLE [dbo].[Recruiter]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FirstName] NVARCHAR(80) NOT NULL, 
    [MiddleName] NVARCHAR(80) NULL, 
    [LastName] NVARCHAR(80) NOT NULL, 
    [EmailAddress] NVARCHAR(80) NULL
)
