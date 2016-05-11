using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Exceptions;
using AppFramework.Core.Helpers;
using AppFramework.DataProxy;
using Common.Logging;
using NCalc;

namespace AppFramework.Core.Calculation
{
    public class AttributeCalculator : IAttributeCalculator
    {
        public static string ParameterPrefix = "$";
        public static string GlobalVariablePrefix = "#";
        public static string RelationOperator = "@";

        private readonly Dictionary<string, Expression> _expressionsCache = new Dictionary<string, Expression>();

        private readonly IAssetsService _assetsService;
        private readonly CalculationFunctions _calculationFunctions;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        //todo: use AuthenticationService
        private readonly long _currentUserId;

        private static readonly object ExclusionsLock = new object();
        private readonly List<long> _dependenciesCalculationExclusions = new List<long>();
        private readonly DependenciesFinder _dependenciesFinder;

        public Asset CallingAsset { get; set; }

        /// <summary>
        /// Returns errors.
        /// Must be null on success.
        /// </summary>
        public string Error { get; private set; }

        // ToDo fix circular dependency AssetsService - AttributeCalculator
        public AttributeCalculator(
            IUnitOfWork unitOfWork,
            IAssetsService assetsService,
            IAssetTypeRepository assetTypeRepository,
            IAttributeRepository attributeRepository)
        {
            _currentUserId = 1;

            _assetsService = assetsService;
            _dependenciesFinder = new DependenciesFinder(assetsService, assetTypeRepository, attributeRepository);
            _calculationFunctions = new CalculationFunctions(unitOfWork, assetsService, this);
        }

        /// <summary>
        /// Calculates attribute value by formula (CalculationFormula).
        /// Works only for attributes within same asset.
        /// </summary>
        /// <param name="asset">Asset instance</param>
        /// <param name="attrs"></param>
        /// <param name="expressionString">Custom formula text</param>
        /// <param name="callingAsset"></param>
        /// <returns>Calculated value</returns>
        public object GetValue(Asset asset, ScreenAttrs attrs, string expressionString, long callingAsset = -1)
        {
            if (asset == null)
                throw new ArgumentNullException("asset");

            if (string.IsNullOrEmpty(expressionString))
                throw new ArgumentException("expressionString");

            Error = null;

            // try to get screen formula from cache
            var expression = GetExpression(expressionString);

            // clean old parameters for cached expression
            expression.Parameters.Clear();

            expression.EvaluateParameter += (name, args) =>
            {
                if (name.StartsWith(RelationOperator))
                {
                    name = name.TrimStart(RelationOperator.ToCharArray());
                    var paramValue = GetParameterValue(asset, attrs, name, callingAsset);
                    args.Result = paramValue;
                }
                else if (name.StartsWith(ParameterPrefix))
                {
                    args.Result = name.TrimStart(ParameterPrefix.ToCharArray());
                }
                else if (name.StartsWith(GlobalVariablePrefix))
                {
                    args.Result = GetGloblaValue(name);
                }
                else
                {
                    var paramValue = GetParameterValue(asset, attrs, name);
                    args.Result = paramValue;
                }
            };

            expression.EvaluateFunction += (name, args) =>
            {
                var function = _calculationFunctions.EvaluateFunction(name, asset, args);
                if (function != null)
                    args.Result = function;
            };

            object result;

            try
            {
                result = expression.Evaluate();
            }
            catch (Exception e)
            {
                SetError(e);
                throw new InvalidFormulaException(expressionString, e);
            }

            return result;
        }

        private object GetGloblaValue(string globalName)
        {
            if (globalName == "#CurrentUserId")
                return _currentUserId;

            return null;
        }

        private Expression GetExpression(string expressionText)
        {
            Expression expression;
            _expressionsCache.TryGetValue(expressionText, out expression);

            if (expression == null)
            {
                expression = new Expression(expressionText);
                _expressionsCache.Add(expressionText, expression);
            }

            if (expression.HasErrors())
            {
                throw new InvalidFormulaException(expressionText, Error);
            }

            return expression;
        }

        public void CalculateDependencies(Asset asset)
        {
            lock (ExclusionsLock)
            {
                if (_dependenciesCalculationExclusions.Contains(asset.ID))
                    return;
            }

            var assets = _dependenciesFinder.GetDependentAssets(asset);
            assets.ForEach(a =>
            {
                lock (ExclusionsLock)
                {
                    _dependenciesCalculationExclusions.Add(a.ID);
                }
                _logger.DebugFormat("Saving dependency asset - asset: '{0}'", a.Name);
                _assetsService.InsertAsset(a);
            });
        }

        private void SetError(Exception e)
        {
            _logger.ErrorFormat("Formula error", e);
            Error = e.Message;
        }

        private object GetParameterValue(Asset asset, ScreenAttrs attrs, string parameterName, long callingAsset = -1)
        {
            if (asset == null || attrs == null || attrs.Count == 0)
                return string.Empty;

            object result = string.Empty;

            if (parameterName.Contains(RelationOperator))
            {
                var dotIndex = parameterName.IndexOf(RelationOperator, StringComparison.Ordinal);
                var relatedAssetName = parameterName.Substring(0, dotIndex);
                var valueFieldName = parameterName.Substring(dotIndex + 1, parameterName.Length - dotIndex - 1);

                var related = attrs[relatedAssetName].RelatedAsset;

                if (related != null)
                {
                    // handle self dependency to get edited result on the page
                    if (related.Configuration.ID == asset.Configuration.ID && related.ID == asset.ID)
                    {
                        result = GetParameterValue(asset, attrs, valueFieldName, callingAsset);
                    }
                    else
                    {
                        result = GetParameterValue(related, new ScreenAttrs(related.Attributes), valueFieldName, callingAsset);
                    }
                }
                else
                    result = TypesHelper.GetTypedValue(attrs[relatedAssetName].Configuration.DataTypeEnum, result);

                return result;
            }

