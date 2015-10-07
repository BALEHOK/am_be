using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes
{
    public class LayoutRepository : ILayoutRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public LayoutRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns screen layout by its DB ID
        /// </summary>
        /// <param name="id">ID of layout</param>
        /// <returns>Layout object</returns>
        public Layout GetById(int id)
        {
            return new Layout(_unitOfWork.ScreenLayoutRepository.Single(sl => sl.Id == id));
        }

        /// <summary>
        /// Returns the list of all available screen layouts
        /// </summary>
        public IEnumerable<Layout> GetAll()
        {
            return _unitOfWork.ScreenLayoutRepository.Get().Select(record => new Layout(record));
        }

        /// <summary>
        /// Returns the layout by its type
        /// </summary>
        /// <param name="type"></param>
        public Layout GetByType(LayoutType type)
        {
            var data = _unitOfWork.ScreenLayoutRepository.SingleOrDefault(sl => sl.Type == (int)type);
            return data != null ? new Layout(data) : null;
        }
    }
}