using System;
using System.Data;
using System.Windows.Forms;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.DynamicEntity.Data;
using AppFramework.DynamicEntity.Entities;

namespace AssetManagerMaintainer
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnSearchReindex_Click(object sender, EventArgs e)
        {
            try
            {
                var indexEntities = new TList<DynEntityIndex>();
                foreach (var at in AssetType.GetAll(0, int.MaxValue))
                {
                    foreach (Asset asset in AssetFactory.GetAssetsByAssetType(at, true))
                    {
                        // we are putting here only ACTIVE assets
                        // DynEntityIndex talbe contains ALL ACTIVE assets in system
                        // DynEntityIndex mainly (but not only) uses for pagination to retrieve pages count for given asset type                    
                        if (Convert.ToBoolean(asset[AttributeNames.ActiveVersion].Value))
                        {
                            var indexEntity = new AppFramework.DynamicEntity.Entities.DynEntityIndex();
                            indexEntity.DynEntityId = asset.ID;
                            indexEntity.DynEntityConfigId = asset.GetConfiguration().ID;
                            if (asset[AttributeNames.Name] != null && !string.IsNullOrEmpty(asset[AttributeNames.Name].Value))
                            {
                                indexEntity.Name = asset[AttributeNames.Name].Value;
                            }
                            if (asset[AttributeNames.Barcode] != null && !string.IsNullOrEmpty(asset[AttributeNames.Barcode].Value))
                            {
                                indexEntity.Barcode = asset[AttributeNames.Barcode].Value;
                            }
                            if (asset[AttributeNames.LocationId] != null && asset[AttributeNames.LocationId].ValueAsID != 0)
                            {
                                indexEntity.LocationId = asset[AttributeNames.LocationId].ValueAsID;
                            }
                            if (asset[AttributeNames.DepartmentId] != null && asset[AttributeNames.DepartmentId].ValueAsID != 0)
                            {
                                indexEntity.DepartmentId = asset[AttributeNames.DepartmentId].ValueAsID;
                            }
                            if (asset[AttributeNames.UserId] != null && asset[AttributeNames.UserId].ValueAsID != 0)
                            {
                                indexEntity.UserId = asset[AttributeNames.UserId].ValueAsID;
                            }
                            if (asset[AttributeNames.OwnerId] != null && asset[AttributeNames.OwnerId].ValueAsID != 0)
                            {
                                indexEntity.OwnerId = asset[AttributeNames.OwnerId].ValueAsID;
                            }
                            indexEntities.Add(indexEntity);
                        }
                    }
                }

                if (indexEntities.Count == 0)
                    throw new Exception("Cannot collect indexes");

                TransactionWrapper.RunInTransaction(() =>
                {
                    DataRepository.Provider.ExecuteNonQuery(CommandType.Text, "DELETE FROM DynEntityIndex");
                    DataRepository.Provider.DynEntityIndexProvider.BulkInsert(indexEntities);
                });
            }
            catch (Exception ex)
            {
                txtLog.Text += string.Format("{0}{1}{2}{1}{1}", ex.Message, Environment.NewLine, ex.StackTrace);
            }
        }
    }
}
