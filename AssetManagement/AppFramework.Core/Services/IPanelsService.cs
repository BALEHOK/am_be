using System.Collections.Generic;
using AppFramework.Entities;

namespace AppFramework.Core.Services
{
    public interface IPanelsService
    {
        AttributePanel GetByUid(long panelUid);
        AttributePanel GetById(long panelId);
        void Delete(long panelUid);
        void Save(AttributePanel panel);
        List<AttributePanel> GetAllByScreenId(long screenId);
    }
}