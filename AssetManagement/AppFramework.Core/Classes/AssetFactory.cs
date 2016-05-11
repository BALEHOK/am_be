using AppFramework.Core.AC.Providers;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.DAL.Adapters;

namespace AppFramework.Core.Classes
{
    using AppFramework.Core.AC.Authentication;
    using AppFramework.Core.ConstantsEnumerators;
    using AppFramework.Core.DAL;
    using Services;
    using AppFramework.DataProxy;
    using AppFramework.Entities;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    public class AssetFactory
	{
		/// <summary>
		/// Returns the list of assets by given asset type.
		/// DataSet is already filtered with permissions.
		/// </summary>
		/// <param name="at">Asset type</param>
		/// <param name="rowStart">Start row</param>
		/// <param name="rowsNumber">Rows amount</param>
		/// <returns>Assets collection</returns>
		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public static IEnumerable<Asset> GetAssetsByAssetTypeId(long assetTypeId, bool filterWithPermissions = true, int? rowStart = null, int? rowsNumber = null)
		{
			var unitOfWork = new UnitOfWork();
            var atRepository = AssetTypeRepository.Create(unitOfWork);
            var linkedEntityFinder = new LinkedEntityFinder(unitOfWork);
            var attributeValueFormatter = new AttributeValueFormatter(linkedEntityFinder);
            var rightsService = new RightsService(unitOfWork);
            var attributeRepository = new AttributeRepository(unitOfWork);
            var indexationService = new IndexationService(unitOfWork);
            var assetsService = new AssetsService(unitOfWork, atRepository, attributeRepository, attributeValueFormatter, rightsService, indexationService);
            var assetType = atRepository.GetById(assetTypeId);
            var userService = new UserService(unitOfWork, atRepository, assetsService);
		    var currentUserId = userService.GetCurrentUser().Id;

		    var result = assetsService.GetAssetsByAssetTypeAndUser(assetType, currentUserId).ToList();
            return rowStart.HasValue && rowsNumber.HasValue ? result.Skip(rowStart.Value).Take(rowsNumber.Value) : result;
		}

		/// <summary>
		/// Returns the count of active assets filtered by permissions
		/// </summary>
		/// <param name="assetTypeId">AssetType ID</param>
		/// <param name="filterWithPermissions"></param>
		/// <returns>Count of assets</returns>
		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public static int GetAssetsCountByAssetTypeId(long assetTypeId, bool filterWithPermissions)
		{
            var unitOfWork = new UnitOfWork();
            var atRepository = AssetTypeRepository.Create(unitOfWork);
            var linkedEntityFinder = new LinkedEntityFinder(unitOfWork);
            var attributeValueFormatter = new AttributeValueFormatter(linkedEntityFinder);
            var rightsService = new RightsService(unitOfWork);
            var attributeRepository = new AttributeRepository(unitOfWork);
            var indexationService = new IndexationService(unitOfWork);
            var assetsService = new AssetsService(unitOfWork, atRepository, attributeRepository, attributeValueFormatter, rightsService, indexationService);
            var assetType = atRepository.GetById(assetTypeId);
            var userService = new UserService(unitOfWork, atRepository, assetsService);
            var currentUser = userService.GetCurrentUser();
			int count;
			if (filterWithPermissions)
			{
				//А иначе каунты не сходятся, записей типа всего 7, а GetPermittedAssetsCount возвращает все 62 без учёта ActiveVersion. Paging падает 
				//var result = unitOfWork.GetPermittedAssetsCount(assetTypeId, AccessManager.Instance.CurrentUserId).FirstOrDefault().Value;

                var assetIndexes = unitOfWork.GetPermittedAssets(assetType.ID, currentUser.Id, null, null);
				var assets = assetIndexes.Select(i => assetsService.GetAssetById(i.DynEntityId, assetType)).ToList();
				return assets.Count;
			}
			else
			{
				count = int.Parse(unitOfWork.SqlProvider.ExecuteScalar(string.Format(
					"SELECT COUNT(*) AS Expr1 FROM {0} WHERE ActiveVersion=1", assetType.DBTableName)).ToString());
			}
			return count;
		}

		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public static IEnumerable<Asset> GetHistoryAssets(long assetTypeId, long assetId, int rowStart,
																		int rowsNumber)
		{
            var unitOfWork = new UnitOfWork();
            var atRepository = AssetTypeRepository.Create(unitOfWork);
            var linkedEntityFinder = new LinkedEntityFinder(unitOfWork);
            var attributeRepository = new AttributeRepository(unitOfWork);
            var dynamicListsService = new DynamicListsService(unitOfWork, attributeRepository);
            var attributeValueFormatter = new AttributeValueFormatter(linkedEntityFinder);
            var rightsService = new RightsService(unitOfWork);
            var indexationService = new IndexationService(unitOfWork);
            var assetsService = new AssetsService(unitOfWork, atRepository, attributeRepository, attributeValueFormatter, rightsService, indexationService);
            var dataTypeService = new DataTypeService(unitOfWork);
            var tableProvider = new DynTableProvider(unitOfWork, new DynColumnAdapter(dataTypeService));
            var assetType = atRepository.GetById(assetTypeId);
            var rows = tableProvider.GetHistoryAssets(assetType.Base, assetId, rowStart, rowsNumber);
			return rows.Select(row => new Asset(
				assetType, 
				row,
                attributeValueFormatter,
                atRepository,
				assetsService,
				unitOfWork,
                dynamicListsService));
		}

		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public static int GetHistoryAssetsCount(long assetTypeId, long assetId)
		{
            var unitOfWork = new UnitOfWork();
            var assetTypeRepository = AssetTypeRepository.Create(unitOfWork);
            var at = assetTypeRepository.GetById(assetTypeId);
			var count = DynTableProvider.GetCount(at.Base, new Dictionary<string, string>() 
			{ { AttributeNames.ActiveVersion, bool.FalseString },
			  { AttributeNames.DynEntityId, assetId.ToString() }});
			return count;
		}

		/// <summary>
		/// Get current asset type  uid for uid      
		/// </summary>
		/// <param name="_assetTypeUID">Uid of AssetType</param>
		/// <returns>Uid of AssetType</returns>
		public static long? GetCurrentAssetTypeUid(long assetTypeUid)
		{
			var unitOfWork = new UnitOfWork();
			var currentConfigAsset = unitOfWork.DynEntityConfigRepository.Get(a => a.DynEntityConfigUid == assetTypeUid).FirstOrDefault();
			long? assetTypeUID = null;
			if (currentConfigAsset != null)
				assetTypeUID = unitOfWork.DynEntityConfigRepository.Where(
					a => a.DynEntityConfigId == currentConfigAsset.DynEntityConfigId && a.ActiveVersion
					).
				   OrderByDescending(a => a.DynEntityConfigUid).FirstOrDefault().DynEntityConfigUid;
			return assetTypeUID;
		}
        
		/// <summary>
		/// Returns the information about connected taxonomy items
		/// </summary>
		/// <returns></returns>
		public static TaxonomyItemsInformation GetTaxonomyItemsInformation(long assetUid, long assetTypeUid)
		{
			var taxonomyUids = new StringBuilder();
			var taxonomiesDescriptions = new StringBuilder();
			var categoriesDescriptions = new StringBuilder();
			var taxonomyItemsIds = new StringBuilder();
			var taxonomyItemsUds = new StringBuilder();

			var unitOfWork = new DataProxy.UnitOfWork();
			foreach (AssetsTaxonomies item in unitOfWork.AssetsTaxonomiesRepository.Get(at => at.DynEntityUid == assetUid
				&& at.DynEntityConfigUid == assetTypeUid))
			{
				taxonomyUids.AppendFormat("{0} ", item.TaxonomyUid);
				taxonomiesDescriptions.AppendFormat("{0} {1} ", item.TaxonomyItemName, item.TaxonomyItemDescription);
				taxonomyItemsIds.AppendFormat("{0} ", item.TaxonomyItemId);
				taxonomyItemsUds.AppendFormat("{0} ", item.TaxonomyItemUid);
				if (item.IsCategory)
				{
					categoriesDescriptions.AppendFormat("{0} {1} ", item.TaxonomyItemName, item.TaxonomyItemDescription);
				}
			}

			return new TaxonomyItemsInformation()
			{
				DynEntityUid = assetUid,
				DynEntityConfigUid = assetTypeUid,
				TaxomomiesUids = taxonomyUids.ToString().Trim(),
				TaxonomyItemsIds = taxonomyItemsIds.ToString().Trim(),
				TaxonomyItemsUids = taxonomyItemsUds.ToString().Trim(),
				TaxonomiesDescriptions = taxonomiesDescriptions.ToString().Trim(),
				CategoriesDescriptions = categoriesDescriptions.ToString().Trim()
			};
		}

		public static IEnumerable<KeyValuePair<long, string>> GetAssetsByIds(AssetType assetType, IEnumerable<long> ids)
		{
			using (var unitOfWork = new DataProxy.UnitOfWork())
			{
                var dataTypeService = new DataTypeService(unitOfWork);
                var tableProvider = new DynTableProvider(unitOfWork, new DynColumnAdapter(dataTypeService));
                var rows = tableProvider.GetRows(
					assetType.Base,
					new Dictionary<string, string>() { { AttributeNames.ActiveVersion, bool.TrueString } },
					null,
					null,
					AttributeNames.Name,
					ids.ToList());

				foreach (var row in rows)
				{
					yield return new KeyValuePair<long, string>(
                        long.Parse(row.Fields.Single(r => r.Name == AttributeNames.DynEntityId).Value.ToString()),
						row.Fields.Single(r => r.Name == AttributeNames.Name).Value.ToString());
				}
			}
		}
	}
}
