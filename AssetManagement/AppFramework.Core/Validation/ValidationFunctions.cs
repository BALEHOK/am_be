using System;
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
        private const string EqualityTestForFloatGreaterThanOne = "ABS(([{0}] - @value) / @value) < 0.000001";
        private const string EqualityTestForFloatLessThanOne = "ABS([{0}] - @value)  < 0.000001";
        private const string CommonEqualityTest = "[{0}] = @value";
        private readonly IUnitOfWork _unitOfWork;
        private readonly AssetAttribute _attribute;

        public ValidationFunctions(IUnitOfWork unitOfWork, AssetAttribute attribute)
        {
            _unitOfWork = unitOfWork;
            _attribute = attribute;

            Functions.Add("ISDIGIT", IsDigit);
            Functions.Add("REGEX", RegEx);
            Functions.Add("ISIP", IsIP);
            Functions.Add("ISBARCODE", IsCorrectBarcode);
            Functions.Add("ISEMAIL", IsCorrectEmail);
            Functions.Add("ISURL", IsUrl);
            Functions.Add("UNIQUE", Unique);
            Functions.Add("SYSTEMUNIQUE", SystemUnique);
        }

        private bool Unique(ValidationResult validation, object[] parameters)
        {
            var value = parameters[0].ToString();

            var result = ValidationResultLine.Success;

            if (string.IsNullOrEmpty(value))
                return result.IsValid;

            if (_attribute.ParentAsset != null)
            {
                result.IsValid = _unitOfWork.IsValueUnique(_attribute.ParentAsset.GetConfiguration().DBTableName,
                    _attribute.Configuration.DBTableFieldName,
                    value,
                    _attribute.ParentAsset.ID);
            }
            else
            {
                var count =
                    _unitOfWork.SqlProvider.ExecuteScalar(
                    string.Format("SELECT COUNT(*) FROM [{1}] WHERE " + GetEqualityTest(ref value),
                            _attribute.Configuration.DBTableFieldName,
                            _attribute.Configuration.Parent.DBTableName),
                        new IDataParameter[] {new SqlParameter("@value", value)});

                result.IsValid = int.Parse(count.ToString()) == 0;
            }

            result.Message = !result.IsValid
                ? string.Format(
                    "An entity with the attribute <i>{0}</i> = <i>{1}</i> already exists. Value must be unique.",
                    _attribute.Configuration.NameLocalized, value)
                : null;

            validation.ResultLines.Add(result);
            return result.IsValid;
        }

        private bool SystemUnique(ValidationResult validation, object[] parameters)
        {
            var value = parameters[0].ToString();

            var dbDataType = DataTypeService.ConvertToDbDataType(_attribute.Configuration.DataType);

            long assetId;
            long assetConfigId;
            if (_attribute.ParentAsset != null)
            {
                assetId = _attribute.ParentAsset.ID;
                assetConfigId = _attribute.ParentAsset.DynEntityConfigUid;
            }
            else
            {
                assetId = 0;
                assetConfigId = 0;
            }
            
            var foundValues =
                _unitOfWork.SqlProvider.ExecuteScalar(
                    GetSqlForUniquenessRequest(_attribute.Configuration.Name, dbDataType, GetEqualityTest(ref value)),
                    new IDataParameter[]
                    {
                        new SqlParameter("@attrValue", value),
                        new SqlParameter("@DynEntityId", assetId),
                        new SqlParameter("@DynEntityConfigUid", assetConfigId)
                    });
            

            ValidationResultLine result;
            if (foundValues == null)
            {
                result = ValidationResultLine.Success;
            }
            else
            {
                var message = string.Format(
                    "An entity with the attribute <i>{0}</i> = <i>{1}</i> already exists. Value must be unique in the whole system.",
                    _attribute.Configuration.NameLocalized, value);
                result = ValidationResultLine.Error(string.Empty, message);
            }

            validation.ResultLines.Add(result);
            return result.IsValid;
        }

        private string GetEqualityTest(ref string value)
        {
            var equalityTest = CommonEqualityTest;
            var isFloatValue = _attribute.Configuration.Name == "float";
            if (isFloatValue)
            {
                float floatValue;
                if (float.TryParse(value, out floatValue))
                {
                    equalityTest = Math.Abs(floatValue) < 1
                        ? EqualityTestForFloatLessThanOne
                        : EqualityTestForFloatGreaterThanOne;
                }
                value = value.Replace(",", ".");
            }
            return equalityTest;
        }

        private static string GetSqlForUniquenessRequest(string attributeName, string dbDataType, string equalityTest)
        {
            equalityTest = string.Format(equalityTest, attributeName);
            // first select all types that contain attribute with the same name
            // (i.e. put all the tables containing the attribute in a single UNION SELECT)
            // then check uniqueness among all the values in all the selected tables
            return string.Format(
                @"
DECLARE @union NVARCHAR(max);
SELECT @union = COALESCE(@union + ' UNION ', '')
		+ '(SELECT [DynEntityId], [DynEntityConfigUid], [' + clmn + '] as [{0}] FROM ' + tbl + ' WHERE ActiveVersion = 1)'
FROM (
	SELECT [DBTableName] as tbl
		,[DBTableFieldname] as clmn
	FROM [DynEntityAttribConfig] attr JOIN [DynEntityConfig] t
		ON attr.DynEntityConfigUid = t.DynEntityConfigId
	WHERE attr.Name = '{0}') as tt

DECLARE @query NVARCHAR(max);
SET @query = N'SELECT TOP 1 1 from (' + @union + ') as allValues WHERE {1} AND NOT ([DynEntityId] = @id AND [DynEntityConfigUid] = @config)'

EXEC sp_executesql @query, N'@value {2},@id bigint,@config bigint',@value=@attrValue,@id=@DynEntityId,@config=@DynEntityConfigUid",
                attributeName,
                equalityTest,
                dbDataType);
        }

        private bool IsUrl(ValidationResult validation, object[] parameters)
        {
            var value = (string) parameters[0];
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
            var value = (string) parameters[0];
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
            var value = (string) parameters[0];

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
            var expression = new Regex((string) parameters[0]);
            var value = (string) parameters[1];

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
            var value = (string) parameters[0];
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