using AppFramework.Core.DataTypes;

namespace AppFramework.Core.Classes.SearchEngine.ContextSearchElements
{
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
    using AppFramework.DataProxy;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ContextItem
    {
        public long Value { get; set; }
        public string Text { get; set; }
        public long DataTypeUid { get; set; }

        public Enumerators.DataType DataTypeEnum
        {
            get
            {
                return _dataType.Code;
            }
        }

        public List<Operator> Operators
        {
            get
            {
                if (_operators == null)
                    LoadOperators();
                return _operators;
            }
        }

        private List<Operator> _operators = null;
        private readonly CustomDataType _dataType;
        private readonly IUnitOfWork _unitOfWork;

        public ContextItem(CustomDataType dataType, IUnitOfWork unitOfWork)
        {
            if (dataType == null)
                throw new ArgumentNullException("CustomDataType");
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            _dataType = dataType;
            _unitOfWork = unitOfWork;
        }

        private void LoadOperators()
        {
            var datatype = _unitOfWork.DataTypeRepository.Single(d => d.DataTypeUid == DataTypeUid, include: dt => dt.SearchOperators);
            _operators = (from so in datatype.SearchOperators
                          select new Operator(so)).ToList();
        }
    }
}
