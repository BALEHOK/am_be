using AppFramework.Core.DataTypes;

namespace AppFramework.Core.DAL.Adapters
{
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.Classes;
    using AppFramework.Core.ConstantsEnumerators;
    using Exceptions;
    using Common.Logging;
    using System;
    using System.Globalization;

    public interface IDynColumnAdapter
    {
        /// <summary>
        /// Returns the DAL entity for DB operations
        /// </summary>
        /// <returns></returns>
        DynColumn ConvertAttributeToDynColumn(AssetAttribute attribute);

        /// <summary>
        /// Converts DynEntityAttribConfig to DynColumn entity.
        /// </summary>
        /// <param name="attributeConfiguration">AssetTypeAttribute</param>
        /// <param name="vt">Type of value - for FW or for SQL</param>
        /// <returns>DynColumn</returns>
        DynColumn ConvertDynEntityAttribConfigToDynColumn(Entities.DynEntityAttribConfig attributeConfiguration);
    }

    public class DynColumnAdapter : IDynColumnAdapter
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public DynColumnAdapter(IDataTypeService dataTypeService)
        {
            if (dataTypeService == null)
                throw new ArgumentNullException("IDataTypeService");
            _dataTypeService = dataTypeService;
        }

        /// <summary>
        /// Returns the DAL entity for DB operations
        /// </summary>
        /// <returns></returns>
        public DynColumn ConvertAttributeToDynColumn(AssetAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("AssetAttribute");

            DynColumn column = attribute.GetDataEntity();
            column.IsNull = !attribute.GetConfiguration().IsRequired;

            switch (attribute.Configuration.DataTypeEnum)
            {
                case Enumerators.DataType.DateTime:
                    if (!string.IsNullOrEmpty(attribute.Value))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(attribute.Value, ApplicationSettings.DisplayCultureInfo.DateTimeFormat, DateTimeStyles.None, out dt))
                            column.Value = dt.ToString(ApplicationSettings.PersistenceCultureInfo);
                    }
                    break;

                case Enumerators.DataType.CurrentDate:
                    column.Value = DateTime.Now.ToString(ApplicationSettings.PersistenceCultureInfo);
                    break;

                case Enumerators.DataType.Asset:
                case Enumerators.DataType.Document:
                    column.Value = attribute.ValueAsId ?? default(long);
                    break;

                case Enumerators.DataType.Assets:
                    column.Value = default(long);
                    break;

                case Enumerators.DataType.DynList:
                case Enumerators.DataType.DynLists:
                    column.Value = default(long);
                    break;

                case Enumerators.DataType.Float:
                    if (!string.IsNullOrEmpty(attribute.Value))
                    {
                        float result;
                        if (float.TryParse(attribute.Value, NumberStyles.Float, ApplicationSettings.DisplayCultureInfo, out result) ||
                            float.TryParse(attribute.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                        {
                            column.Value = result;
                        }
                        else
                        {
                            var message = string.Format("Cannot parse as float value \"{0}\" of attribute \"{1}\"",
                                attribute.Value, attribute.Configuration.Name);
                            _logger.Error(message);
                            throw new AttributeConvertionException(message);
                        }
                    }
                    break;

                case Enumerators.DataType.Role:
                    column.Value = (int)Enum.Parse(typeof (PredefinedRoles), attribute.Value);
                    break;

                case Enumerators.DataType.Money:
                case Enumerators.DataType.USD:
                case Enumerators.DataType.Euro:
                    if (!string.IsNullOrEmpty(attribute.Value))
                    {
                        float result;
                        if (float.TryParse(attribute.Value, NumberStyles.Currency, ApplicationSettings.DisplayCultureInfo, out result))
                        {
                            column.Value = result;
                        }
                        else
                        {
                            var message = string.Format("Cannot parse as float value \"{0}\" of attribute \"{1}\"",
                                attribute.Value, attribute.Configuration.Name);
                            _logger.Error(message);
                            throw new AttributeConvertionException(message);
                        }
                    }
                    break;

                default:
                    column.Value = attribute.Value;
                    break;
            }
            return column;
        }        

        /// <summary>
        /// Converts DynEntityAttribConfig to DynColumn entity.
        /// </summary>
        /// <param name="attributeConfiguration">AssetTypeAttribute</param>
        /// <param name="vt">Type of value - for FW or for SQL</param>
        /// <returns>DynColumn</returns>
        public DynColumn ConvertDynEntityAttribConfigToDynColumn(Entities.DynEntityAttribConfig attributeConfiguration)
        {
            if (attributeConfiguration == null)
                throw new ArgumentNullException("DynEntityAttribConfig");

            var datatype = attributeConfiguration.DataType != null 
                ? new CustomDataType(attributeConfiguration.DataType)
                : _dataTypeService.GetByUid(attributeConfiguration.DataTypeUid);

            var column = new DynColumn(attributeConfiguration.DBTableFieldname,
                                       datatype,
                                       !attributeConfiguration.IsRequired,
                                       attributeConfiguration.Name == AttributeNames.DynEntityUid);
            return column;
        }
    }
}
