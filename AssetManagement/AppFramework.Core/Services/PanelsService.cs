using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.ScreensServices
{
    public class PanelsService : IPanelsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PanelsService(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public List<Panel> GetAllByScreenId(long screenId)
        {
            var screen = _unitOfWork.AssetTypeScreenRepository.Single(s => s.ScreenId == screenId);
            var panels = _unitOfWork.AttributePanelRepository
                .Where(
                    a => a.DynEntityConfigUId == screen.DynEntityConfigUid && a.ScreenId == screenId,                    
                    a => a.AttributePanelAttribute
                        .Select(ap => ap.DynEntityAttribConfig))
                .Select(panel => new Panel(panel, panel.AttributePanelAttribute
                    .OrderBy(apa => apa.DisplayOrder)
                    .Select(apa => new AssetTypeAttribute(apa.DynEntityAttribConfig, _unitOfWork))
                    .Where(a => a.IsActive)
                    .ToList()))
                .ToList();
            return panels;
        }

        public Panel GetByUid(long panelUid)
        {
            Panel result = null;
            var panel = _unitOfWork.AttributePanelRepository
                .SingleOrDefault(
                    p => p.AttributePanelUid == panelUid,
                    p => p.AttributePanelAttribute.Select(a => a.DynEntityAttribConfig));
            if (panel != null)
                result = new Panel(panel, panel.AttributePanelAttribute
                    .OrderBy(apa => apa.DisplayOrder)
                    .Select(apa => new AssetTypeAttribute(apa.DynEntityAttribConfig, _unitOfWork))
                    .Where(a => a.IsActive)
                    .ToList());
            return result;
        }

        public void Delete(long panelUid)
        {
            var panel = _unitOfWork.AttributePanelRepository
                .SingleOrDefault(p => p.AttributePanelUid == panelUid);

            if (panel != null)
            {
                var attributePanelAttributes =
                    _unitOfWork.AttributePanelAttributeRepository.Get(
                        apa => apa.AttributePanelUid == panel.AttributePanelUid).ToList();

                _unitOfWork.AttributePanelAttributeRepository.Delete(attributePanelAttributes);

                _unitOfWork.AttributePanelRepository.Delete(panel);
                _unitOfWork.Commit();
            }
        }

        public void Save(AttributePanel panel)
        {
            panel.UpdateDate = DateTime.Now;
            if (panel.AttributePanelId != 0)
            {
                _unitOfWork.AttributePanelRepository.Update(panel);
            }
            else
            {
                _unitOfWork.AttributePanelRepository.Insert(panel);
            }
            _unitOfWork.Commit();
        }
    }
}
