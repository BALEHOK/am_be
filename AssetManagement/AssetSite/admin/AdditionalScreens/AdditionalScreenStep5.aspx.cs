using AppFramework.Core.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.Entities;
using Microsoft.Practices.Unity;
using AppFramework.Core.ConstantsEnumerators;

namespace AssetSite.admin.AdditionalScreens
{
    public partial class AdditionalScreenStep5 : ScreensController
    {
        [Dependency]
        public IPanelsService PanelsService { get; set; }

        public Dictionary<long, List<long>> AttributesToAttribute = new Dictionary<long, List<long>>();

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                var linkedAssetTypeAttributesUids = _currentScreen
                    .DynEntityAttribScreens
                    .Select(a => a.DynEntityAttribUid)
                    .ToList();

                // fixture to hide attributes which are formely linked assets
                AttributesToAttribute.Add(0, linkedAssetTypeAttributesUids); 

                var attributesSource = UnitOfWork.DynEntityAttribConfigRepository.AsQueryable();
                var configsSource = UnitOfWork.DynEntityConfigRepository.AsQueryable();
                var configurations = (from a in attributesSource
                                      from d in configsSource
                                      where linkedAssetTypeAttributesUids.Contains(a.DynEntityAttribConfigUid)
                                        && a.RelatedAssetTypeID != null
                                        && a.RelatedAssetTypeID == d.DynEntityConfigId
                                        && d.ActiveVersion
                                      select new { Configuration = d, Name = a.Name, ReferencingDynEntityAttribConfigId = a.DynEntityAttribConfigId, ReferencingDynEntityAttribConfigName = a.Name })
                                          .ToList();

                configurations.ForEach(c => UnitOfWork.DynEntityConfigRepository
                    .LoadProperty(c.Configuration, d => d.DynEntityAttribConfigs));
                configurations.Insert(0, new
                {
                    Configuration = _currentType.Base,
                    Name = _currentType.Name,
                    ReferencingDynEntityAttribConfigId = default(long),
                    ReferencingDynEntityAttribConfigName = _currentType.Name
                });

                var panels = PanelsService.GetAllByScreenId(_currentScreen.ScreenId)
                    .OrderBy(p => p.DisplayOrder)
                    .ThenBy(p => p.UID);

                var attributes = panels
                    .SelectMany(p => p.Base.AttributePanelAttribute)
                    .GroupBy(a => a.ReferencingDynEntityAttribConfigId)
                    .Select(g => new
                    {
                        ReferencingDynEntityAttribConfigId = g.Key == null ? default(long) : (long)g.Key,
                        AttributesUids = g.Select(a => a.DynEntityAttribConfigUId).ToList()
                    });

                foreach (var attr in attributes)
                {
                    if (AttributesToAttribute.ContainsKey(attr.ReferencingDynEntityAttribConfigId))
                        AttributesToAttribute[attr.ReferencingDynEntityAttribConfigId]
                            .AddRange(attr.AttributesUids);
                    else
                        AttributesToAttribute
                            .Add(attr.ReferencingDynEntityAttribConfigId, attr.AttributesUids);
                }

                repATAttributes.DataSource = configurations;
                repATAttributes.DataBind();

                repPanels.DataSource = panels;
                repPanels.DataBind();

