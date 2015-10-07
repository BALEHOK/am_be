IF EXISTS (SELECT * FROM dbo.sysfulltextcatalogs WHERE NAME = 'IndexEntitiesCatalog')  
BEGIN	
	EXEC sp_fulltext_table 'IndexHistoryDynEntities', 'drop'
	EXEC sp_fulltext_table 'IndexActiveDynEntities', 'drop'
	EXEC sp_fulltext_catalog 'IndexEntitiesCatalog', 'drop'  
END
  
-- Create the catalog if it doesn't already exist.  
IF NOT EXISTS (SELECT * FROM dbo.sysfulltextcatalogs WHERE NAME = 'IndexEntitiesCatalog')  
EXEC sp_fulltext_catalog 'IndexEntitiesCatalog', 'create'  
  
-- Add the full text index to the table   
EXEC sp_fulltext_table '[dbo].[IndexActiveDynEntities]', 'create', 'IndexEntitiesCatalog', 'IndexActiveDynEntities_PK'  
EXEC sp_fulltext_table '[dbo].[IndexHistoryDynEntities]', 'create', 'IndexEntitiesCatalog', 'IndexHistoryDynEntities_PK'  
  
-- Add the columns to the full text index   
EXEC sp_fulltext_column '[dbo].[IndexActiveDynEntities]', 'Name', 'add'   
EXEC sp_fulltext_column '[dbo].[IndexActiveDynEntities]', 'Description', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexActiveDynEntities]', 'Keywords', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexActiveDynEntities]', 'EntityConfigKeywords', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexActiveDynEntities]', 'AllAttrib2IndexValues', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexActiveDynEntities]', 'AllContextAttribValues', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexActiveDynEntities]', 'AllAttribValues', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexActiveDynEntities]', 'CategoryKeywords', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexActiveDynEntities]', 'TaxonomyKeywords', 'add'  

EXEC sp_fulltext_column '[dbo].[IndexHistoryDynEntities]', 'Name', 'add'   
EXEC sp_fulltext_column '[dbo].[IndexHistoryDynEntities]', 'Description', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexHistoryDynEntities]', 'Keywords', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexHistoryDynEntities]', 'EntityConfigKeywords', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexHistoryDynEntities]', 'AllAttrib2IndexValues', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexHistoryDynEntities]', 'AllContextAttribValues', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexHistoryDynEntities]', 'AllAttribValues', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexHistoryDynEntities]', 'CategoryKeywords', 'add'  
EXEC sp_fulltext_column '[dbo].[IndexHistoryDynEntities]', 'TaxonomyKeywords', 'add'   

-- Activate the index  
EXEC sp_fulltext_table '[dbo].[IndexActiveDynEntities]', 'activate'   
EXEC sp_fulltext_table '[dbo].[IndexHistoryDynEntities]', 'activate'   
  
-- Start population  
EXEC sp_fulltext_catalog   'IndexEntitiesCatalog', 'start_full'  
