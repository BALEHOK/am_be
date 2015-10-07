using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Classes.Validation;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes
{
    public interface IValidationRulesService
    {
        IEnumerable<AttributeValidationRule> GetValidationRulesForAttribute(AssetTypeAttribute attribute);
        IEnumerable<DataTypeValidationRule> GetValidationRulesForDataType(DataTypeBase dataType);
    }

    public class ValidationRulesService : IValidationRulesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationOperatorFactory _validationOperatorFactory;

        public ValidationRulesService(
            IUnitOfWork unitOfWork, 
            IValidationOperatorFactory validationOperatorFactory)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (validationOperatorFactory == null)
                throw new ArgumentNullException("validationOperatorFactory");
            _validationOperatorFactory = validationOperatorFactory;
        }

        public IEnumerable<DataTypeValidationRule> GetValidationRulesForDataType(DataTypeBase dataType)
        {
            var validationRules = dataType.Base.ValidationList
                .Select(vl => new DataTypeValidationRule(vl, 
                    dataType.Base.DataTypeUid, 
                    _validationOperatorFactory.Get(vl.ValidationOperator)))
                .ToList();
            validationRules.ForEach(r => r.LoadValues());
            return validationRules;
        }

        public IEnumerable<AttributeValidationRule> GetValidationRulesForAttribute(AssetTypeAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("AssetTypeAttribute");
            var validationRules = new List<AttributeValidationRule>();
            if (attribute.Base.DynEntityAttribConfigId > 0)
            {
                // Load validation rules from DynEntityAttribValidation table (many-to-many)
                var validationLists = _unitOfWork.DynEntityAttribValidationRepository
                                                .Get(
                                                    deav =>
                                                    deav.DynEntityAttribConfigId == attribute.Base.DynEntityAttribConfigId,
                                                    include: deav => deav.ValidationList)
                                                .Select(av => av.ValidationList)
                                                .ToList();
                
                validationRules = (from vl in validationLists
                                   let op = _validationOperatorFactory.GetByUid(vl.ValidationOperatorUid)
                                   select new AttributeValidationRule(vl,
                                       attribute.Base.DynEntityAttribConfigId, 0, op))
                    .ToList();

                // Load validation rule which is pre-defined for this configuration with ValidationExpr column value.
                if (!string.IsNullOrEmpty(attribute.ValidationExpr))
                {
                    var validationRuleName = attribute.ValidationExpr.TrimStart('@');
                    var inlineConfigValidations =
                        _unitOfWork.ValidationListRepository.Get(l => l.Name == validationRuleName).ToList();
                    validationRules.AddRange(
                        from inlineValidator in inlineConfigValidations
                        let op = _validationOperatorFactory.GetByUid(inlineValidator.ValidationOperatorUid)
                        select new AttributeValidationRule(inlineValidator, attribute.ID, 0, op));
                }

                foreach (var item in validationRules)
                {
                    item.ValidationOperator.AssetTypeAttribute = attribute;
                    item.LoadValues();
                }
            }
            return validationRules;
        }
    }
}