                btnFinish.OnClientClick = "return CollectData('" + hfldCollectedData.ClientID + "');";
            }
        }

        protected void OnPanelDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            Literal lit = (Literal)e.Item.FindControl("litScript");
            ListBox lst = (ListBox)e.Item.FindControl("lstPanelAttrib");
            var currentPanel = ((AppFramework.Core.Classes.Panel)e.Item.DataItem).Base;

            string script = "<script type='text/javascript'>{0}</script>";
            string innerScript = "var mvr" + currentPanel.AttributePanelId + "=new Mover('" + lst.ClientID + "');";

            lit.Text = String.Format(script, innerScript);

            lst.Items.Clear();

            var parentNames = new Dictionary<long, string>();

            foreach (var attr in currentPanel.AttributePanelAttribute.OrderBy(a => a.DisplayOrder))
            {
                string parentName = string.Empty;
                if (attr.ReferencingDynEntityAttribConfigId.HasValue && attr.ReferencingDynEntityAttribConfigId > 0)
                {
                    if (!parentNames.ContainsKey(attr.ReferencingDynEntityAttribConfigId.Value))
                        parentNames.Add(attr.ReferencingDynEntityAttribConfigId.Value,
                            AssetTypeRepository.GetAttributeById(attr.ReferencingDynEntityAttribConfigId.Value).NameLocalized);
                    parentName = parentNames[attr.ReferencingDynEntityAttribConfigId.Value];
                }
                else
                {
                    parentName = _currentType.Name;
                }

                ListItem itm = new ListItem();
                itm.Text = string.Format("{0} ({1})",
                    new TranslatableString(attr.DynEntityAttribConfig.Name).GetTranslation(),
                    parentName);

                itm.Value = string.Format("{0}:{1}:{2}",
                    attr.DynEntityAttribConfigUId,
                    attr.ReferencingDynEntityAttribConfigId,
                    attr.DynEntityAttribConfig.IsRequired ? "*" : string.Empty);
                lst.Items.Add(itm);
            }
        }

        #region Binding Helpers
        public string GetAddScript(object id) { return "return mvr" + id + ".AddItem();"; }
        public string GetRemoveScript(object id) { return "return mvr" + id + ".RemoveItem();"; }

        public string GetTopScript(object id) { return "return mvr" + id + ".MoveTop();"; }
        public string GetUpScript(object id) { return "return mvr" + id + ".Up();"; }

        public string GetDownScript(object id) { return "return mvr" + id + ".Down();"; }
        public string GetBottomScript(object id) { return "return mvr" + id + ".MoveBottom();"; }

        public string GetPanelDivId(object id)
        {
            return "panelDiv" + id;
        }
        #endregion

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("AdditionalScreenStep4.aspx?ScreenId={0}&atuid={1}",
                _currentScreen.ScreenId, _currentType.UID));
        }

        protected void btnFinish_Click(object sender, EventArgs e)
        {            
            string[] sets = hfldCollectedData.Value
                .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string set in sets)
            {
                Regex pidRE = new Regex(@"\(\d+\)");
                Match pidMatch = pidRE.Match(set);
                if (pidMatch.Success)
                {
                    long panelUid = long.Parse(pidMatch.Value.Trim(new[] {'(', ')'}));

                    // DynEntityAttribConfigUid:ContainerId:isRequired;
                    Regex pairRE = new Regex(@"\d+\:\d+:\*?;");
                    Match pairMatch = pairRE.Match(set);
                    int order = 0;

                    while (pairMatch.Success)
                    {
                        string[] parts = pairMatch.Value.TrimEnd(new[] {';', '*'})
                            .Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);

                        var dynEntityAttribConfigUid = long.Parse(parts[0]);
                        var referencingDynEntityAttribConfigId = long.Parse(parts[1]);

                        // get existing AttributePanelAttribute or create new if there is no existing
                        // it's important to have same Uids for AttributePanelAttributes because screen formulas have links for it
                        var apaEntity = UnitOfWork
                            .AttributePanelAttributeRepository
                            .Get(apa => apa.AttributePanelUid == panelUid &&
                                        apa.DynEntityAttribConfigUId == dynEntityAttribConfigUid &&
                                        apa.ReferencingDynEntityAttribConfigId == referencingDynEntityAttribConfigId)
                            .SingleOrDefault();

                        var isNew = apaEntity == null;
                        if (isNew)
                        {
                            apaEntity = new AttributePanelAttribute();
                        }

                        apaEntity.DynEntityAttribConfigUId = dynEntityAttribConfigUid;
                        apaEntity.ReferencingDynEntityAttribConfigId = referencingDynEntityAttribConfigId;
                        apaEntity.AttributePanelUid = panelUid;
                        apaEntity.UpdateDate = DateTime.Now;
                        apaEntity.UpdateUserId = AuthenticationService.CurrentUserId;
                        apaEntity.DisplayOrder = order++;

                        if (isNew)
                            UnitOfWork.AttributePanelAttributeRepository.Insert(apaEntity);
                        else
                            UnitOfWork.AttributePanelAttributeRepository.Update(apaEntity);
                        
                        pairMatch = pairMatch.NextMatch();
                    }
                }
            }

            if (_currentScreen.IsDefault)
            {
                var screens = UnitOfWork
                    .AssetTypeScreenRepository
                    .Where(s => s.DynEntityConfigUid == _currentScreen.DynEntityConfigUid)
                    .ToList();

                screens.ForEach(s =>
                    {
                        if (s.ScreenId != _currentScreen.ScreenId)
                        {
                            s.IsDefault = false;
                            UnitOfWork.AssetTypeScreenRepository.Update(s);
                        }
                    });
            }
            UnitOfWork.Commit();
            if (!TryRedirectToBatch())
                Response.Redirect("~/Wizard/EditAssetType.aspx");
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            if (!TryRedirectToBatch())
                Response.Redirect("~/Wizard/EditAssetType.aspx");
        }
    }
}