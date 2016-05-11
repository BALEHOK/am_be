using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AppFramework.Core.Classes.Extensions;
using AppFramework.Core.Classes.SearchEngine.ContextSearchElements;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.SearchEngine
{
    public static class ExtensionMethods
    {
        // TODO refactor to remove
        public static IEnumerable<AttributeElement> ToSearchChains(
            this IEnumerable<AttributeElement> elements, 
            AssetType assetType, 
            IUnitOfWork unitOfWork,
            IAssetsService assetsService,
            IAssetTypeRepository assetTypeRepository)
        {
            if (assetType == null)
                throw new ArgumentNullException();
            if (unitOfWork == null)
                throw new ArgumentNullException();
            if (assetsService == null)
                throw new ArgumentNullException();
            OneType CurrentType = new OneType(assetType, unitOfWork, assetsService, assetTypeRepository);
            string queryString = string.Empty;
            int i = 0;
            AttributeElement item = null;
            if (elements.Count() == 1 && (item = elements.FirstOrDefault()).OperatorId == -1)
            {

                AttributeElement transformedElement = new AttributeElement();
                transformedElement.StartBrackets = item.StartBrackets;
                transformedElement.FieldName = CurrentType.Attributes[item.AttributeId].Configuration.Name;
                transformedElement.FieldSql = CurrentType.Attributes[item.AttributeId].Configuration.DBTableFieldName;
                transformedElement.EndBrackets = item.EndBrackets;
                transformedElement.AndOr = item.AndOr;
                transformedElement.AscDesc = item.AscDesc;
                transformedElement.ServiceMethod = "EqualOperator";
                queryString += string.Format("{0}{1} {2} ", transformedElement.StartBrackets, transformedElement.FieldName, "==");
                transformedElement.ElementType = CurrentType.Attributes[item.AttributeId].Configuration.DataTypeEnum;
                transformedElement.Value = item.AssetAttribute.ValueAsId.Value.ToString();
                queryString += item.AssetAttribute.Value;
                yield return transformedElement;
            }
            else
            {
                foreach (AttributeElement element in elements.Where(e => !string.IsNullOrEmpty(e.Text) || e.ItemId >= 0 || e.AssetAttribute != null))
                {
                    if (CurrentType.Attributes[element.AttributeId].Operators.Count == 0)
                        continue;

                    AttributeElement transformedElement = new AttributeElement();
                    transformedElement.StartBrackets = element.StartBrackets;
                    transformedElement.FieldName = CurrentType.Attributes[element.AttributeId].FieldText;
                    transformedElement.FieldSql = CurrentType.Attributes[element.AttributeId].FieldSql;
                    transformedElement.ServiceMethod = CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].OperatorMethod;
                    transformedElement.EndBrackets = element.EndBrackets;
                    transformedElement.AndOr = element.AndOr;
                    transformedElement.AscDesc = element.AscDesc;
                    transformedElement.ElementType = CurrentType.Attributes[element.AttributeId].Configuration.DataTypeEnum;

                    var @operator = CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId];
                    if (@operator.IsAssetListDropDown || @operator.IsDynListDropDown)
                    {
                        queryString += string.Format("{0}{1} {2} ", transformedElement.StartBrackets, transformedElement.FieldName,
                            CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].OperatorText);

                        // get the text from dropdown but not id for search on MultipleAssets
                        if (transformedElement.ElementType == AppFramework.ConstantsEnumerators.Enumerators.DataType.Assets ||
                            transformedElement.ElementType == AppFramework.ConstantsEnumerators.Enumerators.DataType.Asset)
                        {
                            Debug.Assert(element.AssetAttribute.ValueAsId != null, "element.Asset.ValueAsId != null");
                            transformedElement.Value = element.AssetAttribute.ValueAsId.Value.ToString();
                            queryString += element.AssetAttribute.Value;
                        }
                        else
                        {
                            transformedElement.Value = CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].Items[element.ItemId].Value.ToString();
                            queryString += CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].Items[element.ItemId].Key.ToString();
                        }
                    }
                    else
                    {
                        switch (transformedElement.ElementType)
                        {
                            case AppFramework.ConstantsEnumerators.Enumerators.DataType.CurrentDate:
                            case AppFramework.ConstantsEnumerators.Enumerators.DataType.DateTime:
                                DateTime dt;

                                if (DateTime.TryParse(element.Text, ApplicationSettings.DisplayCultureInfo.DateTimeFormat, DateTimeStyles.None, out dt))
                                {
                                    if (dt.TimeOfDay.TotalSeconds == 0)
                                    {
                                        transformedElement.FieldSql = "CAST(" + transformedElement.FieldSql + " AS DATE" + ")";
                                        transformedElement.Value = dt.ToShortDateString();
                                    }
                                    else
                                    {
                                        transformedElement.Value = dt.ToString();
                                    }
                                }

                                queryString += element.Text;
                                break;
                            default:
                                queryString += string.Format("{0}{1} {2} ", transformedElement.StartBrackets, transformedElement.FieldName,
                                    CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].OperatorText);

                                transformedElement.Value = element.Text;
                                queryString += element.Text;
                                break;
                        }
                    }
                    yield return transformedElement;

                    queryString += transformedElement.EndBrackets;
                    if (i != elements.Count() - 1)
                    {
                        queryString += transformedElement.AndOr == 0 ? " and " : " or ";
                    }
                    i++;
                }
            }
        }

        // TODO refactor to remove
        public static IEnumerable<AttributeElement> ToSearchChains(this IEnumerable<AttributeElement> elements, ContextForSearch CurrentType)
        {
            foreach (AttributeElement element in elements.Where(e => !string.IsNullOrEmpty(e.Text) || e.ItemId >= 0 || e.AssetAttribute != null))
            {
                if (CurrentType.Attributes[element.AttributeId].Operators.Count == 0)
                    continue;
                AttributeElement chain = new AttributeElement();
                chain.ContextUID = CurrentType.Attributes[element.AttributeId].Value;
                chain.StartBrackets = element.StartBrackets;
                chain.FieldName = CurrentType.Attributes[element.AttributeId].Text;
                chain.ServiceMethod = CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].OperatorMethod;
                chain.ElementType = CurrentType.Attributes[element.AttributeId].DataTypeEnum;
                var @operator = CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId];
                if (@operator.IsAssetListDropDown || @operator.IsDynListDropDown)
                {
                    chain.Value = CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].Items[element.ItemId].Value.ToString();
                }
                else
                {
                    chain.Value = element.Text;
                }
                if (element.DateType == AppFramework.ConstantsEnumerators.Enumerators.DataType.DynList)
                {
                    chain.DynListItemId = element.DynListItemId;
                }
                else if (element.DateType == AppFramework.ConstantsEnumerators.Enumerators.DataType.Asset)
                {
                    chain.Value = element.AssetAttribute.ValueAsId.ToString();
                }
                chain.EndBrackets = element.EndBrackets;
                chain.AndOr = element.AndOr;
                chain.AscDesc = element.AscDesc;
                yield return chain;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="CurrentType"></param>
        /// <returns></returns>
        public static string ToVerbalString(this IEnumerable<AttributeElement> elements, ContextForSearch CurrentType)
        {
            string queryString = string.Empty;
            int i = 0;
            foreach (AttributeElement element in elements.Where(e => !string.IsNullOrEmpty(e.Text) || e.ItemId >= 0 || e.AssetAttribute != null))
            {
                if (CurrentType.Attributes[element.AttributeId].Operators.Count == 0)
                    continue;
                queryString += string.Format("{0}{1} {2} ", element.StartBrackets, CurrentType.Attributes[element.AttributeId].Text,
                        CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].OperatorText);
                var @operator = CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId];
                if (@operator.IsAssetListDropDown || @operator.IsDynListDropDown)
                {
                    queryString += CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].Items[element.ItemId].Key;
                }
                else if (element.DateType == AppFramework.ConstantsEnumerators.Enumerators.DataType.Asset)
                {
                    queryString += element.AssetAttribute.RelatedAsset != null ? element.AssetAttribute.RelatedAsset.Name : string.Empty;
                }
                else
                {
                    queryString += element.Text;
                }
                queryString += element.EndBrackets;
                if (i != elements.Count() - 1)
                {
                    queryString += element.AndOr == 0 ? " and " : " or ";
                }
                i++;
            }
            return queryString;
        }

        public static string ToVerbalString(
            this IEnumerable<AttributeElement> elements, 
            AssetType assetType,
            IUnitOfWork unitOfWork,
            IAssetsService assetsService,
            IAssetTypeRepository assetTypeRepository)
        {
            if (assetType == null)
                throw new ArgumentNullException();
            if (unitOfWork == null)
                throw new ArgumentNullException();
            if (assetsService == null)
                throw new ArgumentNullException();
            OneType CurrentType = new OneType(assetType, unitOfWork, assetsService, assetTypeRepository);
            string queryString = string.Empty;
            int i = 0;
            AttributeElement item = null;
            if (elements.Count() == 1 && (item = elements.FirstOrDefault()).OperatorId == -1 &&
                 item.DateType == AppFramework.ConstantsEnumerators.Enumerators.DataType.Asset)
            {

                queryString += string.Format("{0}{1} {2} ",
                        CurrentType.Attributes[item.AttributeId].AttributeText.Localized(),
                        "==", item.AssetAttribute.Value.Localized());
            }
            else
            {
                foreach (AttributeElement element in elements.Where(e => !string.IsNullOrEmpty(e.Text) || e.ItemId >= 0 || e.AssetAttribute != null))
                {
                    if (CurrentType.Attributes[element.AttributeId].Operators.Count == 0)
                        continue;

                    queryString += string.Format("{0}{1} {2} ",
                        element.StartBrackets,
                        CurrentType.Attributes[element.AttributeId].AttributeText,
                        CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].OperatorText);

                    var @operator = CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId];
                    
                    if (element.AssetAttribute != null)
                    {
                        if (element.AssetAttribute.Configuration.DataTypeEnum == AppFramework.ConstantsEnumerators.Enumerators.DataType.Place)
                        {
                            queryString += element.Text;
                        }
                        else
                        {
                            queryString += element.AssetAttribute.Value;
                        }
                    }
                    else if (@operator.IsAssetListDropDown || @operator.IsDynListDropDown)
                    {
                        queryString += CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].Items[element.ItemId].Key.ToString();
                    }
                    else
                    {
                        queryString += element.Text;
                    }

                    queryString += element.EndBrackets;
                    if (i != elements.Count() - 1)
                    {
                        queryString += element.AndOr == 0 ? " and " : " or ";
                    }
                    i++;
                }
            }
            return queryString;
        }
    }
}
