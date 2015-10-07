using AppFramework.Core.Classes.ScreensServices;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Calculation;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    /// <summary>
    /// Provides interface between list of asset attibutes and their 
    /// presentation on panels, which described by asset's configuration.
    /// </summary>
    public class AssetPanelsAdapter : AppFramework.Core.Classes.IAssetPanelsAdapter
    {
        public IDictionary<AssetAttribute, Asset> DependencyDescriptor { get; private set; }
       
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;
        private readonly IScreensService _screensService;

        /// <summary>
        /// Provides interface between list of asset attibutes and their 
        /// presentation on panels
        /// </summary>
        public AssetPanelsAdapter(
            IUnitOfWork unitOfWork,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService,
            IScreensService screensService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (screensService == null)
                throw new ArgumentNullException("screensService");
            _screensService = screensService;

            DependencyDescriptor = new Dictionary<AssetAttribute, Asset>();
        }

        /// <summary>
        /// Returns list of panels with asset attributes based on asset's configuration.
        /// </summary>
        /// <returns></returns>
        public Dictionary<AttributePanel, List<AssetAttribute>> GetPanelsByScreen(Asset asset, Entities.AssetTypeScreen screen)
        {
            if (asset == null)
                throw new ArgumentNullException("asset");
            if (screen == null)
                throw new ArgumentNullException("screen");

            // collect all assets with current one
            LoadRelatedAssets(asset, screen);
            var panels = LoadScreenPanels(asset, screen);
            return panels;
        }

        private Dictionary<AttributePanel, List<AssetAttribute>> LoadScreenPanels(Asset asset, Entities.AssetTypeScreen screen)
        {
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
                        attribute = asset.Attributes.SingleOrDefault(a => a.Configuration.ID == apa.DynEntityAttribConfig.DynEntityAttribConfigId);
                    }
                    else
                    {
                        // attribute relates to one of linked assets, which itself linked to some particular attribute of main asset
                        var parentAttribute = asset.Attributes.SingleOrDefault(a => a.GetConfiguration().ID == apa.ReferencingDynEntityAttribConfigId);
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
                                    var assetAttribute = assignedAttributes.Single(a => a == parentAttribute) as AssetAttribute;
                                    if (assetAttribute != null)
                                    {
                                        assetAttribute.CustomMultipleAssetsFields
                                                      .Add(apa.DynEntityAttribConfig.DBTableFieldname);
                                    }
                                }
                            }
                        }
                        
                    }
                    if (attribute == null)
                        continue;

//todo:                    attribute.ScreenFormula = apa.ScreenFormula;
                    assignedAttributes.Add(attribute);
                }
                panels[panel] = assignedAttributes;
            }
            return panels;
        }

        private void LoadRelatedAssets(Asset asset, Entities.AssetTypeScreen screen)
        {
            // load related assets screens
            _unitOfWork.AssetTypeScreenRepository.LoadProperty(screen, e => e.DynEntityAttribScreens);

            foreach (var attRel in screen.DynEntityAttribScreens)
            {                
                var attribute = asset.Attributes.SingleOrDefault(a => a.GetConfiguration().UID == attRel.DynEntityAttribUid);                

                if (attribute == null)
                    continue;
                if (attribute.GetConfiguration().RelatedAssetTypeID == null)
                    continue;
                if (attribute.GetConfiguration().IsMultipleAssets)
                    continue;
                                                
                var relatedAssetType =
                    _assetTypeRepository.GetById(attribute.Configuration.RelatedAssetTypeID.Value);

                if (relatedAssetType == null)
                    throw new NullReferenceException("Cannot find related assettype");

                Asset relAsset = null;

                if (asset.IsNew)
                {
                    relAsset = _assetsService.CreateAsset(relatedAssetType);
                }
                else
                {
                    if (attribute.GetConfiguration().IsAsset && attribute.ValueAsId.HasValue && attribute.ValueAsId != 0)
                    {
                        if (!asset.IsHistory)
                        {
                            relAsset = _assetsService.GetAssetById(attribute.ValueAsId.Value, relatedAssetType);
                        }
                        else
                        {
                            relAsset = _assetsService.GetAssetByUid(attribute.ValueAsId.Value, relatedAssetType);
                        }
                    }
                }

                if (relAsset != null)
                    DependencyDescriptor.Add(attribute, relAsset);
            }
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
