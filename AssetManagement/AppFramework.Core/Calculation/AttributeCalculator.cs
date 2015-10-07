using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
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
        private readonly Dictionary<long, string> _screenFormulasCache = new Dictionary<long, string>();

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _typeRepository;
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

        public AttributeCalculator(
            IUnitOfWork unitOfWork,
            IAssetsService assetsService,
            IAssetTypeRepository assetTypeRepository)
        {
            _currentUserId = 1;

            _unitOfWork = unitOfWork;
            _typeRepository = assetTypeRepository;
            _assetsService = assetsService;
            _dependenciesFinder = new DependenciesFinder(_unitOfWork, _assetsService, _typeRepository);
            _calculationFunctions = new CalculationFunctions(_unitOfWork, _typeRepository, _assetsService, this);
        }

        /// <summary>
        /// Calculates attribute value by formula (CalculationFormula).
        /// Works only for attributes within same asset.
        /// </summary>
        /// <param name="asset">Asset instance</param>
        /// <param name="attribute">Calculating attribute type (can be null if <param name="expressionString"/> is specified)</param>
        /// <param name="expressionString">Custom formula text</param>
        /// <param name="callingAsset"></param>
        /// <returns>Calculated value</returns>
        public object GetValue(Asset asset, string expressionString, long callingAsset = -1)
        {
            if (asset == null)
                throw new ArgumentNullException("asset");

            if (string.IsNullOrEmpty(expressionString))
                throw new ArgumentException("expressionString");

            Error = null;

            // try to get screen formula from cache
            var expression = GetExpression(asset, expressionString);

            // clean old parameters for cached expression
            expression.Parameters.Clear();

            expression.EvaluateParameter += (name, args) =>
            {
                if (name.StartsWith(RelationOperator))
                {
                    name = name.TrimStart(RelationOperator.ToCharArray());
                    var paramValue = GetParameterValue(asset, name, callingAsset);
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
                    var paramValue = GetParameterValue(asset, name);
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
                SetError(e.ToString());
                throw;
            }

            return result;
        }

        private object GetGloblaValue(string globalName)
        {
            if (globalName == "#CurrentUserId")
                return _currentUserId;

            return null;
        }

        private Expression GetExpression(Asset asset, string expressionText)
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
                SetError(string.Format("Invalid formula '{0}'. Error: {1}", expressionText, expression.Error));
                throw new InvalidOperationException(Error);
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

        private void SetError(string message)
        {
            _logger.Info(message);
            Error = message;
        }

        public object GetParameterValue(Asset asset, string parameterName, long callingAsset = -1)
        {
            if (asset == null)
                return string.Empty;

            object result = string.Empty;

            if (parameterName.Contains(RelationOperator))
            {
                var dotIndex = parameterName.IndexOf(RelationOperator, StringComparison.Ordinal);
                var relatedAssetName = parameterName.Substring(0, dotIndex);
                var valueFieldName = parameterName.Substring(dotIndex + 1, parameterName.Length - dotIndex - 1);

                var related = asset[relatedAssetName].RelatedAsset;

                if (related != null)
                {
                    // handle self dependency to get edited result on the page
                    if (related.Configuration.ID == asset.Configuration.ID && related.ID == asset.ID)
                        related = asset;

                    result = GetParameterValue(related, valueFieldName, callingAsset);
                }
                else
                    result = TypesHelper.GetTypedValue(asset[relatedAssetName].Configuration.DataTypeEnum, result);

                return result;
            }

            if (callingAsset != -1)
            {
                result = CallingAsset[parameterName].Value;
            }
            else
                result = TypesHelper.GetTypedValue(asset, asset[parameterName].Configuration);

            return result;
        }

        public Asset PreCalculateAsset(Asset asset, long? screenId = null, bool overwrite = true)
        {
            _logger.DebugFormat("Calculating asset \"{0}\" (uid #{1}, DynEntityConfigUid #{2})",
                asset.Name, asset.UID, asset.DynEntityConfigUid);

            var attributesFormulas =
                from panel in asset
                    .GetConfiguration()
                    .Panels
                    .Where(p => screenId == null || p.ScreenId == screenId) // for all screens or just for one particular
                let tuples = panel.Base
                    .AttributePanelAttribute
                    .Where(apa => apa.ScreenFormula != null) // with screen formulas
                    .Select(apa => new Tuple<long, string>(apa.DynEntityAttribConfigUId, apa.ScreenFormula)) 
                from attribute in panel.AssignedAttributes
                from tuple in tuples
                where attribute.UID == tuple.Item1
                select Tuple.Create(attribute, tuple.Item2); 

            _calculateAttributes(asset, attributesFormulas.ToList());

            return asset;
        }

        public Asset PostCalculateAsset(Asset asset, bool calculateDependencies = true)
        {
            _logger.DebugFormat("Calculating asset \"{0}\" (uid #{1}, DynEntityConfigUid #{2})", 
                asset.Name, asset.UID, asset.DynEntityConfigUid);

            var attributesFormulas = asset.GetConfiguration()
                .AllAttributes
                .Where(a => a.IsCalculated && !string.IsNullOrEmpty(a.FormulaText)) // for all attributes with table formulas
                .Select(a => Tuple.Create(a, a.FormulaText));

            _calculateAttributes(asset, attributesFormulas.ToList());

            if (calculateDependencies)
                CalculateDependencies(asset);

            return asset;
        }

        private void _calculateAttributes(Asset asset, List<Tuple<AssetTypeAttribute, string>> attributesToCalc)
        {
            var attributeReferences = attributesToCalc
                .Select(a => a.Item1.DBTableFieldName)
                .ToList();

            while (attributesToCalc.Any())
            {
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
                        continue;
                    }
                }

                var value = GetValue(asset, current.Item2);

                if (Error == null)
                {
                    if (current.Item1.DataTypeEnum == Enumerators.DataType.Asset)
                    {
                        asset[current.Item1.DBTableFieldName].ValueAsId = TypesHelper.GetTypedValue<long>(value);
                    }
                    else
                    {
                        _logger.DebugFormat("Assigning calculated value \"{0}\"", value);
                        asset[current.Item1.DBTableFieldName].Value = value == null
                            ? string.Empty
                            : value.ToString();
                    }
                }
                attributesToCalc.Remove(current);
            }
        }

    }
}