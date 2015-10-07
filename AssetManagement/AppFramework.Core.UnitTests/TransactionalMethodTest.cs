using AppFramework.Core.Interceptors;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace AppFramework.Core.UnitTests
{
    public abstract class TransactionalMethodTest<T>
        where T: class
    {
        public TransactionalMethodTest()
        {

        }

        public bool RunInTransaction(string methodName, Action methodBody)
        {
            var transactionAttribute = typeof(T)
               .GetMethod(methodName)
               .GetCustomAttributes(typeof(TransactionAttribute), false)
               .SingleOrDefault();

            if (transactionAttribute == null)
                throw new Exception("This method doesn't marked as transactional");

            bool completed = false;
            using (var scope = new TransactionScope())
            {
                try
                {
                    methodBody.Invoke();
                    scope.Complete();
                    completed = true;
                }
                catch (Exception)
                {
                    
                }
            }
            return completed;
        }
    }
}
