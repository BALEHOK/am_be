/****** Object:  UserDefinedFunction [dbo].[f_SplitString]    Script Date: 12.08.2012 1:22:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Ilya Bolkhovsky
-- Create date: 11.08.2012
-- Description:	Splits space-separated string into list of values
-- =============================================
CREATE FUNCTION [dbo].[f_SplitString]
(	
	@list nvarchar(MAX)
)
RETURNS @tbl TABLE (string nvarchar(50) NOT NULL)
AS
BEGIN
   DECLARE @pos        int,
           @nextpos    int,
           @valuelen   int,
		   @string     nvarchar(50)

   SELECT @pos = 0, @nextpos = 1

   WHILE @nextpos > 0
   BEGIN
      SELECT @nextpos = charindex(' ', @list, @pos + 1)
      SELECT @valuelen = CASE WHEN @nextpos > 0
                              THEN @nextpos
                              ELSE len(@list) + 1
                         END - @pos - 1;
	  SET @string = RTRIM(LTRIM(substring(@list, @pos + 1, @valuelen)));
	  IF LEN(@string) > 0
		INSERT @tbl (string) VALUES (@string);         
      SELECT @pos = @nextpos
   END
   RETURN
END
GO

