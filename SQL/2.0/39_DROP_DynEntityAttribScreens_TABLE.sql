-- 10.03.2016 Alexander Shukletsov
-- This script removes Related items relation from screens
DELETE FROM [AssetTypeScreen]
WHERE ScreenId IN
(
	SELECT DISTINCT ScreenId FROM [DynEntityAttribScreens]
);

GO

IF @@ERROR = 0
	drop table [DynEntityAttribScreens];