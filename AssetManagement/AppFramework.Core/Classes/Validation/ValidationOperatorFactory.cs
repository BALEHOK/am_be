using AppFramework.Core.Classes.Barcode;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AppCore = AppFramework.Core.Classes.Validation;

    public class ValidationOperatorFactory : IValidationOperatorFactory
    {
        private IUnitOfWork _unitOfWork;
        private IBarcodeProvider _barcodeProvider;

        public ValidationOperatorFactory(IUnitOfWork unitOfWork, IBarcodeProvider barcodeProvider)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (barcodeProvider == null)
                throw new ArgumentNullException("barcodeProvider");
            _barcodeProvider = barcodeProvider;
        }

        public Operators.ValidationOperatorBase Get(Entities.ValidationOperator op)
        {
            if (op == null)
                throw new ArgumentNullException("ValidationOperator");
            var oper = GetOperatorByClassName(op.ClassName);
            oper.UID = op.ValidationOperatorUid;
            oper.Name = op.Name;
            oper.Operands = _unitOfWork.ValidationOperandRepository
                .Get(vond => vond.ValidationOperatorUid == op.ValidationOperatorUid)
                .Select(opn => new ValidationOperand(opn))
                .ToList();
            return oper;
        }

        public Operators.ValidationOperatorBase GetByUid(long uid)
        {
            Operators.ValidationOperatorBase oper = null;
            Entities.ValidationOperator op = _unitOfWork.ValidationOperatorRepository
                .Single(o => o.ValidationOperatorUid == uid);
            if (!Equals(op, null))
            {
                oper = Get(op);
            }
            return oper;
        }

        public List<Operators.ValidationOperatorBase> GetAll()
        {
            return _unitOfWork.ValidationOperatorRepository.Get()
                                                           .Select(Get)
                                                           .ToList();
        }

        private Operators.ValidationOperatorBase GetOperatorByClassName(string className)
        {
            Operators.ValidationOperatorBase op;
            Type opType = Type.GetType(string.Format("AppFramework.Core.Classes.Validation.{0}", className));

            if (Equals(opType, null))
            {
                throw new Exception(string.Format("Validation operator {0} not implemented", className));
            }

            if (opType.GetConstructors().Any(c => c.GetParameters().Any()))
            {
                op = Activator.CreateInstance(opType, _barcodeProvider) as Operators.ValidationOperatorBase;
            }
            else
            {
                op = Activator.CreateInstance(opType) as Operators.ValidationOperatorBase;   
            }

            if (Equals(op, null))
            {
                throw new Exception(string.Format("Validation operator {0} can not be created", className));
            }
            return op;
        }
    }
}
