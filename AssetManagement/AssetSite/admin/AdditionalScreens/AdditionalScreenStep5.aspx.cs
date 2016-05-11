using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Extensions;
using AppFramework.Core.Services;
using AppFramework.Entities;
using AssetManager.Infrastructure.Services;
using Microsoft.Practices.Unity;

namespace AssetSite.admin.AdditionalScreens
{
    public partial class AdditionalScreenStep5 : ScreensController
    {
        protected class AssetTypeConfiguration
        {
            public DynEntityConfig Configuration { get; set; }
            public string Name { get; set; }
            public long ReferencingDynEntityAttribConfigId { get; set; }
            public string ReferencingDynEntityAttribConfigName { get; set; }
            public bool IgnoreValidation { get; set; }
        }

        [Dependency]
        public IPanelsService PanelsService { get; set; }

        [Dependency]
        public IAssetTypeService AssetTypeService { get; set; }

        public Dictionary<long, List<long>> AttributesToAttribute = new Dictionary<long, List<long>>();

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                // show configurations of related assets
                var configurations = new List<AssetTypeConfiguration>();

                // load all the panels of the screen
                var panels = PanelsService.GetAllByScreenId(_currentScreen.ScreenId);

                // show configurations of child assets
                var panelChildAttribIds = new HashSet<long>();
                var assetChildAttribs = AssetTypeService
                    .GetChildAttribs(AuthenticationService.CurrentUserId, _currentType.ID)
                    .ToList();

                foreach (var childAssetPanel in panels.Where(p => p.IsChildAssets))
                {
                    panelChildAttribIds.Add(childAssetPanel.ChildAssetAttrId.Value);
                    var panelAttrib =
                        assetChildAttribs.Single(a => a.DynEntityAttribConfigId == childAssetPanel.ChildAssetAttrId);

                    // careful! updating a property of a trackable antity
                    childAssetPanel.Name = string.Format("{0} [{1} - ({2})]", childAssetPanel.Name,
                        panelAttrib.DynEntityConfig.Name, panelAttrib.Name);
                }
                if (panelChildAttribIds.Count > 0)
                {

                    configurations = configurations
                        .Union(assetChildAttribs
                                .Where(a => panelChildAttribIds.Contains(a.DynEntityAttribConfigId))
                                .Select(a => new AssetTypeConfiguration
                                {
                                    Configuration = a.DynEntityConfig,
                                    Name = string.Format("{0} ({1})", a.DynEntityConfig.Name, a.Name),
                                    ReferencingDynEntityAttribConfigId = a.DynEntityAttribConfigId,
                                    ReferencingDynEntityAttribConfigName = a.DynEntityConfig.Name, // this is added to the name of a list item: "attrib name (attrib config name)"
                                    IgnoreValidation = true
                                }))
                        .ToList();
                }

                configurations.ForEach(c => UnitOfWork.DynEntityConfigRepository
                    .LoadProperty(c.Configuration, d => d.DynEntityAttribConfigs));
                configurations.Insert(0, new AssetTypeConfiguration
                {
                    Configuration = _currentType.Base,
                    Name = _currentType.Name,
                    ReferencingDynEntityAttribConfigId = 0L,
                    ReferencingDynEntityAttribConfigName = _currentType.Name
                });

