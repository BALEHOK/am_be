UPDATE DataType SET DBDataType='Float' WHERE DataTypeUid=49;
UPDATE DataType SET DBDataType='BigInt' WHERE DBDataType='bigint';
UPDATE DataType SET DBDataType='Int' WHERE DBDataType='int';
UPDATE DataType SET DBDataType='NVarChar' WHERE DBDataType='nvarchar';
UPDATE DataType SET DBDataType='Money' WHERE DBDataType='money';

UPDATE DataType SET FrameworkDataType='System.String' WHERE FrameworkDataType='string';
UPDATE DataType SET FrameworkDataType='System.Single' WHERE FrameworkDataType='float';
UPDATE DataType SET FrameworkDataType='System.Boolean' WHERE FrameworkDataType='bool';
UPDATE DataType SET FrameworkDataType='System.Char' WHERE FrameworkDataType='char';
UPDATE DataType SET FrameworkDataType='System.DateTime' WHERE FrameworkDataType='DateTime';
UPDATE DataType SET FrameworkDataType='System.Decimal' WHERE FrameworkDataType='decimal';
UPDATE DataType SET FrameworkDataType='System.Int32' WHERE FrameworkDataType='int';
UPDATE DataType SET FrameworkDataType='System.Int64' WHERE FrameworkDataType='Int64';
UPDATE DataType SET FrameworkDataType='System.Int64' WHERE FrameworkDataType='long';
UPDATE DataType SET FrameworkDataType='System.SByte' WHERE FrameworkDataType='bit';
UPDATE DataType SET FrameworkDataType='AppFramework.Core.Classes.AssetType' WHERE FrameworkDataType='AssetType';
UPDATE DataType SET FrameworkDataType='System.Int64' WHERE FrameworkDataType='AppFramework.Core.Classes.AssetType';