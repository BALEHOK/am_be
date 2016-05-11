-- 10.03.2016 Alexander Shukletsov
-- Ensure that all the attribute configs have only one active ver
WITH t (Id, LastRev) AS
(
	SELECT [DynEntityAttribConfigId], MAX([Revision]) FROM [DynEntityAttribConfig]
	WHERE [ActiveVersion] = 1
	GROUP BY [DynEntityAttribConfigId]
	HAVING COUNT([ActiveVersion]) > 1
)
UPDATE a
SET a.ActiveVersion = 0
FROM [DynEntityAttribConfig] a JOIN t ON a.DynEntityAttribConfigId = t.Id
WHERE a.ActiveVersion = 1 AND a.Revision <> t.LastRev
