using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.Core.Classes.Stock;
using AppFramework.Core.Classes.Validation;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DataTypes;
using AppFramework.Core.Validation;
using AppFramework.Entities;
using AppFramework.Reports;
using AssetSite.Controls;
using AssetSite.Helpers;
using Microsoft.Practices.Unity;
using WarstarDev.XmlInterpreter;
using TaxonomyItem = AppFramework.Entities.TaxonomyItem;
using TransactionType = AppFramework.Core.Classes.Stock.TransactionType;

namespace AssetSite.Asset
{
    public partial class View : AssetController
    {
        [Dependency]
        public IDataTypeService DataTypeService { get; set; }
        [Dependency]
        public IAttributeCalculator AttributeCalculator { get; set; }
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }
        [Dependency]
        public IValidationService ValidationService { get; set; }
        [Dependency]
        public IScreensService ScreensService { get; set; }
        [Dependency]
        public IAssetPanelsAdapter AssetPanelsAdapter { get; set; }

        private int _panelsCount;
        protected void Page_Load(object sender, EventArgs e)
        {
            long assetTypeId;
            long assetId;
            long.TryParse(Request.QueryString["AssetID"], out assetId);
            long.TryParse(Request.QueryString["AssetTypeID"], out assetTypeId);

            long assetTypeUid;
            long assetUid;
            long.TryParse(Request.QueryString["AssetUID"], out assetUid);
            long.TryParse(Request.QueryString["AssetTypeUID"], out assetTypeUid);

            long screenId;
            long.TryParse(Request.QueryString["ScreenId"], out screenId);

            if (assetTypeUid != 0 && assetUid != 0)
            {
                var currentTypeUid = AssetFactory.GetCurrentAssetTypeUid(assetTypeUid);
                if (currentTypeUid != null)
                    assetTypeUid = currentTypeUid.Value;

                AssetType = AssetTypeRepository.GetByUid(assetTypeUid);
                Asset = AssetsService.GetAssetByUid(assetUid, AssetType);
            }
            else if (assetTypeId != 0 && assetId != 0)
            {
                AssetType = AssetTypeRepository.GetById(assetTypeId);
                Asset = AssetsService.GetAssetById(assetId, AssetType);
                assetTypeUid = Asset.DynEntityConfigUid;
            }
            else
            {
                Response.Redirect("~/AssetView.aspx");
            }

            var permission = AuthenticationService.GetPermission(Asset);
            if (!permission.CanRead())
                Response.Redirect("~/AssetView.aspx");

            var screen = screenId != 0
                ? ScreensService.GetScreenById(screenId)
                : ScreensService.GetScreensByAssetTypeUid(AssetType.UID).Single(s => s.IsDefault);
            Dictionary<AttributePanel, List<AssetAttribute>> panels = AssetPanelsAdapter.GetPanelsByScreen(Asset, screen);
            _panelsCount = panels.Count();
            Repeater1.DataSource = panels;
            Repeater1.DataBind();

            var buttonCollection = new List<Enumerators.ToolbarButtonType>
            {
                Enumerators.ToolbarButtonType.Print,
                Enumerators.ToolbarButtonType.History
            };
            if (bool.Parse(Asset[AttributeNames.ActiveVersion].Value))
            {
                if (permission.CanWrite())
                    buttonCollection.Add(Enumerators.ToolbarButtonType.Edit);
                if (permission.CanDelete())
                    buttonCollection.Add(Enumerators.ToolbarButtonType.Delete);
            }
            else
            {
                buttonCollection.Add(Asset.IsDeleted
                    ? Enumerators.ToolbarButtonType.Restore
                    : Enumerators.ToolbarButtonType.CurrentVersion);
            }
            buttonCollection.Add(Enumerators.ToolbarButtonType.Documents);

            buttonCollection.ForEach(b =>
            {
                toolbar.ButtonCollection.Add(b);
                bottomtoolbar.ButtonCollection.Add(b);
            });

            if (Asset.IsDeleted)
            {
                if (toolbar[Enumerators.ToolbarButtonType.Restore] != null)
                    toolbar[Enumerators.ToolbarButtonType.Restore].OnButtonClick = OnRestoreAssetClick;
                if (bottomtoolbar[Enumerators.ToolbarButtonType.Restore] != null)
                    bottomtoolbar[Enumerators.ToolbarButtonType.Restore].OnButtonClick = OnRestoreAssetClick;
            }

            toolbar.Options.Add("AssetUID", Asset.UID.ToString());
            toolbar.Options.Add("AssetID", Asset.ID.ToString());
            toolbar.Options.Add("AssetTypeUID", AssetType.UID.ToString());
            toolbar.Options.Add("AssetTypeID", AssetType.ID.ToString());

            bottomtoolbar.Options.Add("AssetUID", Asset.UID.ToString());
            bottomtoolbar.Options.Add("AssetID", Asset.ID.ToString());
            bottomtoolbar.Options.Add("AssetTypeUID", AssetType.UID.ToString());
            bottomtoolbar.Options.Add("AssetTypeID", AssetType.ID.ToString());

            if (AssetType.ParentChildRelations)
            {
                caPanel.AssetId = Asset.ID;
                caPanel.AssetTypeId = AssetType.ID;
                caPanel.Visible = true;
            }

            #region Documents
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_DlgInitalize_" + this.ClientID,
                "$(function () {$('#" + this.DialogContainer.ClientID + "').dialog({ autoOpen: false, width: 420, height: 520 });});", true);
            bottomtoolbar.ExternalScript = toolbar.ExternalScript = "return ShowDialog('" + this.DialogContainer.ClientID + "');";

