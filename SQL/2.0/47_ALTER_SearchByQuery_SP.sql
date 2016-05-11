SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: Wouter Steegmans
-- Create date	: 05/01/2013
-- Modified		: 04/02/2016 Ilya Bolkhovsky
-- Description	: Checks for custom queries
-- =============================================
-- Overview custom queries
--	* OverviewSuspendedSOBDossiers
--  * OverviewSOBCertificatesZeroDays
--  * OverviewUnequalDaysSOBPaymentsCertificates
--  * OverviewSOBPaymentsZeroDays
--  * OverviewSOBDossierMembershipOK
--  * OverviewSOBDossierRunningSOB54
--  * OverviewSOBPaymentsNotLinked2Certificate
--  * OverviewSOBCertificatesWithoutPayment
--  * OverviewSOBRecentBatches
--  * OverviewBUBDossierMembershipOK
--  * OverviewBUBDossierSuspended
--  * OverviewBUBDossierRunningBUB5254
--  * OverviewBUBPaymentsNotLinked2Certificate
--  * OverviewBUBCertificatesWithoutPayment
--  * OverviewBUBRecentBatches
--  * OverviewBUBPaymentsCertificatesDiffDates
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchCustomQuery]
        @SearchId uniqueidentifier,
        -- Id of current user (required)
        @UserId bigint,
        -- Space-separated list of keywords (required parameter). Ex.: '"item*" AND "111*"'     
        @keywords nvarchar(1000),
        --  Space-separated list of ConfigIds - assettype (optional parameter)
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL,
        -- Search area
        @SearchBufferCount int = 0,
        @IsCustomQuery int OUTPUT
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;
        
        DECLARE @BUBDOSSIER_CONFIGID bigint = 6337;
		DECLARE @AccesLevel bigint = 8; -- read

		IF CHARINDEX('[', @keywords) = 0 RETURN

		-- SOB custom queries
		IF @keywords = '[OverviewSuspendedSOBDossiers]'
		BEGIN
		
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY ISNULL(Suspended_date, '1900-01-01'), Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, d.Suspended_Date, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 						
								INNER JOIN ADynEntitySOBDossier d ON FT_TBL.DynEntityUid = d.DynEntityUId 
								       AND FT_TBL.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynListValue lv ON lv.DynEntityConfigUid = d.DynEntityConfigUid 
								       AND lv.DynEntityAttribConfigUid = ac.DynEntityAttribConfigUid
								       AND lv.AssetUid = d.DynEntityUid
								INNER JOIN DynListItem li ON li.DynListItemUid = lv.DynListItemUid
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
							-- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
									d.ActiveVersion = 1
								AND ac.ActiveVersion = 1
								AND p.ActiveVersion = 1
								AND ac.DynEntityAttribConfigId = 7103
								AND li.Value = 'Suspended'
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights										
					) AS SearchResults
			)
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END
		
		IF @keywords = '[OverviewSOBCertificatesZeroDays]'
		BEGIN		
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY Trimester DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, cert.Trimester, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 							
								INNER JOIN ADynEntitySOBCertificate cert ON FT_TBL.DynEntityUid = cert.DynEntityUId 
									   AND FT_TBL.DynEntityConfigUid = cert.DynEntityConfigUid
								INNER JOIN ADynEntitySOBDossier d ON d.DynEntityId = cert.dossier
								INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynListValue lv ON lv.DynEntityConfigUid = ac.DynEntityConfigUid 
								       AND lv.DynEntityAttribConfigUid = ac.DynEntityAttribConfigUid
								       AND lv.AssetUid = d.DynEntityUid									   
								INNER JOIN DynListItem li ON li.DynListItemUid = lv.DynListItemUid
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
  							-- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
							    	cert.ActiveVersion = 1
								AND ac.ActiveVersion = 1
								AND p.ActiveVersion = 1
								AND d.ActiveVersion = 1
								AND cert.Total_days = 0
								AND ac.DynEntityAttribConfigId = 7103
								AND li.Value IN ('Running', 'Suspended', 'Membership OK')
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights									
					) AS SearchResults
			)
	        
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount

			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewUnequalDaysSOBPaymentsCertificates]'
		BEGIN		
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY Suspended_date DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, d.Suspended_Date, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 
								INNER JOIN ADynEntitySOBCertificate cert ON FT_TBL.DynEntityUid = cert.DynEntityUId 
									   AND FT_TBL.DynEntityConfigUid = cert.DynEntityConfigUid
								INNER JOIN ADynEntitySOBDossier d ON cert.dossier = d.DynEntityId
								INNER JOIN ADynEntitySOBPayment pay ON pay.DynEntityId = cert.Payment
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
						    -- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
									cert.ActiveVersion = 1
								AND p.ActiveVersion = 1
								AND d.ActiveVersion = 1
								AND pay.ActiveVersion = 1
								AND cert.Total_days <> pay.Days
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights									
					) AS SearchResults
			)
	        
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewSOBPaymentsZeroDays]'
		BEGIN		
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY Suspended_date DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, d.Suspended_Date, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 
								INNER JOIN ADynEntitySOBPayment pay ON FT_TBL.DynEntityUid = pay.DynEntityUId 
									   AND FT_TBL.DynEntityConfigUid = pay.DynEntityConfigUid
								INNER JOIN ADynEntitySOBDossier d ON pay.dossier = d.DynEntityId
								INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynListValue lv ON lv.DynEntityConfigUid = d.DynEntityConfigUid 
								       AND lv.DynEntityAttribConfigUid = ac.DynEntityAttribConfigUid
								       AND lv.AssetUid = d.DynEntityUid
								INNER JOIN DynListItem li ON li.DynListItemUid = lv.DynListItemUid
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
						    -- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
									pay.ActiveVersion = 1
								AND p.ActiveVersion = 1
								AND d.ActiveVersion = 1
								AND pay.Days = 0
								AND ac.DynEntityAttribConfigId = 7103
								AND li.Value IN ('Running', 'Suspended', 'Membership OK')
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights										
					) AS SearchResults
			)
	        
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewSOBDossierMembershipOK]'
		BEGIN
		
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY ISNULL(Suspended_date, '1900-01-01') DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, d.Suspended_Date, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 						
								INNER JOIN ADynEntitySOBDossier d ON FT_TBL.DynEntityUid = d.DynEntityUId 
								       AND FT_TBL.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynListValue lv ON lv.DynEntityConfigUid = d.DynEntityConfigUid 
								       AND lv.DynEntityAttribConfigUid = ac.DynEntityAttribConfigUid
								       AND lv.AssetUid = d.DynEntityUid
								INNER JOIN DynListItem li ON li.DynListItemUid = lv.DynListItemUid
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
							-- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
									d.ActiveVersion = 1
								AND ac.ActiveVersion = 1
								AND p.ActiveVersion = 1
								AND ac.DynEntityAttribConfigId = 7103
								AND li.Value = 'Membership OK'
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights									
					) AS SearchResults
			)
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewSOBDossierRunningSOB54]'
		BEGIN
		
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY ISNULL(Suspended_date, '1900-01-01') DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, d.Suspended_Date, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 						
								INNER JOIN ADynEntitySOBDossier d ON FT_TBL.DynEntityUid = d.DynEntityUId 
								       AND FT_TBL.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynListValue lv ON lv.DynEntityConfigUid = d.DynEntityConfigUid 
								       AND lv.DynEntityAttribConfigUid = ac.DynEntityAttribConfigUid
								       AND lv.AssetUid = d.DynEntityUid
								INNER JOIN DynListItem li ON li.DynListItemUid = lv.DynListItemUid
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
							-- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
									d.ActiveVersion = 1
								AND ac.ActiveVersion = 1
								AND p.ActiveVersion = 1
								AND ac.DynEntityAttribConfigId = 7103
								AND li.Value = 'Running'
								AND d.SOB54 = 1
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights									
					) AS SearchResults
			)
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewSOBPaymentsNotLinked2Certificate]'
		BEGIN
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),	
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY Suspended_date DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, d.Suspended_Date, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 
								INNER JOIN ADynEntitySOBPayment pay ON FT_TBL.DynEntityUid = pay.DynEntityUId 
									   AND FT_TBL.DynEntityConfigUid = pay.DynEntityConfigUid
								INNER JOIN ADynEntitySOBDossier d ON pay.dossier = d.DynEntityId
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
						    -- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
									pay.ActiveVersion = 1
								AND p.ActiveVersion = 1
								AND d.ActiveVersion = 1
								AND	NOT EXISTS (SELECT 1 FROM ADynEntitySOBCertificate WHERE ActiveVersion = 1 AND Payment = pay.DynEntityId)
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights									
					) AS SearchResults
			)
	        
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewSOBCertificatesWithoutPayment]'
		BEGIN
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY /*Print_date, */ Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 
								INNER JOIN ADynEntitySOBCertificate cert ON FT_TBL.DynEntityUid = cert.DynEntityUId 
									   AND FT_TBL.DynEntityConfigUid = cert.DynEntityConfigUid
								INNER JOIN ADynEntitySOBDossier d ON cert.dossier = d.DynEntityId
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
						    -- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
									cert.ActiveVersion = 1
								AND p.ActiveVersion = 1
								AND d.ActiveVersion = 1
								AND (cert.Payment IS NULL OR cert.Payment = 0)
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights									
					) AS SearchResults
			)
	        
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewSOBRecentBatches]'
		BEGIN
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY Date_Created DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, batch.Date_Created, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 
								INNER JOIN ADynEntitySOB_Certificates_B batch ON FT_TBL.DynEntityUid = batch.DynEntityUId 
									   AND FT_TBL.DynEntityConfigUid = batch.DynEntityConfigUid							
							WHERE   
						    -- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
									batch.ActiveVersion = 1
								AND batch.Date_Created >= DATEADD(mm, -13, getdate())
								AND batch.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights								
					) AS SearchResults
			)
	        
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END
		
		-- BUB Custom Queries
		IF @keywords = '[OverviewBUBDossierMembershipOK]'
		BEGIN
		
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY ISNULL(Suspended_date, '1900-01-01') DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, d.Suspended_Date, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 						
								INNER JOIN ADynEntityBUBDossier d ON FT_TBL.DynEntityUid = d.DynEntityUId 
								       AND FT_TBL.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynListValue lv ON lv.DynEntityConfigUid = d.DynEntityConfigUid 
								       AND lv.DynEntityAttribConfigUid = ac.DynEntityAttribConfigUid
								       AND lv.AssetUid = d.DynEntityUid
								INNER JOIN DynListItem li ON li.DynListItemUid = lv.DynListItemUid
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
							-- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
									d.ActiveVersion = 1
								AND ac.ActiveVersion = 1
								AND p.ActiveVersion = 1
								AND ac.DynEntityAttribConfigId = 6337
								AND li.Value = 'Membership OK'
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights									
					) AS SearchResults
			)
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewBUBDossierRunningBUB5254]'
		BEGIN
		
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY ISNULL(Suspended_date, '1900-01-01') DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, d.Suspended_Date, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 						
								INNER JOIN ADynEntityBUBDossier d ON FT_TBL.DynEntityUid = d.DynEntityUId 
								       AND FT_TBL.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynListValue lv ON lv.DynEntityConfigUid = d.DynEntityConfigUid 
								       AND lv.DynEntityAttribConfigUid = ac.DynEntityAttribConfigUid
								       AND lv.AssetUid = d.DynEntityUid
								INNER JOIN DynListItem li ON li.DynListItemUid = lv.DynListItemUid
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
							-- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
									d.ActiveVersion = 1
								AND ac.ActiveVersion = 1
								AND p.ActiveVersion = 1
								AND ac.DynEntityAttribConfigId = 6337
								AND li.Value = 'Running'
								AND d.BUB_52_54 = 1
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights									
					) AS SearchResults
			)
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewBUBDossierSuspended]'
		BEGIN
		
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY ISNULL(Suspended_date, '1900-01-01'), Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, d.Suspended_Date, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 						
								INNER JOIN ADynEntityBUBDossier d ON FT_TBL.DynEntityUid = d.DynEntityUId 
								       AND FT_TBL.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityConfigUid = d.DynEntityConfigUid
								INNER JOIN DynListValue lv ON lv.DynEntityConfigUid = d.DynEntityConfigUid 
								       AND lv.DynEntityAttribConfigUid = ac.DynEntityAttribConfigUid
								       AND lv.AssetUid = d.DynEntityUid
								INNER JOIN DynListItem li ON li.DynListItemUid = lv.DynListItemUid
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
							-- WHERE Clause for the Overview Suspended SOB Dossiers + District filter
							(
									d.ActiveVersion = 1
								AND ac.ActiveVersion = 1
								AND p.ActiveVersion = 1
								AND ac.DynEntityAttribConfigId = 6337
								AND li.Value = 'Suspended'
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights										
					) AS SearchResults
			)
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewBUBPaymentsNotLinked2Certificate]'
		BEGIN		
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY Payment_date DESC, Name) AS RowNumber
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, pay.Payment_Date, FT_TBL.Name, p.District AS DistrictId
							FROM IndexActiveDynEntities AS FT_TBL 
								INNER JOIN ADynEntityBUBPayment pay ON FT_TBL.DynEntityUid = pay.DynEntityUId 
									   AND FT_TBL.DynEntityConfigUid = pay.DynEntityConfigUid
								INNER JOIN ADynEntityBUBDossier d ON pay.dossier = d.DynEntityId
