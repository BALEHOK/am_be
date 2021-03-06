UPDATE DynEntityAttribConfig SET IsRequired=0 WHERE Name='Update User' AND DynEntityConfigUid IN (
SELECT DynEntityConfigUid FROM DynEntityConfig WHERE Name='User' AND ActiveVersion=1
);

UPDATE DynEntityAttribConfig SET Active=0 WHERE Name='Taalcode' AND DynEntityConfigUid IN (
SELECT DynEntityConfigUid FROM DynEntityConfig WHERE Name='User' AND ActiveVersion=1
);

UPDATE DynEntityAttribConfig SET Active=1 WHERE Name IN ('LastLoginDate', 'LastLockoutDate', 'LastActivityDate') AND DynEntityConfigUid IN (
SELECT DynEntityConfigUid FROM DynEntityConfig WHERE Name='User' AND ActiveVersion=1
);