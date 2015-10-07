namespace AppFramework.Core.Classes.Validation
{
    public class ValidationOperand
    {
        public long UID { get { return _base.OperandUid; } }

        public string Alias { get { return _base.Alias; } }

        public long DataTypeUid { get { return _base.DataTypeUid; } }

        public object Value
        {
            get;
            set;
        }

        public Entities.ValidationOperand Base
        {
            get { return _base; }
        }
        private Entities.ValidationOperand _base;

        public ValidationOperand(Entities.ValidationOperand operand)
        {
            if (operand == null)
                throw new System.ArgumentNullException("ValidationOperand");
            _base = operand;
            Value = null;
        }

        public void LoadValue(long _validationRuleUid)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var operandValue = unitOfWork.ValidationOperandValueRepository.SingleOrDefault(o => o.ValidationListUid == _validationRuleUid &&
                o.ValidationOperandUid == UID);
            this.Value = object.Equals(operandValue, null) ? null : operandValue.Value;
        }
    }
}
