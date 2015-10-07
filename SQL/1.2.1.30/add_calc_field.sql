IF COL_LENGTH('DynEntityAttribConfig','CalculationFormula') IS NULL
BEGIN
	ALTER TABLE [dbo].[DynEntityAttribConfig]
	ADD [CalculationFormula] [nchar](1000) NULL
END
IF COL_LENGTH('DynEntityAttribConfig','IsCalcValueEditable') IS NOT NULL
BEGIN
	ALTER TABLE [dbo].[DynEntityAttribConfig]
	DROP [IsCalcValueEditable]
END

DROP TABLE [dbo].[ScreenFormula]
CREATE TABLE [dbo].[ScreenFormula](
	[Uid] [bigint] IDENTITY(1,1) NOT NULL,
	[AttributePanelUid] [bigint] NOT NULL,
	[FormulaText] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_ScreenFormula] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]