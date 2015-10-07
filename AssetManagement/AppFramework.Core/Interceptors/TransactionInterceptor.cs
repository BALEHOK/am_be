using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using AppFramework.DataProxy;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace AppFramework.Core.Interceptors
{
    public class TransactionInterceptor : IInterceptionBehavior
    {
        public TransactionInterceptor()
        {
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute
        {
            get { return true; }
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input.MethodBase.GetCustomAttributes(true).Any(
                a => a.GetType() == typeof (TransactionAttribute)))
            {
                using (var scope = new TransactionScope())
                {
                    var result = getNext().Invoke(input, getNext);
                    if (result.Exception == null)
                        scope.Complete();
                    return result;
                }
            }
            return getNext()(input, getNext);
        }
    }
}
