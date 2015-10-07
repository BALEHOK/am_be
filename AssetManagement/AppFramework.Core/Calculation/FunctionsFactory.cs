using System.Collections.Generic;
using NCalc;

namespace AppFramework.Core.Calculation
{
    public abstract class FunctionsFactory<T, T1>
    {
        protected delegate T FunctionDelegate(T1 typedParameter, params object[] parameters);
        protected readonly Dictionary<string, FunctionDelegate> Functions = new Dictionary<string, FunctionDelegate>();

        public T EvaluateFunction(string functionName, T1 typedParameter, FunctionArgs args)
        {
            var parameters = args.EvaluateParameters();

            FunctionDelegate function;
            Functions.TryGetValue(functionName.ToUpper(), out function);

            return function == null ? default(T) : function(typedParameter, parameters);
        }
    }
}