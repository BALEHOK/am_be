SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[_cust_DisableColumn]
@table_name nvarchar (50),
@column_name nvarchar (50),
@DynEntityAttribConfigId bigint
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
			
			UPDATE [DynEntityAttribConfig] SET DBTableFieldname=@new_column_name WHERE DynEntityAttribConfigId=@DynEntityAttribConfigId;	

		END			
	END
END
