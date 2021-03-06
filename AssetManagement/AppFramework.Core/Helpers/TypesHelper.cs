﻿using System;
using System.Collections.Generic;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.DataTypes;

namespace AppFramework.Core.Helpers
{
    public class TypesHelper
    {
        public static T GetTypedValue<T>(object value)
        {
            if (value == null || (value is string && string.IsNullOrWhiteSpace((string)value)))
                return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T GetTypedValueWithDefault<T>(object value, T defaultValue)
        {
            if (value == null || (value is string && string.IsNullOrWhiteSpace((string)value)))
                return defaultValue;

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static object GetTypedValue(string typeName, object value)
        {            
            var type = Type.GetType(typeName);
            return GetTypedValue(type, value);
        }

        private static Type _dateTimeType = typeof(DateTime);
        public static object GetTypedValue(Type type, object value)
        {
            var valueStr = value as string;
            if (value == null || (value is string && string.IsNullOrWhiteSpace(valueStr)))
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            if (type == _dateTimeType)
            {
                DateTime d;
                if (!DateTime.TryParse(valueStr, out d))
                {
                    // this format is used in datetime attribute editor in React app
                    d = DateTime.ParseExact(valueStr, "M/d/yyyy HH:mm:ss", null);
                }
                return d;
            }

            return Convert.ChangeType(value, type);
        }

        public static object GetTypedValue(DataTypeBase type, object value)
        {
            return GetTypedValue(type.FrameworkDataType, value);
        }

        public static object GetTypedValue(Enumerators.DataType type, object value)
        {
            switch (type)
            {
                case Enumerators.DataType.Bool:
                    return GetTypedValue<bool>(value);
                case Enumerators.DataType.Int:
                case Enumerators.DataType.Revision:
                    return GetTypedValue<int>(value);

                case Enumerators.DataType.Long:
                    return GetTypedValue<long>(value);

                case Enumerators.DataType.Float:
                    return GetTypedValue<double>(value);

                case Enumerators.DataType.Money:
                case Enumerators.DataType.Euro:
                    return GetTypedValue<decimal>(value);

                case Enumerators.DataType.Char:
                    return GetTypedValue<char>(value);

                case Enumerators.DataType.String:
                case Enumerators.DataType.Url:
                case Enumerators.DataType.Email:
                case Enumerators.DataType.Text:
                    return GetTypedValue<string>(value);

                case Enumerators.DataType.CurrentDate:
                case Enumerators.DataType.DateTime:
                    return GetTypedValue<DateTime>(value);

                //                case Enumerators.DataType.Asset:
                //                    return GetTypedValue<string>(value);

                //                case Enumerators.DataType.Assets:
                //                    return GetTypedValue<string>(value);

                default:
                    return value;
            }
        }

        public static object GetTypedValue(Asset asset, AssetTypeAttribute attribute)
        {
            var value = asset[attribute.DBTableFieldName].Value;
            return GetTypedValue(attribute.DataTypeEnum, value);
        }

        public static object GetTypedValue(ScreenAttrs assetAttributes, string parameterName)
        {
            var targetAttrConfig = assetAttributes[parameterName].Configuration;
            return GetTypedValue(targetAttrConfig.DataType, assetAttributes[targetAttrConfig.DBTableFieldName].Value);
        }
    }
}