CREATE TABLE [dbo].[Document]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DocumentName] VARCHAR(MAX) NOT NULL, 
    [Location] VARCHAR(MAX) NOT NULL, 
    [Size] INT NOT NULL, 
    [Data] VARBINARY(MAX) NOT NULL 

)
