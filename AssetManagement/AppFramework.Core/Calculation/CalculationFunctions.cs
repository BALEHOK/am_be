using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.Helpers;
using AppFramework.DataProxy;

namespace AppFramework.Core.Calculation
{
    public static class DateTimeExtender
    {
        public static bool IsWorkingDay(this DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday
                   && date.DayOfWeek != DayOfWeek.Sunday;
        }
    }

    public class CalculationFunctions : FunctionsFactory<object, Asset>
    {
        private readonly IAssetTypeRepository _typeRepository;
        private readonly IAssetsService _assetsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttributeCalculator _calculator;

        public CalculationFunctions(IUnitOfWork unitOfWork, IAssetTypeRepository typeRepository,
            IAssetsService assetsService, IAttributeCalculator calculator)
        {
            _typeRepository = typeRepository;
            _assetsService = assetsService;
            _unitOfWork = unitOfWork;
            _calculator = calculator;

            // datetime
            Functions.Add("NOW", DateNowFunction);
            Functions.Add("NOWGMT", DateNowGmtFunction);
            Functions.Add("DATE", DateFunction);
            Functions.Add("DAY", DayFunction);
            Functions.Add("MONTH", MonthFunction);
            Functions.Add("YEAR", YearFunction);
            Functions.Add("DAYOFWEEK", DayOfWeekFunction);
            Functions.Add("DAYOFYEAR", DayOfYearFunction);
            Functions.Add("HOUR", HourFunction);
            Functions.Add("MINUTE", MinuteFunction);
            Functions.Add("SECOND", SecondFunction);
            Functions.Add("WORKINGDAYS", WorkingDaysFunction);

            // text
            Functions.Add("LENGTH", LengthFunction);
            Functions.Add("LEFT", LeftFunction);
            Functions.Add("RIGHT", RightFunction);
            Functions.Add("MID", MidFunction);
            Functions.Add("TRIM", TrimFunction);
            Functions.Add("VALUE", ValueFunction);
            Functions.Add("SEARCH", SearchFunction);
            Functions.Add("LOWER", LowerFunction);
            Functions.Add("UPPER", UpperFunction);
            Functions.Add("REPLACEALL", ReplaceAllFunction);
            Functions.Add("PROPERCASE", ProperCaseFunction);

            // math
            Functions.Add("ABS", AbsFunction);
            Functions.Add("TRUNCATE", TruncateFunction);
            Functions.Add("ROUND", RoundFunction);
            Functions.Add("ROUNDTO", RoundToFunction);
            Functions.Add("REMAINDER", RemainderFunction);
            Functions.Add("TOMONEY", ToMoneyFunction);

            // statistical
            Functions.Add("SUM", SumFunction);
            Functions.Add("COUNT", CountFunction);
            Functions.Add("AVERAGE", AverageFunction);
            Functions.Add("MAX", MaxFunction);
            Functions.Add("MIN", MinFunction);

            // query
            Functions.Add("SELECT", SelectFunction);
            Functions.Add("SQLFIND", SqlFindFunction);
            Functions.Add("SQLINDEX", SqlIndexFunction);

            // boolean
            Functions.Add("NOT", NotFunction);

            // brackets
            Functions.Add("BR", BracketsFunction);
        }

        private object BracketsFunction(Asset typedparameter, object[] parameters)
        {
            return parameters[0];
        }

        private object SqlIndexFunction(Asset asset, object[] parameters)
        {
            var tableName = (string)parameters[0];
            var fieldName = (string)parameters[1];
            var assetId = TypesHelper.GetTypedValue<string>(parameters[2]);

            var assetById =
                ((AssetsService)_assetsService).GetAssetsByParameters(tableName, new Dictionary<string, string>
                {
                    {AttributeNames.DynEntityId, assetId}
                }).FirstOrDefault();

            var result = assetById == null ? string.Empty : assetById[fieldName].Value;
            return result;
        }

        private object SqlFindFunction(Asset asset, object[] parameters)
        {
            return SelectFunction(asset, parameters);
        }

        private object SelectFunction(Asset asset, object[] parameters)
        {
            var tableName = (string) parameters[0];
            var filterFieldName = (string) parameters[1];
            var filterValue = parameters[2].ToString();
            var expression = parameters.Length > 3 ? (string) parameters[3] : string.Empty;

            var assets =
                ((AssetsService) _assetsService).GetAssetsByParameters(tableName, new Dictionary<string, string>
                {
                    {filterFieldName, filterValue}
                }).ToList();

            dynamic result;

