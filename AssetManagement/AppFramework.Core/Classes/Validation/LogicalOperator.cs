using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.Validation
{
    public enum LogicalOperatorType
    {
        And = 1, Or, Xor, Not
    }

    public class LogicalOperator
    {
        public string Name
        {
            get;
            private set;
        }

        public LogicalOperatorType Type
        {
            get;
            private set;
        }

        public int TypeInt
        {
            get
            {
                return (int)Type;
            }
        }

        public string Expr
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the arityarity - num of operands
        /// </summary>
        /// <value>The arityarity.</value>
        public int Arityarity
        {
            get;
            private set;
        }

        public static LogicalOperator GetByType(LogicalOperatorType type)
        {
            LogicalOperator op = new LogicalOperator();

            switch (type)
            {
                case LogicalOperatorType.And:
                    op.Name = "And";
                    op.Expr = "AND";
                    op.Arityarity = 3;
                    break;
                case LogicalOperatorType.Or:
                    op.Name = "Or";
                    op.Expr = "OR";
                    op.Arityarity = 3;
                    break;
                case LogicalOperatorType.Xor:
                    op.Name = "Excluding or";
                    op.Expr = "XOR";
                    op.Arityarity = 2;
                    break;
                case LogicalOperatorType.Not:
                    op.Name = "Not";
                    op.Expr = "NOT";
                    op.Arityarity = 1;
                    break;
                default:
                    throw new Exception("Invalid logical operation type");
            }

            op.Type = type;

            return op;
        }

        public static List<LogicalOperator> GetAll()
        {
            List<LogicalOperator> ops = new List<LogicalOperator>();

            ops.Add(LogicalOperator.GetByType(LogicalOperatorType.And));
            ops.Add(LogicalOperator.GetByType(LogicalOperatorType.Or));
            ops.Add(LogicalOperator.GetByType(LogicalOperatorType.Xor));
            ops.Add(LogicalOperator.GetByType(LogicalOperatorType.Not));

            return ops;
        }
    }
}
