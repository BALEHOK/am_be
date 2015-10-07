using System.Collections.Generic;
using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation.Operators
{
    public abstract class ValidationOperatorBase
    {
        protected long uid;
        protected string name;
        protected List<ValidationOperand> operands;
        private AssetTypeAttribute attribute;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public List<ValidationOperand> Operands
        {
            get
            {
                return operands;
            }
            set
            {
                operands = value;
            }
        }

        public long UID
        {
            get
            {
                return uid;
            }
            set
            {
                uid = value;
            }
        }

        public AssetTypeAttribute AssetTypeAttribute
        {
            get
            {
                return this.attribute;
            }
            set
            {
                this.attribute = value;
            }
        }

        public AssetAttribute Attribute { get; set; }

        public abstract ValidationResultLine Validate(string value);   
    }
}
