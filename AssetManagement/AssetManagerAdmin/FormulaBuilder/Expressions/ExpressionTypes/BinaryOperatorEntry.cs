using System;
using System.Linq;

namespace AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes
{
    public class BinaryOperatorEntry : ExpressionEntry
    {
        public BinaryOperatorEntry()
        {
            DisplayName = "Operator";            
        }

        public override string ToString()
        {
            var leftOperand = LeftOperands.FirstOrDefault();
            var rightOperand = RightOperands.FirstOrDefault();

            var left = leftOperand != null ? leftOperand.ToString() : "";
            var right = rightOperand != null ? rightOperand.ToString() : "";

            var name = Selected != null ? Selected.Name : "";
            var result = string.Format("{0} {1} {2}", left, name, right);

            return result;
        }

        public override ExpressionEntry AddLeftOperand(ExpressionEntry operand)
        {
            return LeftOperands.FirstOrDefault() != null ? this : base.AddLeftOperand(operand);
        }

        public override ExpressionEntry AddRightOperand(ExpressionEntry operand, Type dependency = null)
        {
            return RightOperands.FirstOrDefault() != null ? this : base.AddRightOperand(operand);
        }

        public override ExpressionEntry ReplaceOperand(ExpressionEntry source, ExpressionEntry destination)
        {
            var idx = LeftOperandsList.IndexOf(source);
            var operandsList = LeftOperands;
            if (idx < 0)
            {
                idx = RightOperandsList.IndexOf(source);
                operandsList = RightOperands;
            }

            if (idx >= 0)
            {
                if (source.Parent != null && source.Parent != this)
                    source.Parent.ReplaceOperand(source, this);

                if (destination == null)
                {
                    operandsList.Remove(source);
                }
                else
                {
                    destination.Parent = this;
                    operandsList[idx] = destination;
                }
            }

            return this;
        }

        public override ExpressionEntry Connect(ExpressionEntry entry)
        {
            if (!IsRightConnectorSelected && !IsLeftConnectorSelected)
                IsRightConnectorSelected = true;

            if (entry is BinaryOperatorEntry)
                return base.Connect(entry);

            var place = IsRightConnectorSelected ? OperandPosition.Right : OperandPosition.Left;

            if (entry.Parent != null)
                entry.Parent.ReplaceOperand(entry, this);

            AddOperand(entry, place);

            return this;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            base.OnPropertyChanged(propertyName);

            if (propertyName != null &&
                (propertyName == "Selected" || propertyName == "Value" || propertyName == "IsEntrySelected"))
            {
                IsRightConnectorVisible &= !RightOperandsList.Any();
                IsLeftConnectorVisible &= !LeftOperandsList.Any();
            }
        }
    }
}