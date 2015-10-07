using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.IE.IntegrityValidation
{
    public abstract class IntegrityValidatorBase
    {
        protected static Random rnd = new Random((int)DateTime.Now.Ticks);
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILayoutRepository _layoutRepository;

        public IntegrityValidatorBase(IUnitOfWork unitOfWork, ILayoutRepository layoutRepository)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (layoutRepository == null)
                throw new ArgumentNullException("layoutRepository");
            _layoutRepository = layoutRepository;
        }

        protected bool ValidateType(int typeId, out int correctedValue)
        {
            bool res = false;
            correctedValue = typeId;

            if (EntityType.GetByID(typeId) != null)
            {
                res = true;                
            }
            else
            {
                res = false;
                if (AllowErrorsCorrection)
                {
                    var et = EntityType.GetAll().FirstOrDefault();
                    if (et != null)
                    {
                        correctedValue = et.ID;
                    }
                }
            }              
            return res;
        }

        protected bool ValidateContext(long contextId, out long correctedValue)
        {
            bool res = false;
            correctedValue = contextId;

            var context = _unitOfWork.ContextRepository.SingleOrDefault(c => c.ContextId == contextId);
            if (context != null)
            {
                res = true;
            }
            else
            {
                res = false;
                if (AllowErrorsCorrection)
                {
                    var ec = _unitOfWork.ContextRepository.Get().FirstOrDefault();
                    if (ec != null)
                    {
                        correctedValue = ec.ContextId;
                    }
                }
            }
            return res;
        }

        protected bool ValidateLayout(int layoutId, out int correctedValue)
        {
            bool res = false;
            correctedValue = layoutId;

            if (_layoutRepository.GetById(layoutId) != null)
            {
                res = true;
            }
            else
            {
                res = false;
                if (AllowErrorsCorrection)
                {
                    Layout la = _layoutRepository.GetAll().FirstOrDefault();
                    if (la != null)
                    {
                        correctedValue = la.Id;
                    }
                }
            }
            return res;            
        }

        public bool AllowErrorsCorrection { get; set; }
    }
}
