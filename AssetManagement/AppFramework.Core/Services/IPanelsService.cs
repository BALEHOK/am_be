using System.Collections.Generic;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.ScreensServices
{
    public interface IPanelsService
    {
        List<Panel> GetAllByScreenId(long screenId);
        Panel GetByUid(long panelUid);
        void Delete(long panelUid);
        void Save(AttributePanel panel);
    }
}