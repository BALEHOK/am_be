
/****** Object:  StoredProcedure [dbo].[_cust_AlterTable]    Script Date: 01.11.2012 18:19:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[_cust_AlterTable]
@table_name nvarchar (50),
@column_name nvarchar (50),
@column_datatype nvarchar (100),
@column_isnull bit,
@column_default nvarchar (50)
AS
BEGIN
	DECLARE @sqlScriptAdd nvarchar(2000);
	DECLARE @sqlScriptAlter nvarchar(2000);

	IF (EXISTS (SELECT * FROM  information_schema.TABLES 
				WHERE TABLE_NAME = @table_name))
	BEGIN	
		
		-- rename original column
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
			AND (column_name = @column_name OR column_name LIKE  @column_name + '_OLD%');
				
			SET @new_column_name = @column_name +'_OLD' + CAST(@columns_count AS varchar(3));
			EXEC sp_rename @full_table_name, @new_column_name, 'COLUMN';

			SET @sqlScriptAlter = ' ALTER TABLE ' + @table_name +  ' ALTER COLUMN ' + @new_column_name + ' ' + @column_datatype + ' NULL ';					  
			EXEC (@sqlScriptAlter);
		END			
				
		-- add new column
		DECLARE @nulldef varchar (20);
		IF @column_isnull = 1
		BEGIN
			SET @nulldef = ' NULL ';
		END
		ELSE			
		BEGIN
			SET @nulldef = ' NOT NULL ';
		END
		IF (LEN(@column_default) > 0)
		BEGIN			
			SET @sqlScriptAlter = ' ALTER TABLE ' + @table_name +  
								  ' ADD "' + @column_name + '" ' + @column_datatype + @nulldef +								   
								  ' DEFAULT ' + @column_default;
		END	
		ELSE
		BEGIN
			SET @sqlScriptAlter = ' ALTER TABLE ' + @table_name +  
								  '	ADD "' + @column_name + '" ' + @column_datatype  + @nulldef;
		END								  
		EXEC (@sqlScriptAlter)

	END
END
