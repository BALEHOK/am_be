IF COL_LENGTH('DynEntityAttribConfig','CalculationFormula') IS NULL
BEGIN
	ALTER TABLE [dbo].[DynEntityAttribConfig]
	ADD [CalculationFormula] [nchar](1000) NULL
END
IF COL_LENGTH('DynEntityAttribConfig','IsCalcValueEditable') IS NULL
BEGIN
	ALTER TABLE [dbo].[DynEntityAttribConfig]
	ADD [IsCalcValueEditable] [bit] NULL
END