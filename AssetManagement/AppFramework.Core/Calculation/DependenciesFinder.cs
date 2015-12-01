using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using QuickGraph;
using QuickGraph.Algorithms;

namespace AppFramework.Core.Calculation
{
    public class DependenciesFinder
    {
        private readonly IAssetTypeRepository _typeRepository;
        private readonly IAttributeRepository _attributeRepository;
        private readonly IAssetsService _assetsService;

        public DependenciesFinder(IAssetsService assetsService,
            IAssetTypeRepository assetTypeRepository, IAttributeRepository attributeRepository)
        {
            _typeRepository = assetTypeRepository;
            _attributeRepository = attributeRepository;
            _assetsService = assetsService;
        }

        #region related dependencies

        private List<long> GetRelatedUids(Asset asset)
        {
            var exclusions = new List<string> { "User", "Owner", "Update User" };
            var relatedAttributes =
                asset.Attributes.Where(a => a.RelatedAsset != null && !exclusions.Contains(a.Configuration.Name))
                    .ToList();

            var relatedUids = relatedAttributes.Select(a => a.RelatedAsset.UID).ToList();
            return relatedUids;
        }

        public void GetDependentAssets(Asset asset, Stack<Asset> saveStack, List<Asset> dependencies = null)
        {
            var assetType = asset.GetConfiguration();
            var type = _typeRepository.GetByUid(assetType.UID);

            var parameterName = string.Format("[{0}{1}]", AttributeCalculator.ParameterPrefix, assetType.DBTableName);

            var calculatable = _attributeRepository.FindPublished(c => 
                !string.IsNullOrEmpty(c.CalculationFormula)
                && (c.RelatedAssetTypeID == type.ID || c.CalculationFormula.Contains(parameterName)));

            var calculatedTypesUids = calculatable.Select(c => c.DynEntityConfigUid).Distinct().ToList();

            var dependenciesList = dependencies ?? new List<Asset>();
            dependenciesList.Add(asset);

            var relatedUids = GetRelatedUids(asset);

            var assetRevision = int.Parse(asset["Revision"].Value);
            if (relatedUids.Count == 0 && assetRevision > 1)
            {
                var previousRevision = _assetsService.GetAssetByParameters(assetType, new Dictionary<string, string>
                {
                    {AttributeNames.DynEntityId, asset.ID.ToString()},
                    {AttributeNames.Revision, (assetRevision - 1).ToString()}
                });
                relatedUids = GetRelatedUids(previousRevision);
            }

            foreach (var typeUid in calculatedTypesUids)
            {
                var dependentType = _typeRepository.GetByUid(typeUid);

                var dependentAssets = _assetsService.GetAssetsByParameters(dependentType, new Dictionary<string, string>
                {
                    {AttributeNames.ActiveVersion, bool.TrueString}
                }).Where(a => relatedUids.Contains(a.UID)).ToList();

                foreach (var dependentAsset in dependentAssets)
                {                    
                    saveStack.Push(dependentAsset);

                    if (dependenciesList.All(added => added.ID != dependentAsset.ID))
                        GetDependentAssets(dependentAsset, saveStack, dependenciesList);

                    dependenciesList.Add(dependentAsset);
                }
            }
        }

        #endregion

        private static void AddVertex(AdjacencyGraph<Asset, TaggedEdge<Asset, string>> graph, Asset asset)
        {
            if (!graph.Vertices.Contains(asset))
                graph.AddVertex(asset);
        }

        private static void AddDependency(AdjacencyGraph<Asset, TaggedEdge<Asset, string>> graph, Asset source, Asset target)
        {
            if (!graph.Vertices.Contains(target))
                throw new ArgumentException("Target asset is not in the graph", "target");

            if (!graph.Vertices.Contains(source))
                graph.AddVertex(source);
            graph.AddEdge(new TaggedEdge<Asset, string>(source, target, ""));
        }

        public List<Asset> GetDependentAssets(Asset asset,
            AdjacencyGraph<Asset, TaggedEdge<Asset, string>> dependencyGraph = null, Stack<Asset> assetsStack = null)
        {
            var stack = assetsStack ?? new Stack<Asset>();
            var graph = dependencyGraph ?? new AdjacencyGraph<Asset, TaggedEdge<Asset, string>>();
            AddVertex(graph, asset);

            var assetTypeId = asset.GetConfiguration().ID;

            // find types with formulas which contain references to related assets ([RelatedAsset@FieldName])
            var relatedTypes = _attributeRepository.FindPublished(a => a.RelatedAssetTypeID == assetTypeId)
                .Select(a => new
                {
                    attribute = a,
                    type = _typeRepository.GetByUid(a.DynEntityConfigUid) // ToDo this issues a huge number of heavy AssetType sql requests
                });

            foreach (var relation in relatedTypes)
            {
                var attributeName = relation.attribute.DBTableFieldname;
                var relationText = string.Format("[{0}@", attributeName);

                var hasRelation =
                    relation.type.Attributes.Any(
                        a => !string.IsNullOrEmpty(a.FormulaText) && a.FormulaText.Contains(relationText));

                if (hasRelation)
                {
                    var dependentAssets =
                        _assetsService.GetAssetsByParameters(relation.type, new Dictionary<string, string>()
                        {
                            {AttributeNames.ActiveVersion, true.ToString()},
                            {attributeName, asset.ID.ToString(CultureInfo.InvariantCulture)}
                        }).ToList();

                    dependentAssets.ForEach(dependetAsset =>
                    {
                        AddDependency(graph, dependetAsset, asset);
                        // recursive call                        
                        GetDependentAssets(dependetAsset, graph, stack);
                    });
                }
            }
            
            GetDependentAssets(asset, stack);
            //            var parameterName = string.Format("[{0}{1}]", ParameterPrefix, asset.GetConfiguration().DBTableName);
            //            // find types with formulas which contain references to asset table ([$ADynEntityTableName])
            //            relatedTypes = _unitOfWork.DynEntityAttribConfigRepository
            //                .Get(a => a.ActiveVersion
            //                          && !string.IsNullOrEmpty(a.CalculationFormula)
            //                          && a.CalculationFormula.Contains(parameterName))
            //                .Select(a => new
            //                {
            //                    attribute = a,
            //                    type = _typeRepository.GetByUid(a.DynEntityConfigUid)
            //                });
            //
            //            foreach (var relation in relatedTypes)
            //            {
            //                var dependentAssets =
            //                    _assetsService.GetAssetsByParameters(relation.type, new Dictionary<string, string>()
            //                    {
            //                        {AttributeNames.ActiveVersion, true.ToString()}
            //                    }).ToList();
            //
            //                dependentAssets.ForEach(dependetAsset =>
            //                {
            //                    AddDependency(graph, dependetAsset, asset);
            //                    // recursive call                        
            //                    GetDependentAssets(dependetAsset, graph);
            //                });
            //            }

            var result = graph.TopologicalSort().Reverse().ToList();
            result.Remove(asset);
            result.AddRange(stack.ToList());
            return result.ToList();
        }
    }
}
