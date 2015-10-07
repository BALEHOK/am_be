using AppFramework.Core.Classes.Validation.Operators;

namespace AppFramework.Core.Classes.Validation
{
    using System;
    using AppFramework.Entities;

    [Serializable]
    public sealed class AttributeValidationRule : ValidationRuleBase
    {
        public Entities.ValidationList ValidationList { get; private set; }      

        public new string Name
        {
            get { return ValidationList.Name; }
            set { ValidationList.Name = value; }
        }

        public long AssetTypeAttributeId { get; set; }

        public override ValidationOperatorBase ValidationOperator
        {
            get { return _validationOperator; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("ValidationOperator");
                _validationOperator = value;
                ValidationList.ValidationOperatorUid = value.UID;
            }
        }
        private ValidationOperatorBase _validationOperator;

        public AttributeValidationRule(ValidationList vl, 
            long assetTypeAttributeId, 
            int priority, 
            ValidationOperatorBase validationOperator)
        {
            if (vl == null)
                throw new ArgumentNullException("ValidationList");
            if (validationOperator == null)
                throw new ArgumentNullException("validationOperator");
            ValidationList = vl;
            AssetTypeAttributeId = assetTypeAttributeId;
            Priority = priority;
            UID = ValidationList.ValidationUid;
            ValidationOperator = validationOperator;
        }
    }
}
