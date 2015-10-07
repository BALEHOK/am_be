using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.Validation;
using AppFramework.Core.Classes.Validation.Expression;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DataTypes;
using AppFramework.Core.Interceptors;
using AppFramework.Core.Properties;
using AppFramework.Core.Validation;
using AppFramework.DataProxy;
using AppFramework.Entities;
using System.Text.RegularExpressions;

namespace AppFramework.Core.Classes
{
    public interface IValidationService
    {
        ValidationResult ValidateAttribute(AssetAttribute attribute, long currentUserId);
        ValidationResult ValidateDataType(DataTypeBase dataType, string value);
        ValidationResult ValidateFileUpload(string fileName, Enumerators.MediaType fileType);
        void SaveDataTypeValidationRule(DataTypeValidationRule validationRule);
        void DeleteDataTypeValidationRule(DataTypeValidationRule validationRule);
        void AddAttributeValidationRule(AttributeValidationRule rule);
        void DeleteAttributeValidationRule(AttributeValidationRule validationRule);
    }

    public class ValidationService : IValidationService
    {
        private readonly IValidationRulesService _validationRulesService;
        private readonly IUnitOfWork _unitOfWork;

        public ValidationService(
            IUnitOfWork unitOfWork,
            IValidationRulesService validationRulesService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (validationRulesService == null)
                throw new ArgumentNullException("validationRulesService");
            _validationRulesService = validationRulesService;
        }

        /// <summary>
        /// Validates this attribute. Validation include checks for data type and asset type attribute
        /// </summary>
        /// <returns></returns>
        public ValidationResult ValidateAttribute(AssetAttribute attribute, long currentUserId)
        {
            if (attribute == null)
                throw new ArgumentNullException("AssetAttribute");
            var configuration = attribute.Configuration;

            var value = attribute.Value;
            if (configuration.DataTypeEnum == Enumerators.DataType.Asset)
            {
                value = attribute.ValueAsId.ToString();

                // 20.7.2011 Set User and Owner to current authenticated user
                if ((configuration.DBTableFieldName == AttributeNames.UserId
                    || configuration.DBTableFieldName == AttributeNames.OwnerId)
                    && value == "0")
                {
                    attribute.ValueAsId = currentUserId;
                    value = attribute.ValueAsId.ToString();
                }
            }
            else if (attribute.IsDynamicList)
            {
                value = string.Join(",", attribute.DynamicListValues);
            }
            else if (configuration.DataTypeEnum == Enumerators.DataType.Assets)
            {
                // string of comma-separated IDs of assets
                value = attribute.MultipleAssets.Count > 0
                    ? string.Join(",", attribute
                        .MultipleAssets
                        .ToList()
                        .Select(m => m.Key.ToString())
                        .ToArray())
                    : string.Empty;
            }

            attribute.ValidationResults = attribute.ValidationResults ?? new List<ValidationResult>(2);

            // validate with attribute rules
            var attributeValidation = _validateAttribute(value, attribute);
            attribute.ValidationResults.Add(attributeValidation);

            // if first validation passed and there's some data filled, validate with datatype rules
            if (attributeValidation.IsValid && !string.IsNullOrEmpty(value))
            {
                var dataTypeValidation = ValidateDataType(configuration.DataType, value);
                attribute.ValidationResults.Add(dataTypeValidation);
            }

            var result = new ValidationResult();

            if (!result.IsValid)
            {
                if (!string.IsNullOrEmpty(configuration.ValidationMessage))
                {
                    var line = new ValidationResultLine(string.Empty) { IsValid = false };
                    if (configuration.ValidationMessage.Contains("{0}"))
                    {
                        line.Message = string.Format(configuration.ValidationMessage, value);
                    }
                    else
                    {
                        line.Message = configuration.ValidationMessage;
                    }
                    result.ResultLines.Add(line);
                }
                else
                {
                    result.ResultLines.AddRange(
                        attribute.ValidationResults.Where(v => v.IsValid == false).SelectMany(v => v.ResultLines));
                }
            }
            return result;
        }

        public ValidationResult ValidateDataType(DataTypeBase dataType, string value)
        {
            var res = new ValidationResult();
            var rules = _validationRulesService.GetValidationRulesForDataType(dataType).ToList();
            if (rules.Any() && !string.IsNullOrEmpty(dataType.ValidationExpr))
            {
                var val = new DataTypeExprValidator(dataType.ValidationExpr, dataType, rules);
                res = val.Validate(value);

                if (!res.IsValid && !string.IsNullOrEmpty(dataType.ValidationMessage))
                {
                    // for the new logic of validation work we will not return all result lines, instead just return single formatted message
                    res.ResultLines.Clear();
                    res.ResultLines.Add(new ValidationResultLine(string.Empty)
                    {
                        IsValid = false,
                        Message = String.Format(dataType.ValidationMessage ?? "Incorrect value - {0}", value)
                    });
                }
            }
            return res;
        }

