/****** Сценарий для команды SelectTopNRows среды SSMS  ******/
UPDATE AttributePanel SET AttributePanel.ScreenId = NULL WHERE AttributePanel.ScreenId IN
(
  SELECT DISTINCT AttributePanel.ScreenId
  FROM AttributePanel
  LEFT JOIN AssetTypeScreen ON AttributePanel.ScreenId = AssetTypeScreen.ScreenId
  WHERE AssetTypeScreen.ScreenId IS NULL AND AttributePanel.ScreenId IS NOT NULL
 )