            repDocuments.DataSource = Asset.GetDocuments();
            repDocuments.DataBind();
            #endregion

            litAssetName.Text = this.Asset.Name;

            if (!IsPostBack)
            {
                TransactionTypes.DataSource = TransactionType.GetAll();
                TransactionTypes.DataBind();

                litRevision.Text = string.Format("[r.{0} - {1}]", Asset[AttributeNames.Revision].Value, Asset[AttributeNames.UpdateDate].Value);

                var taxonomyItems = UnitOfWork.TaxonomyItemRepository.GetTaxonomyItemsByAssetTypeId(AssetType.ID);
                foreach (var item in taxonomyItems)
                {
                    bool isCategory = false;
                    string path = this.GetParentName(item);

                    if (path.Contains("C>"))
                    {
                        isCategory = true;
                        path = path.Replace("C>", string.Empty);
                    }

                    IEnumerable<string> tempPath = path.Split(new char[1] { '>' }, StringSplitOptions.RemoveEmptyEntries).Reverse();
                    string nPath = string.Empty;
                    foreach (string pathElem in tempPath)
                    {
                        nPath += pathElem + " > ";
                    }

                    nPath += "<a href='../AssetView.aspx?AstType=" + AssetType.ID + "'>" + AssetType.Name + "</a>";

                    if (isCategory)
                        litCategoryPath.Text += nPath + "<br/>";
                    else
                        litTaxonomies.Text += nPath + "<br/>";

                }

                var relationAttributes = Asset.Attributes.Where(a => a.RelatedAsset != null);
                foreach (var attribute in relationAttributes)
                {
                    litLinkedAssets.Text += string.Format("{0}: <a href='..{1}'>{2}</a><br/>",
                        attribute.GetConfiguration().NameLocalized,
                        attribute.RelatedAsset.NavigateUrl,
                        attribute.RelatedAsset.GetDisplayName(
                            attribute.GetConfiguration().RelatedAssetTypeAttributeID.Value));
                }
            }

            litreservationInfo.Text = String.Empty;
            var service = new ReservationService(UnitOfWork, AuthenticationService);

            if (AssetType.AllowBorrow && bool.Parse(Asset[AttributeNames.ActiveVersion].Value))
            {
                var reservation = service.IsAssetBorrowed(Asset);
                if (reservation != null)
                {
                    litReserved.Text = "<strong><a class='assetTemplatePopupTrigger' style='cursor:pointer; color:Red; text-decoration:none;' rel='123'>Borrowed</a></strong>";
                    litreservationInfo.Text = GetReservationBorrowingInfo(reservation);
                }

                reservation = service.IsAssetReserved(Asset);
                if (reservation != null)
                {
                    litReserved.Text += "<strong><a class='assetTemplatePopupTrigger' style='cursor:pointer; color:Red; text-decoration:none;' rel='123'>Reserved</a></strong>";
                    litreservationInfo.Text = GetReservationBorrowingInfo(reservation);
                }
            }

            // if asset type used stock system, display transactions management block
            if (AssetType.IsInStock)
            {
                Transactions.Visible = true;
                if (!IsPostBack) ShowLocationsSelector();
            }

            #region SOBenBUB

            tasksPanel.Visible = ApplicationSettings.ApplicationType == ApplicationType.SOBenBUB || ApplicationSettings.ApplicationType == ApplicationType.Combined;
            tasksPanel.AssetType = AssetType;
            tasksPanel.AssetUID = Asset.UID;

