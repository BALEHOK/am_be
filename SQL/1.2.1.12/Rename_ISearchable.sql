EXEC sp_rename  @objname='IndexActiveDynEntities.DynEntityUId', @newname='DynEntityUid', @objtype='COLUMN';
EXEC sp_rename  @objname='IndexActiveDynEntities.DynEntityConfigUId', @newname='DynEntityConfigUid', @objtype='COLUMN';
EXEC sp_rename  @objname='IndexHistoryDynEntities.DynEntityUId', @newname='DynEntityUid', @objtype='COLUMN';
EXEC sp_rename  @objname='IndexHistoryDynEntities.DynEntityConfigUId', @newname='DynEntityConfigUid', @objtype='COLUMN';