            if (!string.IsNullOrWhiteSpace(expression))
            {
                ((AttributeCalculator) _calculator).CallingAsset = asset;
                result = assets.Select(a => _calculator.GetValue(a, expression, asset.ID)).ToList();
                ((AttributeCalculator) _calculator).CallingAsset = null;
            }
            else
            {
                result = assets.Count == 0 ? 0 : assets.First().ID;
            }

            return result;
        }

        private object ToMoneyFunction(Asset asset, object[] parameters)
        {
            var value = TypesHelper.GetTypedValue<decimal>(parameters[0]);
            return value;
        }

        private object NotFunction(Asset asset, object[] parameters)
        {
            var b = TypesHelper.GetTypedValue<bool>(parameters[0]);
            return !b;
        }

        #region date & time functions

        private object WorkingDaysFunction(Asset asset, object[] parameters)
        {
            var fromDate = (DateTime)parameters[0];
            var toDate = (DateTime)parameters[1];

            var dates = Enumerable.Range(0, toDate.Subtract(fromDate).Days + 1).Select(d => fromDate.AddDays(d));
            return dates.Count(d => d.IsWorkingDay());
        }

        private object SecondFunction(Asset asset, object[] parameters)
        {
            var date = (DateTime)parameters[0];
            return date.Second;
        }

        private object MinuteFunction(Asset asset, object[] parameters)
        {
            var date = (DateTime)parameters[0];
            return date.Minute;
        }

        private object HourFunction(Asset asset, object[] parameters)
        {
            var date = (DateTime)parameters[0];
            return date.Hour;
        }

        private object DayOfYearFunction(Asset asset, object[] parameters)
        {
            var date = (DateTime)parameters[0];
            return date.DayOfYear;
        }

        private object DayOfWeekFunction(Asset asset, object[] parameters)
        {
            var date = (DateTime)parameters[0];
            return date.DayOfWeek;
        }

        private object YearFunction(Asset asset, object[] parameters)
        {
            var date = (DateTime)parameters[0];
            return date.Year;
        }

        private object MonthFunction(Asset asset, object[] parameters)
        {
            var date = (DateTime)parameters[0];
            return date.Month;
        }

        private object DayFunction(Asset asset, object[] parameters)
        {
            var date = (DateTime)parameters[0];
            return date.Day;
        }

        private object DateFunction(Asset asset, object[] parameters)
        {
            var month = (int)parameters[0];
            var day = (int)parameters[1];
            var year = (int)parameters[2];
            var date = new DateTime(year, month, day);
            return date;
        }

        private object DateNowGmtFunction(Asset asset, object[] parameters)
        {
            return DateTime.UtcNow;
        }

        private object DateNowFunction(Asset asset, object[] parameters)
        {
            return DateTime.Now;
        }

        #endregion

        #region string functions

        private object ProperCaseFunction(Asset asset, object[] parameters)
        {
            var str = TypesHelper.GetTypedValueWithDefault(parameters[0], string.Empty);
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }

        private object ReplaceAllFunction(Asset asset, object[] parameters)
        {
            var str = TypesHelper.GetTypedValueWithDefault(parameters[0], string.Empty);
            var oldText = TypesHelper.GetTypedValueWithDefault(parameters[1], string.Empty);
            var newText = TypesHelper.GetTypedValueWithDefault(parameters[2], string.Empty);

            return str.Replace(oldText, newText);
        }

        private object UpperFunction(Asset asset, object[] parameters)
        {
            var str = TypesHelper.GetTypedValueWithDefault(parameters[0], string.Empty);
            return str.ToUpper();
        }

        private object LowerFunction(Asset asset, object[] parameters)
        {
            var str = TypesHelper.GetTypedValueWithDefault(parameters[0], string.Empty);
            return str.ToLower();
        }

        private object SearchFunction(Asset asset, object[] parameters)
        {
            var findStr = TypesHelper.GetTypedValueWithDefault(parameters[0], string.Empty);
            var str = TypesHelper.GetTypedValueWithDefault(parameters[1], string.Empty);
            return str.IndexOf(findStr, StringComparison.CurrentCulture);
        }

        private object ValueFunction(Asset asset, object[] parameters)
        {
            var str = TypesHelper.GetTypedValueWithDefault(parameters[0], string.Empty);
            Decimal number;
            if (Decimal.TryParse(str, out number))
                return number;

            return null;
        }

        private object TrimFunction(Asset asset, object[] parameters)
        {
            var str = TypesHelper.GetTypedValueWithDefault(parameters[0], string.Empty);
            return str.Trim();
        }

        private object MidFunction(Asset asset, object[] parameters)
        {
            var str = TypesHelper.GetTypedValueWithDefault(parameters[0], string.Empty);
            var start = (int)parameters[1];
            var number = (int)parameters[2];
            return str.Substring(start, number);
        }

