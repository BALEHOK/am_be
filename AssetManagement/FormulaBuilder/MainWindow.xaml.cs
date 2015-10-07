using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Models.TypeModels;
using FormulaBuilder.Annotations;

namespace FormulaBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static readonly DependencyProperty SomeTextProperty = DependencyProperty.Register(
            "SomeText", typeof (string), typeof (MainWindow), new PropertyMetadata(default(string)));

        public string SomeText
        {
            get { return (string) GetValue(SomeTextProperty); }
            set { SetValue(SomeTextProperty, value); }
        }

        public string ValidationExpression
        {
            get { return _validationExpression; }
            set
            {
                _validationExpression = value;
                OnPropertyChanged();
            }
        }

        public string ValidationSymbolPosition
        {
            get { return _validationSymbolPosition; }
            set
            {
                _validationSymbolPosition = value;
                OnPropertyChanged();
            }
        }

        public string ValidationTest
        {
            get { return _validationTest; }
            set
            {
                _validationTest = value;
                OnPropertyChanged();
            }
        }

        public string ValidationError
        {
            get { return _validationError; }
            set
            {
                _validationError = value;
                OnPropertyChanged();
            }
        }

        public List<ValidationButton> ValidationButtons
        {
            get { return _validationButtons; }
            set
            {
                _validationButtons = value;
                OnPropertyChanged();
            }
        }

        public List<string> ServersList { get; set; }

        public string Server
        {
            get { return _server; }
            set
            {
                _server = value;
                GoAdminHome(Server);
                OnPropertyChanged();
            }
        }

        public List<AssetTypeModel> TypeInfoList
        {
            get { return _typeInfoList; }
            set
            {
                _typeInfoList = value.OrderBy(e => e.DisplayName).ToList();
                OnPropertyChanged();
            }
        }

        public AttributeTypeModel AssetAttribute
        {
            get { return _assetAttribute; }
            set
            {
                _assetAttribute = value;
                if (AssetAttribute != null)
                {
                    ValidationExpression = AssetAttribute.ValidationExpression;
                    AttributeDataType = string.Format("Data type: {0}", AssetAttribute.DataType);
                }
            }
        }

        public string AttributeDataType
        {
            get { return _attributeDataType; }
            set
            {
                _attributeDataType = value;
                OnPropertyChanged();
            }
        }

        public AssetTypeModel CurrentType
        {
            get { return _currentType; }
            set
            {
                _currentType = value;

                if (_currentType != null && _currentType.Attributes.Count > 0 &&
                    _currentType.Attributes.First().DisplayOrder != null)
                    _currentType.Attributes = _currentType.Attributes.OrderBy(e => e.DisplayOrder).ToList();

                OperationsData.CurrenntAssetType = CurrentType;
                OnPropertyChanged();
            }
        }

        public OperationsData OperationsData { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private PlaceHolder _rootPlaceHolder;
        private AssetTypeModel _currentType;
        private AssetsApi _assetsApi;
        private string _server;
        private List<AssetTypeModel> _typeInfoList;
        private string _validationExpression;
        private AttributeTypeModel _assetAttribute;
        private List<ValidationButton> _validationButtons;
        private string _validationTest;
        private string _validationError;
        private string _attributeDataType;
        private string _validationSymbolPosition;

        private static void CallUi(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            ServersList = new List<string> {"http://facilitymanager.facilityflexware.com", "http://am.local"};
            Server = ServersList.First();

            InitWebView();
            LoadTypesInfo();
        }

        private void LoadTypesInfo()
        {
            LoadinGrid.Visibility = Visibility.Visible;
            MainGrid.Visibility = Visibility.Collapsed;
            SomeText = "";

            _assetsApi = new AssetsApi(Server);
            _assetsApi.GetTypesInfo().ContinueWith(a => CallUi(() =>
            {
                var typesInfo = a.Result.ActiveTypes;

                OperationsData = new OperationsData
                {
                    TypesInfo = a.Result
                };

                // get a list of relation attributes
                var relationAttributes = typesInfo
                    .SelectMany(t => t.Attributes
                        .Where(attr => attr.RelationId != 0)).ToList();

                // set relation types info
                relationAttributes.ForEach(attr =>
                {
                    var relationType = typesInfo
                        .Single(t => t.Id == attr.RelationId);
                    attr.RelationType = relationType;
                });

                _rootPlaceHolder = new PlaceHolder(OperationsData, new TokenValue(TokenType.T("expression")));

                TypeInfoList = OperationsData.TypesInfo.ActiveTypes;
                DataContext = this;

                AddValidationButtons();

                PlhContainer.Clear();
                PlhContainer.Add(_rootPlaceHolder);

                LoadinGrid.Visibility = Visibility.Collapsed;
                MainGrid.Visibility = Visibility.Visible;

                PlaceHolder.OnChanged += (sender, args) =>
                {
                    SomeText = _rootPlaceHolder.ToString();
                };
            }));
        }

        private void AddValidationButtons()
        {
            ValidationButtons = new List<ValidationButton>
            {
                new ValidationButton {Name = "Value", Text = "[@value]"},

                new ValidationButton {Name = "RegEx", Text = "RegEx([''], [@value])"},
                new ValidationButton {Name = "IsDigit", Text = "IsDigit([@value])"},
                new ValidationButton {Name = "IsIP", Text = "IsIP([@value])"},
                new ValidationButton {Name = "IsBarcode", Text = "IsBarcode([@value])"},
                new ValidationButton {Name = "IsEmail", Text = "IsEmail([@value])"},
                new ValidationButton {Name = "IsUrl", Text = "IsUrl([@value])"},
                new ValidationButton {Name = "Unique", Text = "Unique([@value])"},

                new ValidationButton {Name = "()", Text = "()"},
//                new ValidationButton {Name = "(", Text = "("},
//                new ValidationButton {Name = ")", Text = ")"},
                new ValidationButton {Name = "and", Text = "and"},
                new ValidationButton {Name = "not", Text = "not"},
                new ValidationButton {Name = "or", Text = "or"},

                new ValidationButton {Name = ">", Text = ">"},
                new ValidationButton {Name = "<", Text = "<"},
                new ValidationButton {Name = "=", Text = "="},
                new ValidationButton {Name = "<>", Text = "<>"},
                new ValidationButton {Name = ">=", Text = ">="},
                new ValidationButton {Name = "<=", Text = "<="},
            };

            TxtValidationExpression.SelectionChanged += TxtValidationExpressionOnSelectionChanged;
            TxtValidationExpression.GotFocus += TxtValidationExpressionOnSelectionChanged;
        }

        private void TxtValidationExpressionOnSelectionChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            ValidationSymbolPosition = "Symbol: " + TxtValidationExpression.SelectionStart;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentType == null)
            {
                MessageBox.Show("Type not defined");
                return;
            }
            if (AssetAttribute == null)
            {
                MessageBox.Show("Attribute not defined");
                return;
            }

            _assetsApi.SaveFormula(CurrentType, AssetAttribute.DbName, SomeText)
                .ContinueWith(a => CallUi(() => MessageBox.Show(a.Result)));
        }

        private void BtnCopyFormula(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(SomeText ?? "");
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTypesInfo();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ValidationButtonClick(object sender, RoutedEventArgs e)
        {
            var validationButton = (ValidationButton) ((Control) sender).Tag;
            InsertValidationOperator(validationButton.Text);
        }

        private void InsertValidationOperator(string operatorText)
        {
            if (ValidationExpression == null)
                ValidationExpression = "";

            var selectionStart = TxtValidationExpression.SelectionStart;
            var selectionLength = TxtValidationExpression.SelectionLength;

            if (operatorText == "()")
            {
                var brackets = new StringBuilder(ValidationExpression);
                brackets.Insert(selectionStart, '(');
                brackets.Insert(selectionStart + selectionLength + 1, ')');
                ValidationExpression = brackets.ToString();
            }
            else
            {
                if (selectionLength > 0)
                {
                    ValidationExpression = ValidationExpression.Remove(selectionStart, selectionLength);
                    TxtValidationExpression.SelectionStart = selectionStart;
                }

                ValidationExpression = ValidationExpression.Insert(selectionStart, " " + operatorText + " ");

                var sb = new StringBuilder();
                // +1 is for space after operator
                var position = selectionStart + operatorText.Length + 1;
                ValidationExpression.Trim().ToCharArray().ForEachWithIndex((c, i) =>
                {
                    if (c == ' ' && sb[sb.Length - 1] == ' ')
                    {
                        if (i <= selectionStart)
                            position--;
                        return;
                    }
                    sb.Append(c);
                });

                ValidationExpression = sb.ToString();
                TxtValidationExpression.SelectionStart = position;
            }

            TxtValidationExpression.Focus();
        }

        private void TestValidationClick(object sender, RoutedEventArgs e)
        {
            _assetsApi.ValidateAttributeAsync(AssetAttribute.Id, ValidationTest, ValidationExpression)
                .ContinueWith(a => CallUi(
                    () =>
                    {
                        var validationResult = a.Result;
                        var result = validationResult.IsValid ? "Success\r\n" : "Fail\r\n";
                        ValidationError = result + validationResult.Message;
                    }));
        }

        private void SaveValidation(object sender, RoutedEventArgs e)
        {
            if (CurrentType == null)
            {
                MessageBox.Show("Type not defined");
                return;
            }
            if (AssetAttribute == null)
            {
                MessageBox.Show("Attribute not defined");
                return;
            }

            _assetsApi.SaveValidation(CurrentType, AssetAttribute.DbName, ValidationExpression)
                .ContinueWith(a => CallUi(() => MessageBox.Show(a.Result)));
        }

        private readonly string[] _elementsToDelete =
        {
            "#masthead",
            "#menubar",
            "#breadcrumb",
            "$('.login-area-block').last()",
            "#footer",
            ".panel.configusers",
            "$('a[href$=\"Batch/Default.aspx\"]').parent()",
            "$('a[href$=\"Import/Default.aspx\"]').parent()",
            "$('a[href$=\"Export/Default.aspx\"]').parent()",
            "$('a[href$=\"LocationMove.aspx\"]').parent()",
            "$('a[href$=\"FAQItems.aspx\"]').parent()",
            "$('a[href$=\"ZipsAndPlaces.aspx\"]').parent()",
            "$('a[href$=\"ServiceOps.aspx\"]').parent()",
        };

        private void GoAdminHome(string server)
        {
            WebBrowserCtl.Source = new Uri(string.Format("{0}/admin/", server));
        }

        private void InitWebView()
        {
            GoAdminHome(Server);

            
            WebBrowserCtl.Navigating += (sender, args) => { BrowserGrid.Visibility = Visibility.Collapsed; };
            WebBrowserCtl.LoadCompleted +=
                (sender, args) =>
                {
                    var script = GetDeleteScript(_elementsToDelete);
                    try
                    {
                        WebBrowserCtl.InvokeScript("eval", script);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("error");
                        GoAdminHome(Server);
                    }
                    BrowserGrid.Visibility = Visibility.Visible;
                };
        }

        private string GetDeleteScript(IEnumerable<string> elements)
        {
            var script = new StringBuilder();
            elements.ToList().ForEach(e =>
            {
                var line = e.StartsWith("$")
                    ? string.Format("{0}.remove();", e)
                    : string.Format("$('{0}').remove();", e);
                script.Append(line);
            });
            return script.ToString();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GoAdminHome(Server);
        }
    }
}