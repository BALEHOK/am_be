using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Calculation;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.ScreensServices
{
    public class ScreenFormulaAttributeModel
    {
        public long Uid { get; set; }
        public string Name { get; set; }
    }

    public class ScreenFormulaModel
    {
        public ScreenFormulaAttributeModel Attribute { get; set; }
        public string FormulaText { get; set; }
    }

    public class ScreensService : IScreensService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScreensService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<AssetTypeScreen> GetScreensByAssetTypeUid(long assetTypeUid)
        {
            var includes = new IncludesBuilder<AssetTypeScreen>(
                s => s.AttributePanel
                        .Select(p => p.AttributePanelAttribute
                        .Select(apa => apa.DynEntityAttribConfig)),
                s => s.ScreenLayout);

            // load the whole screen info including panels, panels attributes and atttributes configs
            var result = _unitOfWork.AssetTypeScreenRepository
                .Where(s => s.DynEntityConfigUid == assetTypeUid, includes.Get()).ToList();
            
            return result;
        }

        public AssetTypeScreen GetScreenById(long screenId)
        {
            var ib = new IncludesBuilder<AssetTypeScreen>(
                s => s.AttributePanel.
                    Select(a => a.AttributePanelAttribute
                        .Select(apa => apa.DynEntityAttribConfig)));
            var screen = _unitOfWork.AssetTypeScreenRepository
                .SingleOrDefault(s => s.ScreenId == screenId, ib.Get());
            return screen;
        }

        IQueryable<AttributePanel> GetAttributePanelsByScreenId(long screenId)
        {
            var result = _unitOfWork.AttributePanelRepository.Get(g => g.ScreenId == screenId,
                include: entity => entity.AttributePanelAttribute.Select(a => a.DynEntityAttribConfig));
            return result.AsQueryable();
        }

        List<AttributePanelAttribute> GetAttributePanelAttributesByScreenId(long screenId)
        {
            var attributePanels = GetAttributePanelsByScreenId(screenId);
            var attributePanelAttributes = attributePanels.SelectMany(p => p.AttributePanelAttribute).ToList();
            return attributePanelAttributes;
        }

        public List<ScreenFormulaAttributeModel> GetScreenFormulaAttributes(long screenId)
        {
            var attributes =
                GetAttributePanelAttributesByScreenId(screenId)
                    .DistinctBy(apa => apa.DynEntityAttribConfigUId)
                    .Where(apa => apa.DynEntityAttribConfig.AllowEditValue).ToList();

            // exclude attributes which have DB formula
            attributes =
                attributes.Where(a => string.IsNullOrWhiteSpace(a.DynEntityAttribConfig.CalculationFormula)).ToList();

            var result = attributes.Select(a => new ScreenFormulaAttributeModel
                {
                    Name = a.DynEntityAttribConfig.Name,
                    Uid = a.AttributePanelAttributeId
                }).ToList();

            return result;
        }    
        
        public void Delete(long screenId)
        {
            var screen = _unitOfWork.AssetTypeScreenRepository
                .SingleOrDefault(s => s.ScreenId == screenId);
            _unitOfWork.AssetTypeScreenRepository.Delete(screen);
            _unitOfWork.Commit();
        }

        public void Save(AssetTypeScreen screen)
        {
            if (screen.IsDefault)
            {
                var screens = 
                    (from s in _unitOfWork.AssetTypeScreenRepository.AsQueryable()
                    where s.DynEntityConfigUid == screen.DynEntityConfigUid && s.ScreenId != screen.ScreenId
                    select s).ToList();

                foreach (var s in screens)
                {
                    s.IsDefault = false;
                    _unitOfWork.AssetTypeScreenRepository.Update(screen);
                    _unitOfWork.Commit();
                }
            }


            if (screen.ScreenId == 0)
            {
                _unitOfWork.AssetTypeScreenRepository.Insert(screen);
            }
            else
            {
                _unitOfWork.AssetTypeScreenRepository.Update(screen);
            }
            _unitOfWork.Commit();
        }
    }
}