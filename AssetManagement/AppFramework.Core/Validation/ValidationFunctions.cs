using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Barcode;
using AppFramework.DataProxy;

namespace AppFramework.Core.Validation
{
    public class ValidationFunctions : FunctionsFactory<bool, ValidationResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidationFunctions(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Functions.Add("ISDIGIT", IsDigit);
            Functions.Add("REGEX", RegEx);
            Functions.Add("ISIP", IsIP);
            Functions.Add("ISBARCODE", IsCorrectBarcode);
            Functions.Add("ISEMAIL", IsCorrectEmail);
            Functions.Add("ISURL", IsUrl);
            Functions.Add("UNIQUE", Unique);
        }

        public AssetAttribute Attribute { get; set; }

        private bool Unique(ValidationResult validation, object[] parameters)
        {
            var value = parameters[0].ToString();

            var result = ValidationResultLine.Success;

            if (string.IsNullOrEmpty(value))
                return result.IsValid;

            if (Attribute.ParentAsset != null)
            {
                result.IsValid = _unitOfWork.IsValueUnique(Attribute.ParentAsset.GetConfiguration().DBTableName,
                    Attribute.Configuration.DBTableFieldName,
                    value,
                    Attribute.ParentAsset.ID);
            }
            else
            {
                var count =
                    _unitOfWork.SqlProvider.ExecuteScalar(
                        string.Format("SELECT COUNT(*) FROM [{0}] WHERE [{1}] = @value",
                            Attribute.Configuration.Parent.DBTableName,
                            Attribute.Configuration.DBTableFieldName),
                        new IDataParameter[] { new SqlParameter("@value", value) });

                result.IsValid = int.Parse(count.ToString()) == 0;
            }

            result.Message = !result.IsValid
                ? string.Format(
                    "An entity with the attribute <i>{0}</i> = <i>{1}</i> already exists. Value must be unique.",
                    Attribute.Configuration.NameLocalized, value)
                : null;

            validation.ResultLines.Add(result);
            return result.IsValid;
        }

        private bool IsUrl(ValidationResult validation, object[] parameters)
        {
            var value = (string)parameters[0];
            var match = Regex.Match(value,
                @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)");

            var result = new ValidationResultLine(string.Empty)
            {
                IsValid = match.Success
            };
            result.Message = !result.IsValid ? string.Format("Value '{0}' is not a valid URL", value) : null;

            validation.ResultLines.Add(result);
            return result.IsValid;
        }

        private bool IsCorrectEmail(ValidationResult validation, object[] parameters)
        {
            var value = (string)parameters[0];
            var match = Regex.Match(value, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

            var result = new ValidationResultLine(string.Empty)
            {
                IsValid = match.Success
            };
            result.Message = !result.IsValid ? string.Format("Value '{0}' is not a valid e-mail", value) : null;

            validation.ResultLines.Add(result);
            return result.IsValid;
        }

        private bool IsCorrectBarcode(ValidationResult validation, object[] parameters)
        {
            var value = (string)parameters[0];

            var barcodeProvider = new DefaultBarcodeProvider();

            var result = new ValidationResultLine(string.Empty)
            {
                IsValid = string.IsNullOrEmpty(value) || barcodeProvider.ValidateBarcode(value)
            };
            result.Message = !result.IsValid ? string.Format("Value '{0}' is not a valid barcode", value) : null;

            validation.ResultLines.Add(result);
            return result.IsValid;
        }

        private bool IsDigit(ValidationResult validation, object[] parameters)
        {
            var value = parameters[0];

            var isNumeric = value is sbyte
                            || value is byte
                            || value is short
                            || value is ushort
                            || value is int
                            || value is uint
                            || value is long
                            || value is ulong
                            || value is float
                            || value is double
                            || value is decimal;

            var result = new ValidationResultLine(string.Empty)
            {
                IsValid = isNumeric
            };
            result.Message = !result.IsValid ? string.Format("Value '{0}' is not a number", value) : null;

            validation.ResultLines.Add(result);
            return result.IsValid;
        }

        private bool RegEx(ValidationResult validation, object[] parameters)
        {
            var expression = new Regex((string)parameters[0]);
            var value = (string)parameters[1];

            var match = expression.Match(value);

            var result = new ValidationResultLine(string.Empty)
            {
                IsValid = match.Success
            };
            result.Message = !result.IsValid ? string.Format("Validation expression failed: '{0}'", expression) : null;

            validation.ResultLines.Add(result);
            return result.IsValid;
        }

        private bool IsIP(ValidationResult validation, object[] parameters)
        {
            var value = (string)parameters[0];
            var match = Regex.Match(value,
                @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");

            var result = new ValidationResultLine(string.Empty)
            {
                IsValid = match.Success
            };
            result.Message = !result.IsValid ? string.Format("Value '{0}' is not a valid IP address", value) : null;

            validation.ResultLines.Add(result);
            return result.IsValid;
        }
    }
}