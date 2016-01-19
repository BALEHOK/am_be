using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AssetManagerAdmin.FormulaBuilder.Expressions;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;
using AssetManagerAdmin.Model;
using AssetManager.Infrastructure.Models.TypeModels;
using GalaSoft.MvvmLight.Command;
using Common.Logging;
using AssetManagerAdmin.Infrastructure;

namespace AssetManagerAdmin.ViewModels
{
    public sealed class FormulaBuilderViewModel : ToolkitViewModelBase
    {
        private FormulaBuilderContextType _currentFormulaContext
            = FormulaBuilderContextType.DbFormulas;
        private ExpressionParser _expressionParser;
        private readonly IDataService _dataService;
        private readonly IAssetsDataProvider _dataProvider;
        private RelayCommand _refreshAssetTypeListCommand;
        private readonly ILog _logger;

        public IAssetsDataProvider DataProvider
        {
            get { return _dataProvider; }
        }

        public const string ExpressionParserPropertyName = "ExpressionParser";
        public ExpressionParser ExpressionParser
        {
            get { return _expressionParser; }

            set
            {
                _expressionParser = value;
                RaisePropertyChanged(ExpressionParserPropertyName);
            }
        }

        public const string FormulaTextPropertyName = "FormulaText";
        private string _formulaText;

        public string FormulaText
        {
            get { return _formulaText; }

            set
            {
                _formulaText = value;
                RaisePropertyChanged(FormulaTextPropertyName);
            }
        }

        public const string SavedFormulaPropertyName = "SavedFormula";
        private string _savedFormula;

        public string SavedFormula
        {
            get { return _savedFormula; }

            set
            {
                _savedFormula = string.Format("Saved formula: {0}", value);
                RaisePropertyChanged(SavedFormulaPropertyName);
            }
        }

        public const string ExpressionPropertyName = "Expression";
        private ExpressionEntry _expression;

        public ExpressionEntry Expression
        {
            get { return _expression; }

            set
            {
                _expression = value;
                RaisePropertyChanged(ExpressionPropertyName);
            }
        }

        public const string GrammarPropertyName = "Grammar";
        private ExpressionsGrammar _grammar;

        public ExpressionsGrammar Grammar
        {
            get { return _grammar; }

            set
            {
                _grammar = value;
                RaisePropertyChanged(GrammarPropertyName);
            }
        }

        public const string BuilderPropertyName = "Builder";
        private ExpressionBuilder _builder;

        public ExpressionBuilder Builder
        {
            get { return _builder; }

            set
            {
                _builder = value;
                RaisePropertyChanged(BuilderPropertyName);
            }
        }  

        public const string AssetTypesListPropertyName = "AssetTypesList";
        private List<AssetTypeModel> _assetTypesList;

        public List<AssetTypeModel> AssetTypesList
        {
            get { return _assetTypesList; }

            set
            {
                _assetTypesList = value;
                RaisePropertyChanged(AssetTypesListPropertyName);
            }
        }

        public const string AttributeTypePropertyName = "AttributeType";
        private AttributeTypeModel _attributeType;

        public AttributeTypeModel AttributeType
        {
            get { return _attributeType; }

            set
            {
                _attributeType = value;
                RaisePropertyChanged(AttributeTypePropertyName);

                LoadAttribute(AttributeType);
            }
        }

        public const string ScreenAttributePropertyName = "ScreenAttribute";
        private ScreenPanelAttributeModel _screenAttribute;

        public ScreenPanelAttributeModel ScreenAttribute
        {
            get { return _screenAttribute; }

            set
            {
                _screenAttribute = value;
                RaisePropertyChanged(ScreenAttributePropertyName);

                if (ScreenAttribute != null)
                    LoadAttribute(ScreenAttribute.AttributeModel);
            }
        }

        public const string IsScreenAttributesSelectorVisiblePropertyName = "IsScreenAttributesSelectorVisible";
        private bool _isScreenAttributesSelectorVisible;

        public bool IsScreenAttributesSelectorVisible
        {
            get { return _isScreenAttributesSelectorVisible; }

            set
            {
                _isScreenAttributesSelectorVisible = value;
                RaisePropertyChanged(IsScreenAttributesSelectorVisiblePropertyName);
            }
        }

        public const string IsAttributesSelectorVisiblePropertyName = "IsAttributesSelectorVisible";
        private bool _isAttributesSelectorVisible;

