using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Barcode;
using AppFramework.Core.Classes.Batch;
using AppFramework.Core.Classes.DynLists;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.Interface;
using AppFramework.Core.Interfaces;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;
using AppFramework.Entities;
using Common.Logging;
using Microsoft.Practices.Unity;
using Place = AppFramework.Core.Classes.Place;
using ZipCode = AppFramework.Core.Classes.ZipCode;
using AssetManager.Infrastructure.Services;
using AppFramework.Core.Services;
using AppFramework.Tasks;

namespace AssetSite
{
    /// <summary>
    /// Summary description for amDataService
    /// </summary>
    [WebService(Namespace = "http://am.local/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class amDataService : WebService
    {
        [Dependency]
        public IAttributeCalculator AttributeCalculator { get; set; }
        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }
        [Dependency]
        public IAssetsService AssetsService { get; set; }
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }
        [Dependency]
        public IBarcodeProvider BarcodeProvider { get; set; }
        [Dependency]
        public IDynListItemService DynListItemService { get; set; }
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }
        [Dependency]
        public IPanelsService PanelsService { get; set; }
        [Dependency]
        public IBatchJobFactory BatchJobFactory { get; set; }
        [Dependency]
        public IBatchJobManager BatchJobManager { get; set; }
        [Dependency]
        public IScreensService ScreensService { get; set; }
        [Dependency]
        public ISearchService SearchService { get; set; }
        [Dependency]
        public ILog Logger { get; set; }
        [Dependency]
        public ITasksService TasksService { get; set; }
        [Dependency]
        public ITaskRunnerFactory TaskRunnerFactory { get; set; }

        public amDataService()
        {
            InjectDependencies();
            Helpers.Culture.InitCulture();
        }

        private void InjectDependencies()
        {
            HttpContext context = HttpContext.Current;

            if (context == null)
                return;

            var app = HttpContext.Current.ApplicationInstance as IHttpUnityApplication;
            if (app == null)
                return;

            if (app.UnityContainer == null)
                throw new InvalidOperationException(
                  "Container on Global Application Class " +
                  "is Null. Cannot perform BuildUp.");
            app.UnityContainer.BuildUp(this);
        }

        /// <summary>
        /// Returns search conditions for last performed search
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string GetSearchConditions(int searchId)
        {
            var conditions = HttpContext.Current.Session["SearchConditions"] 
                as Dictionary<int, string>;
            if (conditions != null && conditions.ContainsKey(searchId))
                return conditions[searchId];
            else return string.Empty;
        }

        [WebMethod]
        public bool Ping()
        {
            return true;
        }

