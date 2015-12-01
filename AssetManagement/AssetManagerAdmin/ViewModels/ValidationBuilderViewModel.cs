using System;
using System.Collections.Generic;
using AssetManagerAdmin.Model;
using GalaSoft.MvvmLight.Command;
using AssetManagerAdmin.Infrastructure;

namespace AssetManagerAdmin.ViewModels
{
    public interface IValidationBuilderViewModel
    {
        event EventHandler<string> OnNewOperator;
        string ValidationExpression { get; set; }
    }

    public class ValidationBuilderViewModel : ToolkitViewModelBase, IValidationBuilderViewModel
    {
        private readonly IDataService _dataService;

        public event EventHandler<string> OnNewOperator;

        #region Properties

        public const string ValidationExpressionPropertyName = "ValidationExpression";
        private string _validationExpression;

        public string ValidationExpression
        {
            get { return _validationExpression; }

            set
            {
                _validationExpression = value;
                RaisePropertyChanged(ValidationExpressionPropertyName);
            }
        }

        public const string ValidationButtonsPropertyName = "ValidationButtons";
        private List<ValidationButton> _validationButtons;

        public List<ValidationButton> ValidationButtons
        {
            get { return _validationButtons; }

            set
            {
                _validationButtons = value;
                RaisePropertyChanged(ValidationButtonsPropertyName);
            }
        }

        public const string ValidationTestPropertyName = "ValidationTest";
        private string _validationTest;

        public string ValidationTest
        {
            get { return _validationTest; }

            set
            {
                _validationTest = value;
                RaisePropertyChanged(ValidationTestPropertyName);
            }
        }

        public const string ValidationErrorPropertyName = "ValidationError";
        private string _validationError;

        public string ValidationError
        {
            get { return _validationError; }

            set
            {
                _validationError = value;
                RaisePropertyChanged(ValidationErrorPropertyName);
            }
        }

        public const string SelectionStartPropertyName = "SelectionStart";
        private int _selectionStart;

        public int SelectionStart
        {
            get { return _selectionStart; }

            set
            {
                _selectionStart = value;
                RaisePropertyChanged(SelectionStartPropertyName);
            }
        }

        public const string SelectionLenghtPropertyName = "SelectionLenght";
        private int _selectionLenght;

        public int SelectionLenght
        {
            get { return _selectionLenght; }

            set
            {
                _selectionLenght = value;
                RaisePropertyChanged(SelectionLenghtPropertyName);
            }
        }

        #endregion

        #region Commands

        private RelayCommand<string> _validationButtonCommand;

        public RelayCommand<string> ValidationButtonCommand
        {
            get
            {
                return _validationButtonCommand ??
                       (_validationButtonCommand = new RelayCommand<string>(InsertValidationOperator, text => true));
            }
        }

        private RelayCommand _testValidationCommand;

        public RelayCommand TestValidationCommand
        {
            get
            {
                return _testValidationCommand ??
                       (_testValidationCommand =
                           new RelayCommand(ExecuteTestValidationCommand,
                               () =>
                                   !string.IsNullOrWhiteSpace(ValidationTest) &&
                                   _dataService.CurrentAssetAttribute != null));
            }
        }

        private void ExecuteTestValidationCommand()
        {
            Api.ValidateAttributeAsync(_dataService.CurrentAssetAttribute.Id, ValidationTest, ValidationExpression)
                .ContinueWith(a =>
                {
                    var validationResult = a.Result;
                    var result = validationResult.IsValid ? "Success\r\n" : "Fail\r\n";
                    ValidationError = result + validationResult.Message;
                });
        }

        private RelayCommand _saveValidatorCommand;

        public RelayCommand SaveValidatorCommand
        {
            get
            {
                return _saveValidatorCommand ??
                       (_saveValidatorCommand =
                           new RelayCommand(ExecuteSaveValidatorCommand,
                               () =>
                                   _dataService.CurrentAssetAttribute != null &&
                                   !string.IsNullOrWhiteSpace(ValidationExpression)));
            }
        }

        private void ExecuteSaveValidatorCommand()
        {
            Api.SaveValidation(_dataService.CurrentAssetType, _dataService.CurrentAssetAttribute.DbName, ValidationExpression)
                .ContinueWith(a =>
                {
                    _dataService.CurrentAssetAttribute.ValidationExpression = ValidationExpression;
                    ValidationError = a.Result;
                });
        }

        #endregion

        private void InsertValidationOperator(string operatorText)
        {
            if (ValidationExpression == null)
                ValidationExpression = "";

            if (OnNewOperator != null)
                OnNewOperator(this, operatorText);
        }

        public ValidationBuilderViewModel(IDataService dataService, IAppContext context)
            : base(context)
        {
            _dataService = dataService;

            ValidationExpression = ValidationError = ValidationTest = string.Empty;

            MessengerInstance.Register(this, MainViewModel.SelectedAttributePropertyName, (object attribute) =>
            {
                if (_dataService.CurrentAssetAttribute != null)
                    ValidationExpression = _dataService.CurrentAssetAttribute.ValidationExpression;
            });

            ValidationButtons = _dataService.GetValidationButtons();
        }
    }
}