        public bool IsAttributesSelectorVisible
        {
            get { return _isAttributesSelectorVisible; }

            set
            {
                _isAttributesSelectorVisible = value;
                RaisePropertyChanged(IsAttributesSelectorVisiblePropertyName);
            }
        }

        #region Commands

        private RelayCommand _copyToClipboardCommand;

        public RelayCommand CopyToClipboardCommand
        {
            get
            {
                return _copyToClipboardCommand ??
                       (_copyToClipboardCommand =
                           new RelayCommand(() => Clipboard.SetText(Builder.Expression.ToString()), () => true));
            }
        }

        private RelayCommand _pasteFormulaCommand;

        public RelayCommand PasteFormulaCommand
        {
            get
            {
                return _pasteFormulaCommand ??
                       (_pasteFormulaCommand = new RelayCommand(ExecutePasteFormulaCommand, () => true));
            }
        }

        private void ExecutePasteFormulaCommand()
        {
        }

        private RelayCommand _saveFormulaCommand;

        public RelayCommand SaveFormulaCommand
        {
            get
            {
                return _saveFormulaCommand ??
                       (_saveFormulaCommand = new RelayCommand(ExecuteSaveFormulaCommand, () => AttributeType != null));
            }
        }
        
        public RelayCommand RefreshAssetTypeListCommand
        {
            get
            {
                return _refreshAssetTypeListCommand ??
                       (_refreshAssetTypeListCommand = new RelayCommand(ExecuteRefreshAssetTypeListCommand));
            }
        }

        private void ExecuteRefreshAssetTypeListCommand()
        {
            MessengerInstance.Send((object)null, AppActions.ClearTypesInfoCache);
            LoadTypesInfo(_currentFormulaContext);
        }

        private void ExecuteSaveFormulaCommand()
        {
            var formulaText = Builder.Expression != null ? Builder.Expression.ToString() : string.Empty;

            switch (_currentFormulaContext)
            {
                case FormulaBuilderContextType.DbFormulas:
                    Api.SaveFormula(_dataProvider.CurrentAssetType, AttributeType.DbName, formulaText)
                        .ContinueWith(a =>
                        {
                            AttributeType.CalculationFormula = FormulaText;
                            MessageBox.Show(a.Result);
                        });
                    break;

                case FormulaBuilderContextType.ScreenFormulas:
                    ScreenAttribute.ScreenFormula = FormulaText;
                    Api.SaveScreenFormula(ScreenAttribute).ContinueWith(a =>
                    {
                        ScreenAttribute.AttributeModel.ScreenFormula = formulaText;
                        MessageBox.Show(a.Result);
                    });
                    ScreenAttribute.AttributeModel.IsHighlighted = !string.IsNullOrEmpty(FormulaText);
                    break;

                case FormulaBuilderContextType.Validation:
                    Api.SaveValidation(_dataProvider.CurrentAssetType, AttributeType.DbName,
                        formulaText)
                        .ContinueWith(a =>
                        {
                            AttributeType.ValidationExpression = formulaText;
                            MessageBox.Show(a.Result);
                        });
                    break;
            }

            AttributeType.IsHighlighted = !string.IsNullOrEmpty(FormulaText);
        }

        #endregion

        private void LoadAttribute(AttributeTypeModel attributeInfo)
        {
            if (attributeInfo == null)
                return;

            _dataProvider.CurrentAttributeType = attributeInfo;

            string formulaText;

            switch (_currentFormulaContext)
            {
                case FormulaBuilderContextType.DbFormulas:
                    formulaText = attributeInfo.CalculationFormula;
                    break;
                case FormulaBuilderContextType.ScreenFormulas:
                    formulaText = attributeInfo.ScreenFormula;
                    break;
                case FormulaBuilderContextType.Validation:
                    formulaText = attributeInfo.ValidationExpression;
                    break;
                default:
                    return;
            }

//            if (Builder != null)
//                Builder.Reset(true);

            if (formulaText == null)
                return;

            try
            {
                ExpressionParser.Parse(formulaText.Trim());
            }
            catch (Exception ex)
            {
                var message = string.Format("This attribute has invalid formula\r\n{0}",
                    formulaText.Trim());
                _logger.Error(message, ex);
                MessageBox.Show(message);
            }
        }

