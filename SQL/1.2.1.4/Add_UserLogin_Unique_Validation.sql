/****** Script for SelectTopNRows command from SSMS  ******/
UPDATE DynEntityAttribConfig SET ValidationExpr='@Unique', ValidationMessage='User with the same name already exists'
WHERE DynEntityAttribConfigUid = 
(SELECT deac.DynEntityAttribConfigUid FROM DynEntityAttribConfig AS deac
JOIN DynEntityConfig AS dc ON dc.DynEntityConfigUid = deac.DynEntityConfigUid
WHERE dc.Name = 'User' AND dc.ActiveVersion=1 AND deac.Name = 'Name');