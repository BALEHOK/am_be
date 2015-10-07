using System;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes
{
    public class FunctionEntry : ExpressionEntry
    {
        public FunctionEntry()
        {
            DisplayName = "Function";
            Suffix = "(";
            Postfix = ")";

            IsEditable = false;           
            CaptionColor = Brushes.Blue;
        }

        public FunctionEntry AddParameter(ExpressionEntry entry)
        {
            // avoid extending typed parameters
            entry.IsExtendable = entry.GetType() == typeof (ExpressionEntry);
            AddRightOperand(entry);

            return this;
        }
        
        public ExpressionEntry GetParameterType(ExpressionEntry entry)
        {
            var idx = AllOperands.IndexOf(entry);
            if (idx < 0)
                throw new ArgumentException("given entry not found in parameters list", "entry");

            return Type.AllOperands[idx];
        }

        public override string ToString()
        {
            var parameters = new StringBuilder();

            RightOperands.ForEach(p =>
            {
                var idx = AllOperands.IndexOf(p);
                var outerOpen = Type.AllOperands[idx].Open;
                var outerClose = Type.AllOperands[idx].Close;

                parameters.Append(Type.AllOperands[idx].GetType() == typeof(ExpressionEntry)
                    ? string.Format("{0}{1}{2}", outerOpen, p.ToString(), outerClose)
                    : p.ToString());

                if (p != RightOperands.Last())
                    parameters.Append(",");
            });

            var name = Selected != null ? Selected.Name : "";
            var open = Type.Open;
            var close = Type.Close;

            var result = string.Format("{0}{1}({2}){3}", open, name, parameters, close);
            return result;
        }
    }
}