        private ExpressionsGrammar GetDbFormulasGrammar()
        {
            var grammar = new ExpressionsGrammar();

            grammar.Group("Values").AddCustomType(new AssetFieldValueEntry());
            grammar.AddCustomType(new ValueEntry());
            grammar.AddCustomType(new RelatedFieldValueEntry());
            grammar.AddCustomType(new NamedVariableEntry());

            grammar.Group("Math Operators").AddBinaryOperator("+");
            grammar.AddBinaryOperator("-");
            grammar.AddBinaryOperator("*");
            grammar.AddBinaryOperator("/");

            grammar.Group("Math Functions").AddFunction("Abs", "ABS")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Truncate", "TRUNCATE")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Round", "ROUND")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("RoundTo", "ROUNDTO")
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Remainder", "REMAINDER")
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("ToMoney", "TOMONEY")
                .AddParameter(new ExpressionEntry());

            grammar.Group("Logical").AddFunction("", "BR")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("if", "if")
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("not", "NOT")
                .AddParameter(new ExpressionEntry());
            grammar.AddBinaryOperator(">");
            grammar.AddBinaryOperator("<");
            grammar.AddBinaryOperator("=");
            grammar.AddBinaryOperator("<>");
            grammar.AddBinaryOperator(">=");
            grammar.AddBinaryOperator("<=");
            grammar.AddBinaryOperator("and");
            grammar.AddBinaryOperator("or");

            grammar.Group("Statistical Functions").AddFunction("Sum", "SUM")
                .AddParameter(new ExpressionEntry());

            grammar.AddFunction("Count", "COUNT")
                .AddParameter(new AssetTypeNameEntry())
                .AddParameter(new AssetFieldNameEntry())
                .AddParameter(new ExpressionEntry());

            grammar.AddFunction("Average", "AVERAGE")
                .AddParameter(new AssetTypeNameEntry())
                .AddParameter(new AssetFieldNameEntry())
                .AddParameter(new AssetFieldNameEntry())
                .AddParameter(new ExpressionEntry());

            grammar.Group("Query Functions").AddFunction("Select", "SELECT")
                .AddParameter(new AssetTypeNameEntry())
                .AddParameter(new AssetFieldNameEntry())
                .AddParameter(new AssetFieldValueEntry())
                .AddParameter(new ExpressionEntry
                {
                    Open = "'$",
                    Close = "'",
                    Overrides = new List<ExpressionEntry>
                    {
                        new AssetFieldNameEntry {Open = "["},
                        new AssetFieldValueEntry {Open = "[@", Close = "]"}
                    }
                });

            grammar.AddFunction("SqlFind", "SQLFIND")
                .AddParameter(new AssetTypeNameEntry())
                .AddParameter(new AssetFieldNameEntry())
                .AddParameter(new ExpressionEntry());

            grammar.AddFunction("SqlIndex", "SQLINDEX")
                .AddParameter(new AssetTypeNameEntry())
                .AddParameter(new AssetFieldNameEntry())
                .AddParameter(new ExpressionEntry());

            grammar.AddCustomType(new AssetTypeNameEntry());
            grammar.AddCustomType(new AssetFieldNameEntry());