/*								
								INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityConfigUid = d.DynEntityConfigUid
								       AND ac.DynEntityAttribConfigId = @BUBDOSSIER_CONFIGID
								       AND ac.ActiveVersion = 1
								INNER JOIN DynListValue lv ON lv.DynEntityConfigUid = ac.DynEntityConfigUid
								       AND lv.DynEntityAttribConfigUid = ac.DynEntityAttribConfigUid
								       AND lv.AssetUid = d.DynEntityUid
								INNER JOIN DynListItem li ON li.DynListItemUid = lv.DynListItemUid
								       AND li.Value <> 'Closed'
*/
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
						    -- WHERE Clause for the Overview Suspended BUB Dossiers + District filter
							(
									pay.ActiveVersion = 1
								AND pay.UpdateDate >= '2013-01-21'  -- All payments converted on 2013-01-20, so these are skipped in the overview!
								AND p.ActiveVersion = 1
								AND d.ActiveVersion = 1
								AND  NOT EXISTS (SELECT 1 FROM ADynEntityBUBCertificate WHERE ActiveVersion = 1 AND Payment = pay.DynEntityId)
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights									
					) AS SearchResults
			)
	        
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
			   AND DistrictId IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId))
		
			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewBUBCertificatesWithoutPayment]'
		BEGIN
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY Print_date DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, cert.Print_date, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 
								INNER JOIN ADynEntityBUBCertificate cert ON FT_TBL.DynEntityUid = cert.DynEntityUId 
									   AND FT_TBL.DynEntityConfigUid = cert.DynEntityConfigUid
								INNER JOIN ADynEntityBUBDossier d ON cert.dossier = d.DynEntityId
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
						    -- WHERE Clause for the Overview 
							(
									cert.ActiveVersion = 1
								AND cert.PrintCert = 1
								AND cert.Print_Date >= '2012-07-01'		-- Certificates older than half a year after conversion, skip them
								AND p.ActiveVersion = 1
								AND d.ActiveVersion = 1
								AND (cert.Payment IS NULL OR cert.Payment = 0)
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights									
					) AS SearchResults
			)
	        
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount

			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewBUBRecentBatches]'
		BEGIN
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY Date_Created DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, batch.Date_Created, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 
								INNER JOIN ADynEntityBUB_Certificates_B batch ON FT_TBL.DynEntityUid = batch.DynEntityUId 
									   AND FT_TBL.DynEntityConfigUid = batch.DynEntityConfigUid							
							WHERE   
						    -- WHERE Clause for the Overview Suspended BUB Dossiers + District filter
							(
									batch.ActiveVersion = 1
								AND batch.Date_Created >= DATEADD(mm, -13, getdate())
								AND batch.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId))
							)	
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights									
					) AS SearchResults
			)
	        
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount
		
			SET @IsCustomQuery = 1;
			RETURN;
		END

		IF @keywords = '[OverviewBUBPaymentsCertificatesDiffDates]'
		BEGIN
			WITH AllowedConfigs AS
			(
				SELECT DynEntityConfigId 
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
			),
			AllowedDepartmetns AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
			),
			GrantedUserIds AS
			(
				SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
			),
			DeniedConfigs AS
			(
				SELECT [DynEntityConfigId]
				FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
			),
			DeniedDepartments AS
			(
				SELECT DepartmentId 
				FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
			),		
			Search AS
			(	
					SELECT *, ROW_NUMBER() OVER(ORDER BY Print_date DESC, Name) AS RowNumber                			
					FROM 
					(
						SELECT FT_TBL.IndexUid, FT_TBL.UserId, FT_TBL.OwnerId, FT_TBL.DynEntityConfigId, FT_TBL.DepartmentId, FT_TBL.TaxonomyItemsIds, 
							   FT_TBL.UpdateDate, FT_TBL.Location, FT_TBL.[User], 1 AS Active, cert.Print_date, FT_TBL.Name
							FROM IndexActiveDynEntities AS FT_TBL 
								INNER JOIN ADynEntityBUBCertificate cert ON FT_TBL.DynEntityUid = cert.DynEntityUId 
									   AND FT_TBL.DynEntityConfigUid = cert.DynEntityConfigUid
								INNER JOIN ADynEntityBUBPayment pay ON cert.Payment = pay.DynEntityId
								       AND pay.ActiveVersion = 1
								INNER JOIN ADynEntityBUBDossier d ON cert.dossier = d.DynEntityId
								INNER JOIN ADynEntityPerson p ON p.DynEntityId = d.person
							WHERE   
						    -- WHERE Clause for the Overview 
							(
									cert.ActiveVersion = 1
								AND cert.PrintCert = 1
								AND (cert.Date_from <> pay.Date_from OR cert.Date_until <> pay.Date_until)
								AND p.ActiveVersion = 1
								AND d.ActiveVersion = 1
								AND (p.District IN (SELECT * FROM f_GetGrantedDistrictIds(@UserId)))
							)
							-- access rights
							AND
							(
								DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
								OR
								DepartmentId IN (SELECT * FROM AllowedDepartmetns)
								OR
								UserId IN (SELECT * FROM GrantedUserIds)
								OR
								OwnerId in (SELECT * FROM GrantedUserIds)
							)
							AND
							(
								DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
								OR
								DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
							)
							-- end access rights										
					) AS SearchResults
			)
	        
			-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
			INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
			SELECT @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
			  FROM Search
			 WHERE RowNumber > @SearchBufferCount

			SET @IsCustomQuery = 1;
			RETURN;
		END


/*
		IF @keywords = '[]'
		BEGIN
			SET @IsCustomQuery = 1;
			RETURN;
		END
*/
END