            result = callingAsset != -1
                ? CallingAsset[parameterName].Value
                : TypesHelper.GetTypedValue(attrs, parameterName);

            return result;
        }

        public void CalculateAssetScreens(AssetWrapperForScreenView assetWrapper, long? screenId = null)
        {
            var asset = assetWrapper.Asset;

            _logger.DebugFormat("Calculating asset \"{0}\" (uid #{1}, DynEntityConfigUid #{2})",
                asset.Name, asset.UID, asset.DynEntityConfigUid);

            if (screenId.HasValue)
            {
                CalculateAssetScreens(assetWrapper, screenId.Value);
            }
            else
            {
                var screenIds = asset.Configuration.Panels
                    .Where(p => p.ScreenId.HasValue) // no idea how it is possible, but there are panels with no screen
                    .Select(p => p.ScreenId.GetValueOrDefault()).Distinct();
                foreach (var s in screenIds)
                {
                    CalculateAssetScreens(assetWrapper, s);
                }
            }
        }

        private void CalculateAssetScreens(AssetWrapperForScreenView assetWrapper, long screenId)
        {
            var asset = assetWrapper.Asset;

            _logger.DebugFormat("Calculating asset \"{0}\" (uid #{1}, DynEntityConfigUid #{2}, Screen Id {3})",
                asset.Name, asset.UID, asset.DynEntityConfigUid, screenId);

            var attributesFormulas =
                from panel in asset
                    .Configuration
                    .Panels
                    .Where(p => p.ScreenId == screenId)
                // for all screens or just for one particular
                let tuples = panel.Base
                    .AttributePanelAttribute
                    .Where(apa => apa.ScreenFormula != null) // with screen formulas
                    .Select(apa => new Tuple<long, string>(apa.DynEntityAttribConfigUId, apa.ScreenFormula))
                from attribute in panel.AssignedAttributes
                from tuple in tuples
                where attribute.UID == tuple.Item1
                select Tuple.Create(attribute, tuple.Item2);

            CalculateAttributes(assetWrapper.Asset, assetWrapper.ScreenAttributes(screenId), attributesFormulas.ToList());
        }

        public Asset PostCalculateAsset(Asset asset, bool calculateDependencies = true)
        {
            _logger.DebugFormat("Calculating asset \"{0}\" (uid #{1}, DynEntityConfigUid #{2})",
                asset.Name, asset.UID, asset.DynEntityConfigUid);

            var attributesFormulas = asset.GetConfiguration()
                .AllAttributes
                .Where(a => a.IsCalculated && !string.IsNullOrEmpty(a.FormulaText))
                // for all attributes with table formulas
                .Select(a => Tuple.Create(a, a.FormulaText));

            CalculateAttributes(asset, new ScreenAttrs(asset.Attributes), attributesFormulas.ToList());

            if (calculateDependencies)
                CalculateDependencies(asset);

            return asset;
        }

        /// <summary>
        /// Calculate attribute values by formulas
        /// </summary>
        /// <param name="asset">Source asset</param>
        /// <param name="attributes">Calculated data holder</param>
        /// <param name="attributesToCalc">turn on your imagination</param>
        private void CalculateAttributes(Asset asset, ScreenAttrs attributes, List<Tuple<AssetTypeAttribute, string>> attributesToCalc)
        {
            var attributeReferences = attributesToCalc
                .Select(a => a.Item1.DBTableFieldName)
                .ToList();

            /* cross reference check
             * if we iterated over all attributes and didn't remove (calculated) at least one,
             * then attributes have unresolvable set of formulas
             */
            var n = 0;
            while (attributesToCalc.Any())
            {
                if (n >= attributesToCalc.Count)
                {
                    throw new Exception(string.Format(
                        "Attributes of Asset (id {0}, type id {1}) have unresolvable cross references in formulas",
                        asset.ID, asset.Configuration.ID));
                }

                var current = attributesToCalc.First();
                var attrRef = attributeReferences
                    .Where(r => current.Item2.Contains(string.Format("[{0}]", r)) && r != current.Item1.DBTableFieldName)
                    .ToArray();

                if (attrRef.Any())
                {
                    var isNotReady = attributesToCalc.Any(a => attrRef.Contains(a.Item1.DBTableFieldName));

                    if (isNotReady)
                    {
                        attributesToCalc.Remove(current);
                        attributesToCalc.Insert(attributesToCalc.Count, current);
                        ++n;
                        continue;
                    }
                }

                var value = GetValue(asset, attributes, current.Item2);

                if (Error == null)
                {
                    if (current.Item1.DataTypeEnum == Enumerators.DataType.Asset)
                    {
                        attributes[current.Item1.DBTableFieldName].ValueAsId = TypesHelper.GetTypedValue<long>(value);
                    }
                    else
                    {
                        _logger.DebugFormat("Assigning calculated value \"{0}\"", value);
                        attributes[current.Item1.DBTableFieldName].Value = value == null
                            ? string.Empty
                            : value.ToString();
                    }
                }
                attributesToCalc.Remove(current);
                n = 0;
            }
        }
    }
}