            grammar.Group("Date and Time Functions").AddFunction("Now", "NOW");
            grammar.AddFunction("NowGMT", "NOWGMT");
            grammar.AddFunction("Date", "DATE")
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Day", "DAY")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Month", "MONTH")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Year", "YEAR")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("DayOfWeek", "DAYOFWEEK")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("DayOfYear", "DAYOFYEAR")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Hour", "HOUR")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Minute", "MINUTE")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Second", "SECOND")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("WorkingDays", "WORKINGDAYS")
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry());

            grammar.Group("Text Functions").AddFunction("Length", "LENGTH")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Left", "LEFT")
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Right", "RIGHT")
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Mid", "MID")
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Trim", "TRIM")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Value", "VALUE")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Search", "SEARCH")
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Lower", "LOWER")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("Upper", "UPPER")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("ReplaceAll", "REPLACEALL")
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("ProperCase", "PROPERCASE")
                .AddParameter(new ExpressionEntry());

            return grammar;
        }

        private ExpressionsGrammar GetValidationFormulasGrammar()
        {
            var grammar = new ExpressionsGrammar();

            grammar.Group("Values");
            grammar.AddCustomType(new ValidationFieldValueEntry());
            grammar.AddCustomType(new ValueEntry());

            grammar.Group("Operations").AddFunction("", "BR")
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("if", "if")
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry())
                .AddParameter(new ExpressionEntry());
            grammar.AddFunction("not", "NOT")
                .AddParameter(new ExpressionEntry());
            grammar.AddBinaryOperator(">");
            grammar.AddBinaryOperator("<");
            grammar.AddBinaryOperator("=");
            grammar.AddBinaryOperator("<>");
            grammar.AddBinaryOperator(">=");
            grammar.AddBinaryOperator("<=");
            grammar.AddBinaryOperator("and");
            grammar.AddBinaryOperator("or");

            grammar.Group("Functions").AddFunction("RegEx", "REGEX")
                .AddParameter(new ValueEntry())
                .AddParameter(new ValidationFieldValueEntry());
            grammar.AddFunction("IsDigit", "ISDIGIT")
                .AddParameter(new ValidationFieldValueEntry());
            grammar.AddFunction("IsIP", "ISIP")
                .AddParameter(new ValidationFieldValueEntry());
            grammar.AddFunction("IsBarcode", "ISBARCODE")
                .AddParameter(new ValidationFieldValueEntry());
            grammar.AddFunction("IsEmail", "ISEMAIL")
                .AddParameter(new ValidationFieldValueEntry());
            grammar.AddFunction("IsUrl", "ISURL")
                .AddParameter(new ValidationFieldValueEntry());
            grammar.AddFunction("Unique", "UNIQUE")
                .AddParameter(new ValidationFieldValueEntry());
            grammar.AddFunction("SystemUnique", "SYSTEMUNIQUE")
                .AddParameter(new ValidationFieldValueEntry());

            return grammar;
        }

        private ExpressionsGrammar GetGrammar(FormulaBuilderContextType contextType)
        {
            switch (contextType)
            {
                case FormulaBuilderContextType.DbFormulas:
                case FormulaBuilderContextType.ScreenFormulas:
                    return GetDbFormulasGrammar();

                case FormulaBuilderContextType.Validation:
                    return GetValidationFormulasGrammar();

                default:
                    return null;
            }
        }

        private void LoadTypesInfo(FormulaBuilderContextType context)
        {
            _dataService.GetTypesInfo(Context.CurrentUser, Context.CurrentServer.ApiUrl)
                .ContinueWith(result =>
            {
                // do not modify original collection
                _dataProvider.AssetTypes = result.Result.ActiveTypes.ToList();

                _dataProvider.AssetTypes.ForEach(t =>
                {
                    t.IsHighlighted =
                        context == FormulaBuilderContextType.DbFormulas && t.HasCalculatedAttributes ||
                        context == FormulaBuilderContextType.ScreenFormulas && t.HasScreenFormulas ||
                        context == FormulaBuilderContextType.Validation && t.HasValidationExpressions;

                    t.Attributes.ForEach(a => a.IsHighlighted =
                        context == FormulaBuilderContextType.DbFormulas && a.HasDatabaseFormula ||
                        context == FormulaBuilderContextType.ScreenFormulas && a.HasScreenFormula ||
                        context == FormulaBuilderContextType.Validation && a.HasValidationExpression);
                });

                IsAttributesSelectorVisible = context == FormulaBuilderContextType.DbFormulas ||
                                              context == FormulaBuilderContextType.Validation;
                IsScreenAttributesSelectorVisible = context == FormulaBuilderContextType.ScreenFormulas;

                AssetTypesList = _dataProvider.AssetTypes;


                Grammar = GetGrammar(context);
                Builder = new ExpressionBuilder(_dataProvider, Grammar);
                Builder.OnExpressionChanged += (sender, args) =>
                {
                    FormulaText = Builder.Expression != null ? Builder.Expression.ToString() : string.Empty;
                };
                ExpressionParser = new ExpressionParser(_dataProvider, Builder, Grammar);
            }, 
            TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public FormulaBuilderViewModel(
            IDataService dataService,
            IAssetsDataProvider dataProvider,
            IAppContext context,
            ILog logger) : base(context)
        {
            _dataService = dataService;
            _dataProvider = dataProvider;
            _logger = logger;

            OnNavigated += (parameter) =>
            {
                if (parameter is FormulaBuilderContextType)
                    _currentFormulaContext = (FormulaBuilderContextType)parameter;
                LoadTypesInfo(_currentFormulaContext);
            };
        }
    }
}