            screensPanel.Visible = ApplicationSettings.ApplicationType == ApplicationType.SOBenBUB || ApplicationSettings.ApplicationType == ApplicationType.Combined;
            screensPanel.AssetType = AssetType;
            screensPanel.Asset = Asset;

            ReportsPanel.Reports.Add("Default Report", new Dictionary<string, object> { 
                {"AssetTypeId", AssetType.ID},
                {"AssetUid", Asset.UID},
                {"ReportType", (int)ReportType.AssetReport}
            });
            ReportsPanel.Reports.Add("Report with child assets", new Dictionary<string, object> { 
                {"AssetTypeId", AssetType.ID},
                {"AssetUid", Asset.UID},
                {"ReportType", (int)ReportType.AssetsWithChildsReport}
            });
            #endregion
        }

        private void OnRestoreAssetClick()
        {
            UnitOfWork.RestoreDeletedItem(Asset.ID, AssetType.ID);
            Response.Redirect(Request.Url.OriginalString);
        }

        public override string LayoutCssClass()
        {
            // hardcoded OneColumn layout;
            const string cssClass = ".panels_leftcol_container {width:100%;} .panels_rightcol_container { width:100%;}";

            return cssClass;
        }

        /// <summary>
        /// Transactions the type selected changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void TransactionTypeSelectedChanged(object sender, EventArgs e)
        {
            UserSelector.Visible = false;
            PriceInfo.Visible = false;
            LocationSelector.Visible = false;
            FromLocationSelector.Visible = false;

            switch (TransactionTypes.SelectedValue)
            {
                case "3":
                    ShowFromLocationsSelector();
                    ShowLocationsSelector();
                    break;
                case "2":
                    ShowFromLocationsSelector();
                    ShowUsersSelector();
                    break;
                case "1":
                    ShowLocationsSelector();
                    PriceInfo.Visible = true;
                    break;
            }
        }

        private void ShowUsersSelector()
        {
            UserSelector.Visible = true;
            UsersList.DataSource = AssetsService.GetIdNameListByAssetType(
                AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.User));
            UsersList.DataBind();
            UsersList.Attributes.Add("onchange", "MakeDescription();");
        }

        private void ShowFromLocationsSelector()
        {
            FromLocationSelector.Visible = true;
            UpdateLocationsList(FromLocationsList, true);
        }

        private void ShowLocationsSelector()
        {
            LocationSelector.Visible = true;
            UpdateLocationsList(TransactionLocationsList, false);
        }

        private void UpdateLocationsList(DropDownList control, bool showOnlyAvailable)
        {
            var locationsList = new List<KeyValuePair<long, string>>();
            var locationAssetType = AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Location);
            if (showOnlyAvailable)
            {
                if (Asset != null)
                {
                    var manager = new StockTransactionManager();
                    var available = manager.GetAvailableLocationsFor(Asset.ID, Asset.GetConfiguration().ID);
                    locationsList = AssetFactory.GetAssetsByIds(locationAssetType, available.Select(d => d.Key)).ToList();
                }
            }
            else
            {
                locationsList = AssetsService.GetIdNameListByAssetType(
                    AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Location)).ToList();
            }

            control.DataSource = locationsList;
            control.DataBind();
        }

        /// <summary>
        /// Called when "Add" button clicked (transaction)
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data</param>
        protected void OnAddTransactionClicked(object sender, EventArgs e)
        {
            CustomDataType @float = DataTypeService.GetByType(Enumerators.DataType.Float);
            //@int = CustomDataType.GetByType(Enumerators.DataType.Long);

            bool isCorrect = true;

            PriceErrorMsg.Text = string.Empty;
            CountErrorMsg.Text = string.Empty;
            AssetErr.Text = string.Empty;

            // first, validate all values
            if (PriceInfo.Visible)
            {
                if (string.IsNullOrEmpty(TotalPrice.Text))
                {
                    isCorrect = false;
                    PriceErrorMsg.Text = "Not filled price";
                }
                else
                {
                    var priceValResult = ValidationService.ValidateDataType(@float, TotalPrice.Text);
                    if (!priceValResult.IsValid)
                    {
                        isCorrect = false;
                        PriceErrorMsg.Text = priceValResult.GetErrorMessage("<br />");
                    }
                }
            }

            long? locationId = 0;
            if (LocationSelector.Visible)
            {
                locationId = GetDropDownValue(TransactionLocationsList);
                isCorrect |= locationId.HasValue;
            }

            long? fromLocationId = null;
            if (FromLocationSelector.Visible)
            {
                fromLocationId = GetDropDownValue(FromLocationsList);
                isCorrect |= fromLocationId.HasValue;
            }

            long? outUserId = null;
            if (UserSelector.Visible)
            {
                outUserId= GetDropDownValue(UsersList);
                isCorrect |= outUserId.HasValue;
            }

            if (string.IsNullOrEmpty(TotalCount.Text))
            {
                isCorrect = false;
                CountErrorMsg.Text = "Not filled count";
            }
            else
            {
                var countValResult = ValidationService.ValidateDataType(@float, TotalCount.Text);
                if (!countValResult.IsValid)
                {
                    isCorrect = false;
                    CountErrorMsg.Text = countValResult.GetErrorMessage("<br />");
                }
            }

            if (isCorrect)
            {
                TransactionTypeCode tcode = (TransactionTypeCode)long.Parse(TransactionTypes.SelectedValue);
                decimal scount = decimal.Parse(TotalCount.Text);
                decimal sprice = 0;
                decimal.TryParse(TotalPrice.Text, out sprice);

                StockTransaction trans = null;

                switch (TransactionTypes.SelectedValue)
                {
                    case "1":
                        trans = new StockTransactionIn(AuthenticationService, AssetsService,
                            this.AssetType.ID, this.Asset.ID, scount, sprice, Description.Text, locationId);
                        break;

                    case "2":
                        trans = new StockTransactionOut(AuthenticationService, AssetsService, 
                            this.AssetType.ID, this.Asset.ID, scount, sprice, Description.Text, outUserId, fromLocationId);
                        break;

                    case "3":
                        trans = new StockTransactionMove(AuthenticationService, AssetsService, 
                            this.Asset.ID, this.AssetType.ID, this.Asset.ID, scount, sprice, Description.Text, locationId, fromLocationId);
                        break;
                }

                ValidationResult res = trans.Validate();
                if (res.IsValid)
                {
                    trans.Commit();
                    Response.Redirect(string.Format("ViewTransactions.aspx?AssetUID={0}&AssetTypeUID={1}", Asset.UID, AssetType.UID));
                }
                else
                {
                    ValidationReport.Text = res.GetErrorMessage("<br />");
                }
            }
        }

        private long? GetDropDownValue(DropDownList dropDownList)
        {
            long result = 0;
            if (!long.TryParse(dropDownList.SelectedValue, out result))
                return null;
            return result;
        }

        /// <summary>
        /// Departments the list data bound.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DepartmentListDataBound(object sender, EventArgs e)
        {
            UsersList.Items.Insert(0, new ListItem("Select...", "0"));
        }

        protected void TransactionLocationsListDataBound(object sender, EventArgs e)
        {
            TransactionLocationsList.Items.Insert(0, new ListItem("Select...", "0"));
            UpdateStockRest();
        }

        private string GetParentName(TaxonomyItem item)
        {
            if (item.ParentItem == null)
            {
                if (item.Taxonomy.IsCategory)
                    return "C>" + item.Name;
                return item.Name;
            }

            if (item.Taxonomy.IsCategory)
                return String.Format("C>{0}>{1}", item.Name, this.GetParentName(item.ParentItem));
            return String.Format("{0}>{1}", item.Name, this.GetParentName(item.ParentItem));
        }

        public string GetReservationBorrowingInfo(Reservation reservation)
        {
            var result = new StringBuilder();
            if (reservation == null) 
                return string.Empty;

            var userType = AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.User);
            var borrower = AssetsService.GetAssetById(reservation.Borrower, userType);
            var reserver = AssetsService.GetAssetById(reservation.UpdateUserId, userType);
            result.Append(String.Format("{0}{1}<br/>", 
                (reservation.IsBorrowed ? "Borrowed by: " : "Reserved to: "), borrower[AttributeNames.Name].Value));
            result.Append(String.Format("Reserved by: {0}<br/>", reserver[AttributeNames.Name].Value));
            result.Append(String.Format("From: {0}<br/>", reservation.StartDate.ToShortDateString()));
            result.Append(String.Format("Till: {0}<br/>", reservation.EndDate.ToShortDateString()));
            result.Append(String.Format("Reserved at: {0}<br/>", reservation.ReservationDate.ToShortDateString()));
            if (!string.IsNullOrEmpty(reservation.Reason))
                result.Append(String.Format("Reason: {0}<br/>", reservation.Reason));
            return result.ToString();
        }

        protected void OnlbtnExportToTxt_Click(object sender, EventArgs e)
        {
            var exporter = new Exporter(AssetTypeRepository, AssetsService);
            using (var rdr = new StreamReader(exporter.ExportToTxt(this.Asset)))
            {
                var content = rdr.ReadToEnd();
                rdr.Close();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "export" + DateTime.Now.ToShortTimeString().Replace(".", "_") + ".txt");
                Response.AddHeader("Content-Length", content.Length.ToString());
                Response.ContentType = "text/plain";
                Response.Write(content);
                Response.End();
            }
        }

        protected void OnlbtnExportToXml_Click(object sender, EventArgs e)
        {
            using (var rdr = new StreamReader(Exporter.ExportToXml(this.Asset)))
            {
                var content = rdr.ReadToEnd();
                rdr.Close();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "export" + DateTime.Now.ToShortTimeString().Replace(".", "_") + ".xml");
                Response.AddHeader("Content-Length", content.Length.ToString());
                Response.ContentType = "xml";
                Response.Write(content);
                Response.End();
            }
        }

        protected void OnlbtnExportToDoc_Click(object sender, EventArgs e)
        {
            var tempFilePath = string.Empty;
            var exporter = new Exporter(AssetTypeRepository, AssetsService);
            using (var stream = exporter.ExportTodoc(this.Asset, Server.MapPath("~"), out tempFilePath))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();
                File.Delete(tempFilePath);
                Response.AddHeader("Content-Disposition",
                                   "attachment; filename=" + "export" +
                                   DateTime.Now.ToShortTimeString().Replace(".", "_") + ".docx");
                Response.AddHeader("Content-Length", buffer.Length.ToString());
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                //Response.ContentType = "application/msword";
                Response.BinaryWrite(buffer);
                Response.End();
            }
        }

        protected void OnlbtnExportToZip_Click(object sender, EventArgs e)
        {
            string tempFolder = Server.MapPath("~") + "//Temp//" + "export" + DateTime.Now.ToLongTimeString().Replace(".", "_").Replace(":", "_");
            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);
            var exporter = new Exporter(AssetTypeRepository, AssetsService);

            using (StreamWriter writer = File.CreateText(tempFolder + "//export.txt"))
            {
                StreamReader rdr = new StreamReader(exporter.ExportToTxt(this.Asset));
                writer.Write(rdr.ReadToEnd());
                rdr.Close();
            }
            using (StreamWriter writer = File.CreateText(tempFolder + "//export.xml"))
            {
                StreamReader rdr = new StreamReader(Exporter.ExportToXml(this.Asset));
                writer.Write(rdr.ReadToEnd());
                rdr.Close();
            }

            string docxPath = string.Empty;
            Stream docxStream = exporter.ExportTodoc(this.Asset, Server.MapPath("~"), out docxPath);
            docxStream.Close();

            File.Copy(docxPath, tempFolder + "//export.doc");

            string zipName = "export" + DateTime.Now.ToLongTimeString().Replace(".", "_").Replace(":", "_") + ".zip";
            string zipPath = Server.MapPath("~") + "//Temp//" + zipName;

            wFastZip fz = new wFastZip();
            fz.CompressionLevel = 9;
            fz.CreateZip(zipPath, tempFolder, true, null, null);
            File.Delete(docxPath);

            FileInfo fi = new FileInfo(zipPath);

            Response.AddHeader("Content-Disposition", "attachment; filename=" + zipName);
            Response.AddHeader("Content-Length", fi.Length.ToString());
            Response.ContentType = "application/zip";
            Response.WriteFile(zipPath);

            Directory.Delete(tempFolder, true);
            //fi.Delete();

            Response.End();
        }

        protected void FromLocationsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStockRest();
        }

        private void UpdateStockRest()
        {
            if (Asset != null)
            {
                var manager = new StockTransactionManager();
                var available = manager.GetAvailableLocationsFor(Asset.ID, Asset.GetConfiguration().ID);

                long locationId = long.Parse(FromLocationsList.SelectedValue);
                if (available.ContainsKey(locationId))
                {
                    var rest = available[locationId];
                    CountInfo.Text = string.Format("{0} of {1}", AssetType.MeasureUnit.Name, rest);
                }
                else
                {
                    CountInfo.Text = "Out of stock";
                }
            }
        }

        protected void Repeater1_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) 
                return;

            var panel = e.Item.FindControl("Panel1") as NewAttributePanel;
            panel.AuthenticationService = AuthenticationService;
            panel.AssetTypeRepository = AssetTypeRepository;
            panel.AssetsService = AssetsService;
            panel.UnitOfWork = UnitOfWork;
            panel.AttributeFieldFactory = AttributeFieldFactory;
        }
    }
}