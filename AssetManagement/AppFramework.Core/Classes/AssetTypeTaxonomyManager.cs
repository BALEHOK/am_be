using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes
{
    public interface IAssetTypeTaxonomyManager
    {
        List<TaxonomyContainer> GetContainers(long assetTypeId);
        void SerializeContainers(long assetTypeId, List<TaxonomyContainer> containers);
        void PersistContainers(long assetTypeId, List<TaxonomyContainer> containers);
    }

    public class AssetTypeTaxonomyManager : IAssetTypeTaxonomyManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssetTypeTaxonomyManager(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public List<TaxonomyContainer> GetContainers(long assetTypeId)
        {
            var containers = new List<TaxonomyContainer>();
            var serializedTaxonomiesTreePath = Path.Combine(
                ApplicationSettings.TempFullPath, 
                string.Format("AssetTypeTaxonomies_{0}.xml", assetTypeId));
            if (File.Exists(serializedTaxonomiesTreePath))
            {
                // try load taxonomy from previously serialized data
                using (var fs = File.Open(serializedTaxonomiesTreePath, FileMode.Open))
                {
                    var xmlSerializer = new XmlSerializer(typeof(List<TaxonomyContainer>));
                    containers = xmlSerializer.Deserialize(fs) as List<TaxonomyContainer> ?? containers;
                }
            }
            return containers;
        }

        public void SerializeContainers(long assetTypeId, List<TaxonomyContainer> containers)
        {
            using (var fs = File.Open(
                Path.Combine(ApplicationSettings.TempFullPath, 
                string.Format("AssetTypeTaxonomies_{0}.xml", assetTypeId)), 
                FileMode.Create))
            {
                var xmlSerializer = new XmlSerializer(typeof(List<TaxonomyContainer>));
                xmlSerializer.Serialize(fs, containers);
            }
        }

        public void PersistContainers(long assetTypeId, List<TaxonomyContainer> containers)
        {
            foreach (var container in containers)
            {
                var oldItems = _unitOfWork.DynEntityConfigTaxonomyRepository
                    .Get(item => item.DynEntityConfigId == assetTypeId);
                foreach (var item in oldItems)
                {
                    _unitOfWork.DynEntityConfigTaxonomyRepository.Delete(item);
                }
                foreach (var item in container.AssignedTaxonomyItemsIds)
                {
                    _unitOfWork.DynEntityConfigTaxonomyRepository.Insert(new Entities.DynEntityConfigTaxonomy()
                    {
                        DynEntityConfigId = assetTypeId,
                        TaxonomyItemId = item
                    });
                }
            }
            _unitOfWork.Commit();
        }
    }

    public class TaxonomyContainer
    {
        public long TaxonomyUid { get; set; }
        public List<long> AssignedTaxonomyItemsIds { get; set; }

        public TaxonomyContainer()
        {
            AssignedTaxonomyItemsIds = new List<long>();
        }
    }
}
