SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:    Ilya Bolkhovsky
-- Create date: 11.08.2012
-- Description: Returns permissions set for a specific user
-- =============================================
-- Modified:  Igor Pistolyaka
-- Create date: 29.04.2013
-- Description: Modified funtion to work with number of TaxonomyId
-- =============================================
CREATE FUNCTION [dbo].[f_GetGrantedSearchConfigIds]
( 
  @UserId bigint,
  @ConfigId bigint = NULL,
  @TaxonomyIds varchar(MAX) = NULL
)
RETURNS @ConfigIds TABLE 
(
  DynEntityConfigId bigint
)
AS
BEGIN

  INSERT @ConfigIds
   SELECT *
     FROM dbo.[f_GetGrantedConfigIds](@UserId, 8) -- all for reading (1000)
    WHERE (DynEntityConfigId = @ConfigId OR @ConfigId IS NULL)
    AND (DynEntityConfigId IN (SELECT DynEntityConfigId FROM DynEntityConfigTaxonomy WHERE (TaxonomyItemId in (select id from dbo.f_SplitIds(@TaxonomyIds)))) OR @TaxonomyIds IS NULL)
          
  RETURN 
END
GO

-- =============================================
-- Author:    Ilya Bolkhovsky
-- Create date: 11.08.2012
-- Description: Returns permissions set for a specific user
-- =============================================
ALTER FUNCTION [dbo].[f_GetGrantedSearchDepartmentIds]
( 
  @UserId bigint,
  @DepartmentId bigint = NULL 
)
RETURNS @DepartmentIds TABLE 
(
  DepartmentId bigint
)
AS
BEGIN
  WITH GrantedDepartmentIds AS
  (
  SELECT DISTINCT DepartmentId
    FROM Rights 
   WHERE UserId = @UserId 
     AND IsDeny = 0
     AND Rights1 >= 8   -- all for reading (1000)
     AND DepartmentId > 0
   EXCEPT
  SELECT DISTINCT DepartmentId
    FROM Rights 
   WHERE UserId = @UserId 
     AND IsDeny = 1
     AND Rights1 >= 8   -- all for reading (1000)
     AND DepartmentId > 0
  )
  INSERT @DepartmentIds
  SELECT * FROM GrantedDepartmentIds        
   WHERE DepartmentId = @DepartmentId OR @DepartmentId IS NULL
  
  RETURN 
END