        private ValidationResult _validateAttribute(string value, AssetAttribute attribute)
        {
            var configuration = attribute.Configuration;

            var result = new ValidationResult();
            bool isRequiredPerformed = !(configuration.IsRequired && string.IsNullOrEmpty(value));

            if (configuration.DataTypeEnum == Enumerators.DataType.Asset)
            {
                long id;
                long.TryParse(value, out id);
                isRequiredPerformed &= !(configuration.IsRequired && id == 0);
            }

            if (isRequiredPerformed)
            {
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(configuration.ValidationExpr))
                {
                    var rules = _validationRulesService.GetValidationRulesForAttribute(configuration).ToList();
                    var validator = new AttributeExprValidator(configuration.ValidationExpr, attribute, rules);
                    result = validator.Validate(value);
                }
            }
            else
            {
                result = new ValidationResult();
                result.ResultLines.Add(new ValidationResultLine(string.Empty)
                {
                    IsValid = false,
                    Message = string.Format("{0}", Resources.RequiredText)
                });
            }
            return result;
        }

        [Transaction]
        public void SaveDataTypeValidationRule(DataTypeValidationRule validationRule)
        {
            var vl = new ValidationList()
            {
                ValidationOperatorUid = validationRule.ValidationOperator.UID,
                Name = validationRule.Name,
                ValidationUid = validationRule.UID
            };

            if (validationRule.UID == 0)
            {
                _unitOfWork.ValidationListRepository.Insert(vl);
            }
            else
            {
                _unitOfWork.ValidationListRepository.Update(vl);
            }

            var datatype = _unitOfWork.DataTypeRepository.Single(d => d.DataTypeUid == validationRule.DataTypeUid);
            datatype.ValidationList.Add(vl);

            foreach (var op in validationRule.ValidationOperator.Operands)
            {
                _unitOfWork.ValidationOperandValueRepository.Insert(new ValidationOperandValue()
                {
                    Value = op.Value.ToString(),
                    ValidationListUid = vl.ValidationUid,
                    ValidationOperandUid = op.UID,
                });
            }
            _unitOfWork.Commit();
        }

        [Transaction]
        public void DeleteDataTypeValidationRule(DataTypeValidationRule validationRule)
        {
            _unitOfWork.SqlProvider.ExecuteNonQuery("DELETE FROM DataTypeValidation WHERE ValidationUid=" + validationRule.UID);
            _unitOfWork.SqlProvider.ExecuteNonQuery("DELETE FROM ValidationOperandValue WHERE ValidationListUid=" + validationRule.UID);
            _unitOfWork.SqlProvider.ExecuteNonQuery("DELETE FROM ValidationList WHERE ValidationUid=" + validationRule.UID);
            _unitOfWork.Commit();
        }

        [Transaction]
        public void AddAttributeValidationRule(AttributeValidationRule rule)
        {
            var attribValidation = new DynEntityAttribValidation
            {
                ValidationList = rule.ValidationList,
                DynEntityAttribConfigId = rule.AssetTypeAttributeId
            };            
            
            _unitOfWork.DynEntityAttribValidationRepository.Insert(attribValidation);
            _unitOfWork.ValidationListRepository.Insert(rule.ValidationList);
            _unitOfWork.Commit();
        }

        [Transaction]
        public void DeleteAttributeValidationRule(AttributeValidationRule validationRule)
        {
            var vl = _unitOfWork.ValidationListRepository.Single(v => v.ValidationUid == validationRule.UID);
            var deav = _unitOfWork.DynEntityAttribValidationRepository
                .Single(d => d.DynEntityAttribConfigId == validationRule.AssetTypeAttributeId &&
                d.ValidationUid == validationRule.UID);
            if (deav != null)
                _unitOfWork.DynEntityAttribValidationRepository.Delete(deav);
            if (vl != null)
                _unitOfWork.ValidationListRepository.Delete(vl);
            _unitOfWork.Commit();
        }

        public ValidationResult ValidateFileUpload(string fileName, Enumerators.MediaType fileType)
        {
            var result = new ValidationResult();
            var allowedImageExtensions = new []{ "jpg", "jpeg", "gif", "bmp", "png" };
            var imageRegex = string.Format(@"(?i:^.*\.({0})$)", 
                string.Join("|", allowedImageExtensions));
            if (fileType == Enumerators.MediaType.Image)
            {
                var match = Regex.Match(
                    fileName,
                    imageRegex);

                if (!match.Success)
                    result.AddErrorFormatted(
                        "{0} is not a valid image", fileName);
            }
            return result;
        }
    }
}
