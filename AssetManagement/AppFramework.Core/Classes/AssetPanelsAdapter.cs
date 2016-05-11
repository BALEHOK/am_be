using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    /// <summary>
    /// Provides interface between list of asset attibutes and their 
    /// presentation on panels, which described by asset's configuration.
    /// </summary>
    public class AssetPanelsAdapter : IAssetPanelsAdapter
    {
        public IDictionary<AssetAttribute, Asset> DependencyDescriptor { get; private set; }
       
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Provides interface between list of asset attibutes and their 
        /// presentation on panels
        /// </summary>
        public AssetPanelsAdapter(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;

            DependencyDescriptor = new Dictionary<AssetAttribute, Asset>();
        }

        /// <summary>
        /// Returns list of panels with asset attributes based on asset's configuration.
        /// </summary>
        /// <returns></returns>
        public Dictionary<AttributePanel, List<AssetAttribute>> GetPanelsByScreen(
            AssetWrapperForScreenView assetWrapper, AssetTypeScreen screen)
        {
            if (assetWrapper == null)
                throw new ArgumentNullException("assetWrapper");
            if (screen == null)
                throw new ArgumentNullException("screen");

            var panels = new Dictionary<AttributePanel, List<AssetAttribute>>();
            foreach (var panel in screen.AttributePanel.OrderBy(p => p.DisplayOrder).ThenBy(p => p.AttributePanelId))
            {
                var assignedAttributes = new List<AssetAttribute>();
                foreach (var apa in panel.AttributePanelAttribute.OrderBy(apa => apa.DisplayOrder))
                {
                    AssetAttribute attribute = null;
                    if (apa.ReferencingDynEntityAttribConfigId == null || apa.ReferencingDynEntityAttribConfigId == 0)
                    {
                        // attribute relates to the main asset                             
                        attribute = assetWrapper
                            .ScreenAttributes(panel.ScreenId.Value)
                            .SingleOrDefault(a => a.Configuration.ID == apa.DynEntityAttribConfig.DynEntityAttribConfigId);
                    }
                    else
                    {
                        // attribute relates to one of linked assets, which itself linked to some particular attribute of main asset
                        var parentAttribute = assetWrapper
                            .ScreenAttributes(panel.ScreenId.Value)
                            .SingleOrDefault(a => a.GetConfiguration().ID == apa.ReferencingDynEntityAttribConfigId);
                        
                        if (parentAttribute != null)
                        {
                            if (parentAttribute.GetConfiguration().IsAsset && DependencyDescriptor.ContainsKey(parentAttribute))
                            {
                                // look for an attribute among all attributes of asset, which is linked to parent attribute (attribute of main asset)
                                attribute = DependencyDescriptor[parentAttribute]
                                                .Attributes
                                                    .SingleOrDefault(attr => attr.GetConfiguration().ID == apa.DynEntityAttribConfig.DynEntityAttribConfigId);
                            }
                            else if (parentAttribute.GetConfiguration().IsMultipleAssets)
                            {
                                // trick to render MultipleAssets control with some pre-tweak
                                if (!assignedAttributes.Contains(parentAttribute))
                                {
                                    attribute = parentAttribute;
                                    attribute.CustomMultipleAssetsFields = new List<string>
                                        {
                                            apa.DynEntityAttribConfig.DBTableFieldname
                                        };
                                }
                                else
                                {
                                    var assetAttribute = assignedAttributes.Single(a => a == parentAttribute);
                                    assetAttribute.CustomMultipleAssetsFields
                                        .Add(apa.DynEntityAttribConfig.DBTableFieldname);
                                }
                            }
                        }
                        
                    }
                    if (attribute == null)
                        continue;

//todo:                    attribute.ScreenFormula = apa.ScreenFormula;
                    // no! never do like that ^ because attribute is a data model, it belongs to asset,
                    // but apa is a part of... view model. multiple apas can reference the same attribute
                    assignedAttributes.Add(attribute);
                }
                panels[panel] = assignedAttributes;
            }
            return panels;
        }

        public Dictionary<AttributePanel, List<AssetAttribute>> GetDefaultPanels(Asset asset)
        {
            long atUid = asset.GetConfiguration().UID;
            var ib = new IncludesBuilder<AttributePanel>();
            ib.Add(ap => ap.AttributePanelAttribute);
            var panelEntities = _unitOfWork.AttributePanelRepository
                .Where(ap => ap.DynEntityConfigUId == atUid && ap.ScreenId == null, ib.Get())
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.AttributePanelId)
                .ToList();

            var panels = new Dictionary<AttributePanel, List<AssetAttribute>>();
            foreach (var panel in panelEntities)
            {
                panels[panel] = (from apa in panel.AttributePanelAttribute
                                            from aatr in asset.Attributes
                                            where apa.DynEntityAttribConfigUId == aatr.GetConfiguration().UID
                                            orderby apa.DisplayOrder
                                            select aatr)
                                           .ToList();
            }
            return panels;
        }
    }
}
