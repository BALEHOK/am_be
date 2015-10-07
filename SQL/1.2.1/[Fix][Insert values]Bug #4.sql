DELETE FROM [ScreenLayout]
      WHERE ScreenLayoutName='TwoColumns'
GO

INSERT INTO [dbo].[ScreenLayout]
           ([ScreenLayoutName]
           ,[ScreenLayoutDescription]
           ,[ImageName]
           ,[CssName])
     VALUES
           ('TwoColumnsWideLeftAndFooter'
           ,'Two columns, the left column is wider then the right, and footer'
           ,'/images/layouts/layout7.png'
           ,'/css/layouts/layout7.css')
GO

INSERT INTO [dbo].[ScreenLayout]
           ([ScreenLayoutName]
           ,[ScreenLayoutDescription]
           ,[ImageName]
           ,[CssName])
     VALUES
           ('TwoColumnsWideRightAndFooter'
           ,'Two columns, the right column is wider then the left,and footer'
           ,'/images/layouts/layout8.png'
           ,'/css/layouts/layout8.css')


