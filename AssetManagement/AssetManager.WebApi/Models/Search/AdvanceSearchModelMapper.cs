using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AssetManager.WebApi.Models.Search
{
    public class AdvanceSearchModelMapper : IAdvanceSearchModelMapper
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdvanceSearchModelMapper(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Maps AdvanceSearchModel to business acceptable collection of AttributeElements
        /// </summary>
        public List<AttributeElement> GetAttributeElements(AdvanceSearchModel searchModel)
        {
            if (searchModel.Attributes.Length == 0)
            {
                return new List<AttributeElement>(0);
            }

            var assetType = _unitOfWork.DynEntityConfigRepository
                .Single(c => c.DynEntityConfigId == searchModel.AssetTypeId && c.ActiveVersion,
                    c => c.DynEntityAttribConfigs.Select(ac => ac.DataType));

            var operators = _unitOfWork.SearchOperatorsRepository.Get().ToList();

            var attributeFilters = searchModel.Attributes;

            var attributeElements = new List<AttributeElement>();
            AttributeElement currentAttributeElement = null;

            var currentFilter = attributeFilters[0];
            for (var i = 0; i < attributeFilters.Length;)
            {
                currentAttributeElement = new AttributeElement();

                CollectOpenParentheses(attributeFilters, currentAttributeElement, ref currentFilter, ref i);

                if (currentFilter.Parenthesis == AttributeFilter.ParenthesisType.None)
                {
                    var attribute = assetType.DynEntityAttribConfigs.Single(
                        a => a.DynEntityAttribConfigId == currentFilter.ReferenceAttrib.Id && a.ActiveVersion);

                    SetAttributeElementProperties(attribute, operators, currentFilter, currentAttributeElement);

                    MoveForward(attributeFilters, ref currentFilter, ref i);
                }

                CollectClosingParentheses(attributeFilters, currentAttributeElement, ref currentFilter, ref i);

                attributeElements.Add(currentAttributeElement);
            }

            return attributeElements;
        }

        private static void CollectOpenParentheses(AttributeFilter[] attributeFilters,
            AttributeElement currentAttributeElement, ref AttributeFilter currentFilter, ref int i)
        {
            while (currentFilter.Parenthesis == AttributeFilter.ParenthesisType.Open)
            {
                currentAttributeElement.StartBrackets += "(";
                if (!MoveForward(attributeFilters, ref currentFilter, ref i))
                {
                    throw new Exception("Invalid search filter. Formula can't end by parenthesis.");
                }
            }
        }

        private static void CollectClosingParentheses(AttributeFilter[] attributeFilters,
            AttributeElement currentAttributeElement, ref AttributeFilter currentFilter, ref int i)
        {
            while (currentFilter.Parenthesis == AttributeFilter.ParenthesisType.Closing)
            {
                currentAttributeElement.EndBrackets += ")";
                if (currentFilter.LogicalOperator != AttributeFilter.LogicalOperators.None)
                {
                    SetLogicalOperator(currentFilter, currentAttributeElement);
                }

                if (!MoveForward(attributeFilters, ref currentFilter, ref i))
                {
                    break;
                }
            }
        }

        private static bool MoveForward(AttributeFilter[] attributeFilters, ref AttributeFilter currentFilter, ref int i)
        {
            if (++i < attributeFilters.Length)
            {
                currentFilter = attributeFilters[i];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Maps view model properties to business model
        /// </summary>
        /// <param name="attribute">Attribute config</param>
        /// <param name="operators"></param>
        /// <param name="currentFilter">Attribute filter view model</param>
        /// <param name="currentAttributeElement">Attribute filter business model</param>
        private void SetAttributeElementProperties(DynEntityAttribConfig attribute, List<SearchOperators> operators,
            AttributeFilter currentFilter, AttributeElement currentAttributeElement)
        {
            currentAttributeElement.FieldName = attribute.Name;
            currentAttributeElement.FieldSql = attribute.DBTableFieldname;
            SetLogicalOperator(currentFilter, currentAttributeElement);

            var oper = operators.Single(o => o.SearchOperatorUid == currentFilter.OperatorId);
            currentAttributeElement.ServiceMethod = oper.ServiceMethod;

            Enumerators.DataType attributeDataType;
            if (!Enum.TryParse(attribute.DataType.Name, true, out attributeDataType))
            {
                throw new Exception("Can't parse attribute type " + attribute.DataType.Name);
            }
            currentAttributeElement.ElementType = attributeDataType;

            if (currentFilter.Value == null || currentFilter.Value.Id == null)
            {
                currentAttributeElement.Value = string.Empty;
                return;
            }

            if (IsDateAttributeType(attributeDataType))
            {
                DateTime dt;

                // use ISO8601 DateTime format on client
                if (!DateTime.TryParse(currentFilter.Value.Id, out dt))
                {
                    currentAttributeElement.Value = string.Empty;
                    return;
                }

                if (Math.Abs(dt.TimeOfDay.TotalSeconds) < 0.1)
                {
                    currentAttributeElement.FieldSql = "CAST(" + currentAttributeElement.FieldSql + " AS DATE" + ")";
                    currentAttributeElement.Value = dt.ToShortDateString();
                }
                else
                {
                    currentAttributeElement.Value = dt.ToString();
                }
            }
            else
            {
                currentAttributeElement.Value = currentFilter.Value.Id;
            }
        }

        private static void SetLogicalOperator(AttributeFilter currentFilter, AttributeElement currentAttributeElement)
        {
            currentAttributeElement.ConcatenationOperation =
                currentFilter.LogicalOperator == AttributeFilter.LogicalOperators.And
                ? AppFramework.Core.Classes.SearchEngine.Enumerations.ConcatenationOperation.And
                : AppFramework.Core.Classes.SearchEngine.Enumerations.ConcatenationOperation.Or;
        }

        private static bool IsDateAttributeType(Enumerators.DataType attributeDataType)
        {
            return attributeDataType == Enumerators.DataType.CurrentDate
                   || attributeDataType == Enumerators.DataType.DateTime;
        }
    }
}