        [WebMethod]
        public AddedItemData AddDynListItem(long dynListUID, string itemValue, long assocListUID, string containerId)
        {
            try
            {
                var _dynList = DynamicListsService.GetByUid(dynListUID);
                var item = new DynamicListItem()
                {
                    ParentDynList = _dynList,
                    DisplayOrder = (_dynList.Items.Count != 0) ? _dynList.Items.Max(i => i.DisplayOrder) + 1 : 1,
                    Value = itemValue
                };

                if (assocListUID > 0)
                    item.AssociatedDynList = DynamicListsService.GetByUid(assocListUID);

                _dynList.Items.Add(item);
                DynamicListsService.Save(_dynList.Base);

                return new AddedItemData() { ItemUID = item.UID, ItemValue = itemValue, ContainerId = containerId, Action = 1 };
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public void EditDynListItem(long DynListId, long DynListItemID, string value, long assocDynListUID)
        {

        }

        [WebMethod]
        public void DeleteDynListItem(long itemId)
        {
            var item = DynListItemService.GetByUid(itemId);
            DynListItemService.Delete(item.Base);
        }

        [WebMethod(EnableSession = true)]
        public SearchDataHolder SearchAssetsReturnsIdName(long assetTypeId, long assetTypeAttributeId, string searchPattern, string containerId, int pageNumber)
        {
            if (string.IsNullOrEmpty(searchPattern))
                searchPattern = "%%";
            int totalCount;
            int pageSize = 100;
            var result = SearchEngine.FindIdNameBySearchPattern(UnitOfWork, assetTypeId, assetTypeAttributeId, out totalCount, searchPattern, pageNumber, pageSize);
            SearchDataHolder svcResult = new SearchDataHolder();
            svcResult.ContainerId = containerId;
            svcResult.Data = result;
            svcResult.Pattern = searchPattern;
            svcResult.HasMoreRows = totalCount > pageNumber * pageSize;
            svcResult.CurrentPage = pageNumber;
            return svcResult;
        }

        [WebMethod(EnableSession = true)]
        public SearchDataHolder SearchAssetIds(long assetTypeId, long assetTypeAttributeId, string searchPattern, string containerId)
        {
            return SearchForAssets(assetTypeId, assetTypeAttributeId, searchPattern, containerId, false);
        }

        [WebMethod(EnableSession = true)]
        private SearchDataHolder SearchForAssets(long assetTypeId, long assetTypeAttributeId, string searchPattern, string containerId, bool useUID)
        {
            var at = AssetTypeRepository.GetById(assetTypeId);
            string attrName = at.Attributes.Single(a => a.ID == assetTypeAttributeId).DBTableFieldName;

            List<KeyValuePair<long, string>> _assets = new List<KeyValuePair<long, string>>();

            // find the assigned asset by name
            List<ISearchCondition> conditions = new List<ISearchCondition>();

            if (!String.IsNullOrEmpty(searchPattern) && searchPattern.ToLower() != "start typing...")
            {
                conditions.Add(new SearchCondition()
                {
                    FieldName = attrName,
                    Value = searchPattern,
                    AndOr = ConcatenationOperation.And,
                    SearchOperator = SearchOperator.Like
                });
            }

            int totalCount;

            // TODO
            throw new NotImplementedException();
            //var result = SearchEngine.FindByAssetType(at.UID, conditions, out totalCount);

            //foreach (AppFramework.Core.Classes.Asset asset in result)
            //{
            //    if (useUID)
            //        _assets.Add(new KeyValuePair<long, string>(asset.UID, asset[attrName].Value));
            //    else
            //        _assets.Add(new KeyValuePair<long, string>(asset.ID, asset[attrName].Value));
            //}

            //SearchDataHolder svcResult = new SearchDataHolder();
            //svcResult.ContainerId = containerId;
            //svcResult.Data = _assets;
            //svcResult.Pattern = searchPattern;
            //return svcResult;
        }

        [WebMethod(EnableSession = true)]
        public ResultDto DeleteAsset(long atUID, long aId)
        {
            var result = new ResultDto();
            try
            {
                var at = AssetTypeRepository.GetByUid(atUID);
                var asset = AssetsService.GetAssetById(aId, at);
                var permission = AuthenticationService.GetPermission(asset);
                AssetsService.DeleteAsset(asset);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        [WebMethod]
        public long DeleteTemplate(long atUID, long tID)
        {
            return AssetTemplateService.DeleteById(atUID, tID) ? tID : 0;
        }

        [WebMethod(true)]
        public dynamic SaveBatchTask(int batchAction, string date, string time, double repeatHours, string notes, string containerId)
        {
            AppFramework.Core.Classes.Batch.BatchJob job;
            if ((BatchActionType)batchAction == BatchActionType.RebuildSearchIndexActive)
                job = BatchJobFactory.CreateRebuildIndexJob(AuthenticationService.CurrentUserId, false);
            else if ((BatchActionType)batchAction == BatchActionType.RebuildSearchIndexHistory)
                job = BatchJobFactory.CreateRebuildIndexJob(AuthenticationService.CurrentUserId, true);
            else 
                throw new NotSupportedException();
            job.Schedule(repeatHours, DateTime.Parse(string.Format("{0} {1}", date, time)), notes);
            BatchJobManager.SaveJob(job);
            return new { ContainerId = containerId, RedirectUrl = job.NavigateUrl };
        }

        [WebMethod(true)]
        public string SaveFaqItem(long dynEntityID, string question, string answer, string culture, string containerId)
        {
            AppFramework.Core.Classes.Asset faqItem = null;
            var at = AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Faq);

            if (dynEntityID == 0)
            {
                faqItem = AssetsService.CreateAsset(at);
                faqItem["Language"].Value = culture;
            }
            else
            {
                faqItem = AssetsService.GetAssetById(dynEntityID, at);
            }

            faqItem["Question"].Value = question;
            faqItem["Answer"].Value = answer;

            AssetsService.InsertAsset(faqItem);
            return containerId;
        }

        [WebMethod(true)]
        public FaqItemInfo GetFaqItem(long dynEntityId, string containerId)
        {
            var result = new FaqItemInfo();
            var at = AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Faq);
            var faqItem = AssetsService.GetAssetById(dynEntityId, at);

            result.ContainerId = containerId;

            if (faqItem != null)
            {
                result.Answer = faqItem["Answer"].Value;
                result.Question = faqItem["Question"].Value;
                result.Cultrue = faqItem["Language"].Value;
            }

            return result;
        }

        [WebMethod]
        public SearchDataHolder FindPOZ(string searchPattern, string containerId)
        {
            SearchDataHolder result = new SearchDataHolder();
            result.ContainerId = containerId;
            result.Pattern = searchPattern;

            var places = new List<AppFramework.Entities.Place>();
            bool searchPlacesByZipcode = !string.IsNullOrEmpty(searchPattern) && searchPattern.All(c => Char.IsDigit(c));
            if (searchPlacesByZipcode)
            {
                var zz = UnitOfWork.ZipCodeRepository.Get(z => z.Code.Contains(searchPattern),
                    orderBy: codes => codes.OrderBy(c => c.Code),
                    include: zip => zip.Place2Zip.Select(z => z.Place));
                places = (from p2z in
                              (from zip in zz
                               select zip.Place2Zip).SelectMany(p2z => p2z)
                          select p2z.Place)
                         .ToList();
            }
            else
            {
                if (!string.IsNullOrEmpty(searchPattern) && searchPattern.ToLower() != "start typing...")
                {
                    places = UnitOfWork.PlaceRepository.Get(p => p.PlaceName.ToLower().Contains(searchPattern.ToLower()),
                        orderBy: pp => pp.OrderBy(p => p.PlaceName),
                        include: place => place.Place2Zip.Select(z => z.ZipCode))
                        .ToList();
                }
            }

            foreach (var place in places)
            {
                result.Data.Add(new KeyValuePair<long, string>(place.PlaceId,
                    string.Format("{0} ({1})", place.PlaceName, string.Join(",", place.Place2Zip.Select(pz => pz.ZipCode.Code)))));
            }
            return result;
        }

        [WebMethod]
        public POZInfo GetPOZ(long id, int oType, string containerId)
        {
            POZInfo info = new POZInfo();
            info.ContainerId = containerId;
            info.Id = id;
            info.Data = "";

            if (oType == 1)// looking zip code
            {
                ZipCode zip = ZipCode.GetById(id);
                if (zip != null)
                {
                    info.Data = zip.Code;
                }
            }
            else
            {
                Place place = Place.GetById(id);
                if (place != null)
                {
                    info.Data = place.PlaceName;
                }
            }
            return info;
        }

        [WebMethod]
        public string EditPlace(long placeId, string placeName, string containerId)
        {
            var place = UnitOfWork.PlaceRepository.SingleOrDefault(p => p.PlaceId == placeId);

            if (place != null)
            {
                place.PlaceName = placeName;
                UnitOfWork.PlaceRepository.Update(place);
                UnitOfWork.Commit();
            }

            return containerId;
        }

        [WebMethod]
        public string Addplace(string placeName, string containerId)
        {
            Place nplace = new Place();
            nplace.PlaceName = placeName;
            nplace.Save();

            return containerId;
        }

        [WebMethod]
        public void RemovePlace(long placeId)
        {
            Place.Delete(placeId);
        }

        [WebMethod]
        public string EditZip(long zipId, string zipName, string containerId)
        {
            //ZipCode current = ZipCode.GetById(zipId);

            var current = UnitOfWork.ZipCodeRepository.Single(p => p.ZipId == zipId);

            if (current != null)
            {
                current.Code = zipName;
                UnitOfWork.ZipCodeRepository.Update(current);
                UnitOfWork.Commit();
            }

            return containerId;
        }

        [WebMethod]
        public string AddZipCode(string zipCode, long PlaceId, string containerId)
        {
            var place = UnitOfWork.PlaceRepository.SingleOrDefault(p => p.PlaceId == PlaceId);
            if (place != null)
            {
                var zip = new AppFramework.Entities.ZipCode() { Code = zipCode };
                UnitOfWork.ZipCodeRepository.Insert(zip);
                place.Place2Zip.Add(new AppFramework.Entities.Place2Zip()
                {
                    Place = place,
                    ZipCode = zip
                });
                UnitOfWork.Commit();
                return containerId;
            }
            return String.Empty;
        }

        [WebMethod]
        public void RemoveZip(long zipId)
        {
            ZipCode.Delete(zipId);
        }

        /// <summary>
        /// Serves AJAX requests for translation adding
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cultureName"></param>
        /// <param name="translation"></param>
        [WebMethod]
        public void AddTranslation(string key, string cultureName, string translation)
        {
            if (object.Equals(string.Empty, key)
                || object.Equals(string.Empty, cultureName)
                || object.Equals(string.Empty, translation))
            {
                return;
            }

            TranslatableString translator = new TranslatableString(key);
            translator.AddTranslation(translation, CultureInfo.GetCultureInfo(cultureName));
        }

        /// <summary>
        /// Serves AJAX requests for translation requesting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        [WebMethod]
        public TranslationInfo GetTranslation(string key, string cultureName, string controlId = "")
        {
            return new TranslationInfo()
            {
                Translation = TranslatableString.GetTranslationDb(key, CultureInfo.GetCultureInfo(cultureName)),
                ControlId = controlId
            };
        }

        /// <summary>
        /// Serves requests for new barcode generation
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GenerateBarcode()
        {
            return BarcodeProvider.GenerateBarcode();
        }

        [WebMethod]
        public string SavePanel(long panelUid, string name, string desc, bool isChildAssets, long? childAttr, long atId, long screenId, string dialogId)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Panel name cannot be empty");

            var pnl = panelUid != 0
                ? PanelsService.GetByUid(panelUid)
                : new AttributePanel();

            pnl.Name = name;
            pnl.Description = desc;
            pnl.IsChildAssets = isChildAssets;
            pnl.ChildAssetAttrId = childAttr;
            pnl.ScreenId = screenId;
            pnl.DynEntityConfigUId = atId;
            pnl.DisplayOrder = (byte)PanelsService.GetAllByScreenId(screenId).Count;
            PanelsService.Save(pnl);
            return dialogId;
        }

        [WebMethod]
        public PanelInfo GetPanel(long panelUid, string dialogId)
        {
            var pnl = PanelsService.GetByUid(panelUid);

            var info = new PanelInfo();
            if (pnl != null)
            {
                info.ContainerId = dialogId;
                info.IsChildAssets = pnl.IsChildAssets;
                info.ChildAssetAttrId = pnl.ChildAssetAttrId.GetValueOrDefault();
                info.Description = pnl.Description;
                info.Name = pnl.Name;
            }

            return info;
        }

        [WebMethod]
        public object[] GetTasksByAssetTypeId(long atId)
        {
            var tasks = TasksService.GetByAssetTypeId(atId, AuthenticationService.CurrentUserId);
            return (from task in tasks
                    select new
                    {
                        Id = task.TaskId,
                        Name = task.Name
                    }).ToArray();
        }

        [WebMethod(EnableSession = true)]
        public TaskExecutionDataHolder ExecuteTask(int taskId, long? assetUid = null)
        {
            var task = TasksService.GetTaskById(taskId, AuthenticationService.CurrentUserId);
            var runner = TaskRunnerFactory.GetRunner(task, AuthenticationService.CurrentUserId, assetUid);
            var result = runner.Run(task);
            return new TaskExecutionDataHolder()
            {
                NavigationResult = 
                    result.Status == AppFramework.Tasks.Enumerations.TaskStatus.Sussess 
                    && result.ActionOnComplete == AppFramework.Tasks.Enumerations.TaskActionOnComplete.Navigate
                        ? result.NavigationResult 
                        : string.Empty,
                Messages = result.Errors.ToArray()
            };
        }

        public class TaskExecutionDataHolder
        {
            public string NavigationResult { get; set; }
            public string[] Messages { get; set; }
        }

        [WebMethod]
        public object[] GetScreensByAssetTypeUid(long atUid)
        {
            var assetType = AssetTypeRepository.GetByUid(atUid);
            var screens = ScreensService.GetScreensByAssetTypeUid(assetType.UID)
                .OrderBy(i => i.UpdateDate);
            return (from screen in screens
                    select new
                    {
                        Id = screen.ScreenId,
                        Name = (new TranslatableString(screen.Name).GetTranslation())
                    }).ToArray();
        }

        [WebMethod]
        public string[] GetCompletionList(string prefixText, int count)
        {
            var items = new List<string>(count);
            var result = AssetsService.GetIdNameListByAssetType(
                AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Document)).Where(x => x.Value.Contains(prefixText));
            for (int i = 0; i < count && i < result.Count(); i++)
            {
                if (!string.IsNullOrEmpty(result.ElementAt(i).Value))
                    items.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem(result.ElementAt(i).Value, result.ElementAt(i).Key.ToString()));
            }
            return items.ToArray();
        }

        [WebMethod(true)]
        public List<SearchCounter> GetSearchCounters(
            Guid searchId,
            string keywords,
            string configsIds,
            string taxonomyItemsIds,
            int time,
            bool type)
        {
            return SearchService.GetCounters(
                searchId,
                AuthenticationService.CurrentUserId,
                keywords, 
                configsIds, 
                taxonomyItemsIds, 
                (TimePeriodForSearch)time, 
                type);
        }
    }

    /// <summary>
    /// Asset data DTO
    /// </summary>
    public class AssetDataDto
    {
        public string AssetUid { get; set; }
        public string AssetTypeUid { get; set; }
        public List<AttributeValueDto> AssetValues { get; set; }
    }

    /// <summary>
    /// attribute value DTO
    /// </summary>
    public class AttributeValueDto
    {
        public string Uid { get; set; }
        public string Value { get; set; }
    }

    public class AddedItemData
    {
        public long ItemUID { get; set; }
        public string ItemValue { get; set; }
        public string ContainerId { get; set; }
        public int Action { get; set; }

        public AddedItemData() { }
    }

    public class SearchDataHolder
    {
        public string Pattern { get; set; }
        public string ContainerId { get; set; }
        public bool HasMoreRows { get; set; }
        public List<KeyValuePair<long, string>> Data { get; set; }

        public SearchDataHolder()
        {
            this.Data = new List<KeyValuePair<long, string>>();
        }

        public int CurrentPage { get; set; }
    }

    public class ReservationResponse
    {
        public string ContainerId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ScheduleTaskInfo
    {
        public string ContainerId { get; set; }
        public int TaskType { get; set; }
        public int Interval { get; set; }
    }

    public class FaqItemInfo
    {
        public string ContainerId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Cultrue { get; set; }
    }

    public class POZInfo
    {
        public long Id { get; set; }
        public string Data { get; set; }
        public string ContainerId { get; set; }

        public POZInfo() { }
    }

    public class TranslationInfo
    {
        public string Translation { get; set; }
        public string ControlId { get; set; }
    }

    public class PanelInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsChildAssets { get; set; }
        public string ContainerId { get; set; }
        public long ChildAssetAttrId { get; set; }
    }

    public class ResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
