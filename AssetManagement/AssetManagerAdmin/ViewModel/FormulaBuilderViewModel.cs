using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AssetManagerAdmin.FormulaBuilder.Expressions;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;
using AssetManagerAdmin.Model;
using AssetManagerAdmin.WebApi;
using AssetManager.Infrastructure.Models.TypeModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace AssetManagerAdmin.ViewModel
{
    public enum FormulaBuilderContextType
    {
        DbFormulas = 0,
        ScreenFormulas,
        Validation,
        DataTypesValidation
    }

    public struct FormulaBuilderContext
    {
        public FormulaBuilderContextType Type { get; set; }
        public string Name { get; set; }
    }

    public sealed class FormulaBuilderViewModel : ViewModelBase, ICommonViewModel
    {
        private ExpressionParser _expressionParser;
        private readonly IDataService _dataService;
        private readonly IAssetsApiManager _assetsApiManager;
        private string _server;

        public AssetsDataProvider DataProvider { get; private set; }

        public bool IsActive { get; set; }

        #region Properties

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

        public const string ContextsPropertyName = "Contexts";
        private List<FormulaBuilderContext> _contexts;

        public List<FormulaBuilderContext> Contexts
        {
            get { return _contexts; }

            set
            {
                _contexts = value;
                RaisePropertyChanged(ContextsPropertyName);
            }
        }

        public const string CurrentContextPropertyName = "CurrentContext";
        private FormulaBuilderContext _currentContext;

        public FormulaBuilderContext CurrentContext
        {
            get { return _currentContext; }

            set
            {
                _currentContext = value;
                RaisePropertyChanged(CurrentContextPropertyName);
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

        #endregion

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

        private RelayCommand _refreshAssetTypeListCommand;

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
            LoadTypesInfo(CurrentContext);
        }

        private void ExecuteSaveFormulaCommand()
        {
            var api = _assetsApiManager.GetAssetApi(_server, _dataService.CurrentUser);

            var formulaText = Builder.Expression != null ? Builder.Expression.ToString() : string.Empty;

            switch (CurrentContext.Type)
            {
                case FormulaBuilderContextType.DbFormulas:
                    api.SaveFormula(DataProvider.CurrentAssetType, AttributeType.DbName, formulaText)
                        .ContinueWith(a =>
                        {
                            AttributeType.CalculationFormula = FormulaText;
                            MessageBox.Show(a.Result);
                        });
                    break;

                case FormulaBuilderContextType.ScreenFormulas:
                    ScreenAttribute.ScreenFormula = FormulaText;
                    api.SaveScreenFormula(ScreenAttribute).ContinueWith(a =>
                    {
                        ScreenAttribute.AttributeModel.ScreenFormula = formulaText;
                        MessageBox.Show(a.Result);
                    });
                    ScreenAttribute.AttributeModel.IsHighlighted = !string.IsNullOrEmpty(FormulaText);
                    break;

                case FormulaBuilderContextType.Validation:
                    api.SaveValidation(DataProvider.CurrentAssetType, AttributeType.DbName,
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

            DataProvider.CurrentAttributeType = attributeInfo;

            string formulaText;

            switch (CurrentContext.Type)
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
            catch (Exception)
            {
                MessageBox.Show(string.Format("This attribute has invalid formula\r\n{0}", formulaText.Trim()));
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

        public async Task WaitForServerName()
        {
            while (string.IsNullOrEmpty(_server))
            {
                await Task.Delay(500).ConfigureAwait(false);
            }
        }

        private async void ChangeContext(FormulaBuilderContext context)
        {
            if (Builder != null)
                Builder.Reset();

            await WaitForServerName();

            LoadTypesInfo(context);
        }

        private void LoadTypesInfo(FormulaBuilderContext context)
        {
            _dataService.GetTypesInfo(_server, (model, exception) =>
            {
                // do not modify original collection
                DataProvider.AssetTypes = model.ActiveTypes.ToList();

                DataProvider.AssetTypes.ForEach(t =>
                {
                    t.IsHighlighted =
                        context.Type == FormulaBuilderContextType.DbFormulas && t.HasCalculatedAttributes ||
                        context.Type == FormulaBuilderContextType.ScreenFormulas && t.HasScreenFormulas ||
                        context.Type == FormulaBuilderContextType.Validation && t.HasValidationExpressions;

                    t.Attributes.ForEach(a => a.IsHighlighted =
                        context.Type == FormulaBuilderContextType.DbFormulas && a.HasDatabaseFormula ||
                        context.Type == FormulaBuilderContextType.ScreenFormulas && a.HasScreenFormula ||
                        context.Type == FormulaBuilderContextType.Validation && a.HasValidationExpression);
                });

                IsAttributesSelectorVisible = context.Type == FormulaBuilderContextType.DbFormulas ||
                                              context.Type == FormulaBuilderContextType.Validation;
                IsScreenAttributesSelectorVisible = context.Type == FormulaBuilderContextType.ScreenFormulas;

                AssetTypesList = DataProvider.AssetTypes;


                Grammar = GetGrammar(CurrentContext.Type);
                Builder = new ExpressionBuilder(DataProvider, Grammar);
                Builder.OnExpressionChanged += (sender, args) =>
                {
                    FormulaText = Builder.Expression != null ? Builder.Expression.ToString() : string.Empty;
                };
                ExpressionParser = new ExpressionParser(DataProvider, Builder, Grammar);
            });
        }

        protected override void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);

            if (propertyName == CurrentContextPropertyName)
            {
                ChangeContext(CurrentContext);
            }
        }

        public FormulaBuilderViewModel(IDataService dataService, IAssetsApiManager assetsApiManager)
        {
            _dataService = dataService;
            _assetsApiManager = assetsApiManager;

            DataProvider = new AssetsDataProvider();

            Contexts = new List<FormulaBuilderContext>
            {
                new FormulaBuilderContext {Name = "Database Formulas", Type = FormulaBuilderContextType.DbFormulas},
                new FormulaBuilderContext {Name = "Screen Formulas", Type = FormulaBuilderContextType.ScreenFormulas},
                new FormulaBuilderContext {Name = "Validation", Type = FormulaBuilderContextType.Validation},
//                new FormulaBuilderContext {Name = "Data Types Validation", Type = FormulaBuilderContextType.DataTypesValidation},
            };
            CurrentContext = Contexts.Single(c => c.Type == FormulaBuilderContextType.DbFormulas);

            MessengerInstance.Register<ServerConfig>(this, AppActions.LoginDone, server =>
            {
                _server = server.ApiUrl;
            });
        }
    }
}