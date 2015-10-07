using AppFramework.Core.DataTypes;

namespace AppFramework.Core.Classes.SearchEngine.ContextSearchElements
{
    using AppFramework.DataProxy;
    using System;
    using System.Collections.Generic;

    public class ContextForSearch
    {
        public List<ContextItem> Attributes
        {
            get
            {
                if (_attributes == null)
                    LoadAttributres();
                return _attributes;
            }
        }

        private readonly List<ContextItem> _attributes = null;
        private readonly IUnitOfWork _unitOfWork;

        public ContextForSearch(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            _unitOfWork = unitOfWork;
        }

        private void LoadAttributres()
        {
            foreach (var contextItem in _unitOfWork.ContextRepository.Get())
                
            {
                var dataType = _unitOfWork.DataTypeRepository
                    .SingleOrDefault(d => d.DataTypeUid == contextItem.DataTypeUid);
                _attributes.Add(new ContextItem(new CustomDataType(dataType), _unitOfWork)
                {
                    Text = contextItem.Name,
                    Value = contextItem.ContextId
                });
            }
        }
    }
}