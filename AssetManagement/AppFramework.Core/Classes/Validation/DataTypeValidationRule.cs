using System;
using AppFramework.Core.Classes.Validation.Operators;

namespace AppFramework.Core.Classes.Validation
{
    using Entities;

    public sealed class DataTypeValidationRule : ValidationRuleBase
    {
        public long DataTypeUid
        {
            get;
            set;
        }

        public override ValidationOperatorBase ValidationOperator
        {
            get;
            set;
        }

        public DataTypeValidationRule(
            ValidationList vl, 
            long dataTypeUid, 
            ValidationOperatorBase validationOperator)
        {
            if (vl == null)
                throw new ArgumentNullException();
            if (validationOperator == null)
                throw new ArgumentNullException();
            Name = vl.Name;
            DataTypeUid = dataTypeUid;
            UID = vl.ValidationUid;
            ValidationOperator = validationOperator;
        }
    }
}
