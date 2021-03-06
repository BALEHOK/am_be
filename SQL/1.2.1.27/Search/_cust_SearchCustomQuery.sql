/****** Object:  StoredProcedure [dbo].[_cust_SearchCustomQuery]    Script Date: 01/08/2013 16:30:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: Wouter Steegmans
-- Create date	: 05/01/2013
-- Description	: Checks for custom queries
-- =============================================
-- Overview custom queries
--	* OverviewSuspendedSOBDossiers
--  * OverviewSOBCertificatesZeroDays
--  * OverviewUnequalDaysSOBPaymentsCertificates
--  * OverviewSOBPaymentsZeroDays
--  * OverviewSOBDossierMembershipOK
--  * OverviewSOBDossierRunningSOB54
--  * OverviewBUBDossierMembershipOK
--  * OverviewBUBDossierRunningBUB5254
-- =============================================
CREATE PROCEDURE [dbo].[_cust_SearchCustomQuery]
        @SearchId bigint,
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
        @IsCustomQuery int = 0 OUTPUT 
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

		/* IF CHARINDEX(@keywords, '#') = 0 */ RETURN;

/*
		IF @keywords = '#customquerylabel#'
		BEGIN
			-- SELECT records ...
		
			SET @IsCustomQuery = 1;
			RETURN;
		END
*/
END