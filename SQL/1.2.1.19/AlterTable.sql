/****** Object:  StoredProcedure [dbo].[_cust_AlterTable]    Script Date: 07/02/2012 18:55:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[_cust_AlterTable]
@table_name nvarchar (50),
@column_name nvarchar (50),
@column_description nvarchar (100),
@column_default nvarchar (50),
@isAdding bit
AS
BEGIN
	DECLARE @sqlScriptAdd nvarchar(2000);
	DECLARE @sqlScriptAlter nvarchar(2000);

	IF (EXISTS (SELECT * FROM  information_schema.TABLES 
				WHERE TABLE_NAME = @table_name))
	BEGIN	
		IF (@isAdding = 1)
		BEGIN	
			
			IF (LEN(@column_default) > 0)
			BEGIN
				SET @sqlScriptAdd = ' ALTER TABLE ' + @table_name + 
									' ADD "' + @column_name + '" ' + @column_description +
									' DEFAULT ' + @column_default;	
			END
			ELSE
			BEGIN
				SET @sqlScriptAdd = ' ALTER TABLE ' + @table_name + 
									' ADD "' + @column_name + '" ' + @column_description;							    
			END
											  
			IF (NOT EXISTS (SELECT * FROM  information_schema.COLUMNS 
							WHERE table_name = @table_name 
							AND column_name = @column_name ))
			BEGIN 		
				EXEC(@sqlScriptAdd);
			END
		END		
		ELSE
		BEGIN
			IF (EXISTS (SELECT * FROM  information_schema.COLUMNS 
						WHERE table_name = @table_name 
						AND column_name = @column_name))
			BEGIN 	
				DECLARE @full_table_name varchar(200);
				SET @full_table_name = @table_name + '.' + @column_name;
				
				DECLARE @new_column_name varchar(50);
				DECLARE @columns_count int;
				
				SELECT @columns_count = COUNT(*) FROM  information_schema.COLUMNS 
				WHERE table_name = @table_name 
				AND column_name = @column_name;
				
				SET @new_column_name = @column_name +'_OLD' + CAST(@columns_count AS varchar(3));
				EXEC sp_rename @full_table_name, @new_column_name, 'COLUMN';
			END			
				
			IF (LEN(@column_default) > 0)
			BEGIN			
				SET @sqlScriptAlter = ' ALTER TABLE ' + @table_name +  
									  '	ADD "' + @column_name + '" ' + @column_description +
									  ' DEFAULT ' + @column_default;
			END	
			ELSE
			BEGIN
				SET @sqlScriptAlter = ' ALTER TABLE ' + @table_name +  
									  '	ADD "' + @column_name + '" ' + @column_description;	
			END								  
			EXEC (@sqlScriptAlter)
		END 
	END
END
