using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.Core.Services
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

        public List<AttributePanel> GetAllByScreenId(long screenId)
        {
            // order in the app
            return _unitOfWork.AttributePanelRepository
                .Where(
                    a => a.DynEntityConfigUId == a.AssetTypeScreen.DynEntityConfigUid && a.ScreenId == screenId,
                    a => a.AttributePanelAttribute.Select(ap => ap.DynEntityAttribConfig))
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.AttributePanelUid)
                .ToList();
        }

        public AttributePanel GetByUid(long panelUid)
        {
            return _unitOfWork.AttributePanelRepository
                .SingleOrDefault(
                    p => p.AttributePanelUid == panelUid,
                    p => p.AttributePanelAttribute.Select(a => a.DynEntityAttribConfig));
        }

        public AttributePanel GetById(long panelId)
        {
            return _unitOfWork.AttributePanelRepository
                .SingleOrDefault(
                    p => p.AttributePanelId == panelId
                        && p.DynEntityConfig.Active
                        && p.DynEntityConfig.ActiveVersion,
                    p => p.AttributePanelAttribute.Select(apa => apa.DynEntityAttribConfig));
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
