using System.Collections.Generic;
using NCalc;

namespace AppFramework.Core.Calculation
{
    /// <summary>
    /// NCalc Expression extension methods
    /// </summary>
    public static class ExpressionExtender
    {
        public static IEnumerable<string> GetParameters(this Expression expression)
        {
            var finder = new ParametersFinder();
            expression.ParsedExpression.Accept(finder);
            return finder.Parameters;
        }
    }
}