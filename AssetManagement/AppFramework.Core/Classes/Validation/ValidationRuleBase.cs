using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlInclude(typeof(AttributeValidationRule))]
    public abstract class ValidationRuleBase
    {
        public long UID
        {
            get;
            set;
        }

        [XmlIgnore]
        public virtual Operators.ValidationOperatorBase ValidationOperator
        {
            get;
            set;
        }

        public object Operands
        {
            get;
            set;
        }

        public long Priority
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the values for rule
        /// </summary>
        public void LoadValues()
        {
            foreach (var op in ValidationOperator.Operands)
            {
                op.LoadValue(UID);
            }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public ValidationResultLine Validate(string value)
        {
            ValidationResultLine ln;
            try
            {
                ln = ValidationOperator.Validate(value);
            }
            catch (Exception e)
            {
                ln = new ValidationResultLine(string.Empty)
                {
                    IsValid = false,
                    Message = string.Format("{0} throws exception {1}", this.Name, e.Message)
                };
            }
            return ln;
        }

        //public static ValidationRule GetByUid(long uid)
        //{
        //    var unitOfWork = new DataProxy.UnitOfWork();
        //    DynEntityAttribValidation deav = unitOfWork.DynEntityAttribValidationRepository.SingleOrDefault(v => v.ValidationUid == uid);
        //    ValidationRule res = null;
        //    if (!object.Equals(deav, null))
        //    {
        //        ValidationList vl = unitOfWork.ValidationListRepository.Single(v => v.ValidationUid == deav.ValidationUid);
        //        res = new ValidationRuleAttrib(vl, deav.DynEntityAttribconfigId, 0);
        //        (res as ValidationRuleAttrib).LoadValues();
        //        return res;
        //    }
        //    return res;
        //}
    }
}