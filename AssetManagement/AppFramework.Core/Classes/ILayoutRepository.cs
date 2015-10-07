using System.Collections.Generic;
using AppFramework.Core.Classes.ScreensServices;

namespace AppFramework.Core.Classes
{
    public interface ILayoutRepository
    {
        /// <summary>
        /// Returns screen layout by its DB ID
        /// </summary>
        /// <param name="id">ID of layout</param>
        /// <returns>Layout object</returns>
        Layout GetById(int id);

        /// <summary>
        /// Returns the list of all available screen layouts
        /// </summary>
        IEnumerable<Layout> GetAll();

        /// <summary>
        /// Returns the layout by its type
        /// </summary>
        /// <param name="type"></param>
        Layout GetByType(LayoutType type);
    }
}