        private object RightFunction(Asset asset, object[] parameters)
        {
            var str = TypesHelper.GetTypedValueWithDefault(parameters[0], string.Empty);
            var number = (int)parameters[1];
            return str.Substring(str.Length - number, number);
        }

        private object LeftFunction(Asset asset, object[] parameters)
        {
            var str = TypesHelper.GetTypedValueWithDefault(parameters[0], string.Empty);
            var number = (int)parameters[1];
            return str.Substring(0, number);
        }

        private object LengthFunction(Asset asset, object[] parameters)
        {
            var str = TypesHelper.GetTypedValueWithDefault(parameters[0], string.Empty);
            return str.Length;
        }

        #endregion

        #region math functions

        private object RemainderFunction(Asset asset, object[] parameters)
        {
            var x = TypesHelper.GetTypedValue<decimal>(parameters[0]);
            var y = TypesHelper.GetTypedValue<decimal>(parameters[1]);
            var remainder = x % y;
            return remainder;
        }

        private object RoundToFunction(Asset asset, object[] parameters)
        {
            var number = TypesHelper.GetTypedValue<decimal>(parameters[0]);
            var decimals = TypesHelper.GetTypedValue<int>(parameters[1]);
            return Math.Round(number, decimals);
        }

        private object RoundFunction(Asset asset, object[] parameters)
        {
            var number = TypesHelper.GetTypedValue<decimal>(parameters[0]);
            return Math.Round(number);
        }

        private object TruncateFunction(Asset asset, object[] parameters)
        {
            var number = TypesHelper.GetTypedValue<decimal>(parameters[0]);
            return Math.Truncate(number);
        }

        private object AbsFunction(Asset asset, object[] parameters)
        {
            var number = TypesHelper.GetTypedValue<decimal>(parameters[0]);
            return Math.Abs(number);
        }

        #endregion

        #region statistical functions

        private object GetScalarWithFilter(string tableName, string filterFieldName, object filterValue,
            string sqlFunction)
        {
            var queryString =
                string.Format(
                    @"SELECT {3} FROM [{0}] WHERE [ActiveVersion] = 1 AND [{1}] = {2}",
                    tableName, filterFieldName, filterValue, sqlFunction);

            var result = _unitOfWork.SqlProvider.ExecuteScalar(queryString);

            if (result is DBNull)
                return "";

            return result;
        }

        private object SumFunction(Asset asset, params object[] parameters)
        {
            if (parameters.Length > 1)
                return SumFunction_old(asset, parameters);

            var list = parameters[0] as List<object>;

            decimal? result = null;

            if (list != null)
                result = list.Select(TypesHelper.GetTypedValue<decimal>).Sum();

            return result;
        }

        //todo: remove it later
        private object SumFunction_old(Asset asset, params object[] parameters)
        {
            var tableName = (string)parameters[0];
            var countField = (string)parameters[1];
            var filterFieldName = (string)parameters[2];
            var filterValue = parameters[3].ToString();

            var sqlFunction = string.Format("SUM({0})", countField);
            var result = GetScalarWithFilter(tableName, filterFieldName, filterValue, sqlFunction);

            return result;
        }

        private object CountFunction(Asset asset, params object[] parameters)
        {
            var tableName = (string)parameters[0];
            var filterFieldName = (string)parameters[1];
            var filterValue = parameters[2].ToString();

            var result = GetScalarWithFilter(tableName, filterFieldName, filterValue, "COUNT(*)");

            return result;
        }

        private object AverageFunction(Asset asset, object[] parameters)
        {
            var tableName = (string)parameters[0];
            var countField = (string)parameters[1];
            var filterFieldName = (string)parameters[2];
            var filterValue = parameters[3].ToString();

            var sqlFunction = string.Format("AVG({0})", countField);
            var result = GetScalarWithFilter(tableName, filterFieldName, filterValue, sqlFunction);

            return result;
        }

        private object MaxFunction(Asset asset, object[] parameters)
        {
            var tableName = (string)parameters[0];
            var countField = (string)parameters[1];
            var filterFieldName = (string)parameters[2];
            var filterValue = parameters[3].ToString();

            var sqlFunction = string.Format("MAX({0})", countField);
            var result = GetScalarWithFilter(tableName, filterFieldName, filterValue, sqlFunction);

            return result;
        }

        private object MinFunction(Asset asset, object[] parameters)
        {
            var tableName = (string)parameters[0];
            var countField = (string)parameters[1];
            var filterFieldName = (string)parameters[2];
            var filterValue = parameters[3].ToString();

            var sqlFunction = string.Format("MIN({0})", countField);
            var result = GetScalarWithFilter(tableName, filterFieldName, filterValue, sqlFunction);

            return result;
        }

        #endregion
    }
}