EXEC sp_rename
    @objname = 'DynEntityTaxonomyItemHistory.DynEntityConfigUID',
    @newname = 'DynEntityConfigUid',
    @objtype = 'COLUMN';
EXEC sp_rename
    @objname = 'DynEntityTaxonomyItemHistory.TaxonomyItemUId',
    @newname = 'TaxonomyItemUid',
    @objtype = 'COLUMN';    
EXEC sp_rename
    @objname = 'DynEntityTaxonomyItemHistory.DynEntityUId',
    @newname = 'DynEntityUid',
    @objtype = 'COLUMN';        