                var attributes = panels
                    .SelectMany(p => p.AttributePanelAttribute)
                    .GroupBy(a => a.ReferencingDynEntityAttribConfigId)
                    .Select(g => new
                    {
                        ReferencingDynEntityAttribConfigId = g.Key == null ? 0L : (long) g.Key,
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
            var lit = (Literal) e.Item.FindControl("litScript");
            var lst = (ListBox) e.Item.FindControl("lstPanelAttrib");
            var currentPanel = (AttributePanel) e.Item.DataItem;

            var script = "<script type='text/javascript'>{0}</script>";
            var innerScript = "var mvr" + currentPanel.AttributePanelId + "=new Mover('" + lst.ClientID + "');";

            lit.Text = string.Format(script, innerScript);

            lst.Items.Clear();

            var attributesSource = UnitOfWork.DynEntityAttribConfigRepository.AsQueryable();
            var configsSource = UnitOfWork.DynEntityConfigRepository.AsQueryable();

            var parentNames = new Dictionary<long, string>();

            foreach (var attr in currentPanel.AttributePanelAttribute.OrderBy(a => a.DisplayOrder))
            {
                string parentName;
                if (currentPanel.IsChildAssets)
                {
                    parentName = (
                        from a in attributesSource
                        from d in configsSource
                        where a.DynEntityAttribConfigUid == attr.DynEntityAttribConfigUId
                            && a.DynEntityConfigUid == d.DynEntityConfigUid
                            && a.ActiveVersion && a.Active
                            && d.ActiveVersion && d.Active
                        select d.Name)
                        .FirstOrDefault();
                }
                else if (attr.ReferencingDynEntityAttribConfigId.HasValue && attr.ReferencingDynEntityAttribConfigId > 0)
                {
                    if (!parentNames.ContainsKey(attr.ReferencingDynEntityAttribConfigId.Value))
                        parentNames.Add(attr.ReferencingDynEntityAttribConfigId.Value,
                            AttributeRepository.GetPublishedById(attr.ReferencingDynEntityAttribConfigId.Value)
                                .NameLocalized());
                    parentName = parentNames[attr.ReferencingDynEntityAttribConfigId.Value];
                }
                else 
                {
                    parentName = _currentType.Name;
                }

                var itm = new ListItem();
                itm.Text = string.Format("{0} ({1})",
                    new TranslatableString(attr.DynEntityAttribConfig.Name).GetTranslation(),
                    parentName);

                itm.Value = string.Format("{0}:{1}:{2}",
                    attr.DynEntityAttribConfigUId,
                    attr.ReferencingDynEntityAttribConfigId,
                    attr.DynEntityAttribConfig.IsRequired && !currentPanel.IsChildAssets ? "*" : string.Empty);
                lst.Items.Add(itm);
            }
        }

        #region Binding Helpers

        public string GetAddScript(object id)
        {
            return "return mvr" + id + ".AddItem();";
        }

        public string GetRemoveScript(object id)
        {
            return "return mvr" + id + ".RemoveItem();";
        }

        public string GetTopScript(object id)
        {
            return "return mvr" + id + ".MoveTop();";
        }

        public string GetUpScript(object id)
        {
            return "return mvr" + id + ".Up();";
        }

        public string GetDownScript(object id)
        {
            return "return mvr" + id + ".Down();";
        }

        public string GetBottomScript(object id)
        {
            return "return mvr" + id + ".MoveBottom();";
        }

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
            var sets = hfldCollectedData.Value
                .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var set in sets)
            {
                var pidRE = new Regex(@"\(\d+\)");
                var pidMatch = pidRE.Match(set);
                if (pidMatch.Success)
                {
                    var panelUid = long.Parse(pidMatch.Value.Trim('(', ')'));
                    var apas = UnitOfWork
                        .AttributePanelAttributeRepository
                        .Where(apa => apa.AttributePanelUid == panelUid);

                    // DynEntityAttribConfigUid:ContainerId:isRequired;
                    var pairRE = new Regex(@"\d+\:\d+:\*?;");
                    var pairMatch = pairRE.Match(set);
                    var order = 0;

                    while (pairMatch.Success)
                    {
                        var parts = pairMatch.Value.TrimEnd(';', '*')
                            .Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);

                        var dynEntityAttribConfigUid = long.Parse(parts[0]);
                        var referencingDynEntityAttribConfigId = long.Parse(parts[1]);

                        // get existing AttributePanelAttribute or create new if there is no existing
                        // it's important to have same Uids for AttributePanelAttributes because screen formulas have links for it
                        var apaEntity = apas
                            .SingleOrDefault(apa => apa.AttributePanelUid == panelUid &&
                                        apa.DynEntityAttribConfigUId == dynEntityAttribConfigUid &&
                                        apa.ReferencingDynEntityAttribConfigId == referencingDynEntityAttribConfigId);

                        var isNew = apaEntity == null;
                        if (isNew)
                        {
                            apaEntity = new AttributePanelAttribute();
                        }
                        else
                        {
                            apas.Remove(apaEntity);
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

                    UnitOfWork.AttributePanelAttributeRepository.Delete(apas);
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