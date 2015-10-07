using System.Collections.Generic;

namespace AppFramework.Core.Classes.Validation
{
    public interface IValidationOperatorFactory
    {
        Operators.ValidationOperatorBase Get(Entities.ValidationOperator op);
        Operators.ValidationOperatorBase GetByUid(long uid);
        List<Operators.ValidationOperatorBase> GetAll();
    }
}