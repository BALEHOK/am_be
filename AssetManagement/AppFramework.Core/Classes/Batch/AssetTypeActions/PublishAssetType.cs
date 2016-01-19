using System;
using AppFramework.Core.ConstantsEnumerators;
using System.Data.SqlClient;
using AppFramework.Core.Interceptors;

namespace AppFramework.Core.Classes.Batch.AssetTypeActions
{
    using DAL;
    using DAL.Adapters;
    using DataProxy;
    using LinqKit;
    using System.Collections.Generic;
    using System.Linq;

    public class PublishAssetType : BatchAction
    {
        private readonly IUnitOfWork _unitOfWork;

        public PublishAssetType(Entities.BatchAction batchAction, IUnitOfWork unitOfWork)
            : base(batchAction)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        [Transaction]
        public override void Run()
        {
            var fromAssetTypeUid = long.Parse(Parameters["FromAssetType"]);
            var toAssetTypeUid = long.Parse(Parameters["ToAssetType"]);

            var unitOfWork = _unitOfWork;
            var fromEntity = unitOfWork.DynEntityConfigRepository.Single(q => q.DynEntityConfigUid == fromAssetTypeUid);
            var toEntity = unitOfWork.DynEntityConfigRepository.Single(q => q.DynEntityConfigUid == toAssetTypeUid);

            unitOfWork.DynEntityConfigRepository.LoadProperty(fromEntity, e => e.DynEntityAttribConfigs);
            unitOfWork.DynEntityConfigRepository.LoadProperty(toEntity, e => e.DynEntityAttribConfigs);

            ChangeSchema(fromEntity, toEntity);

            fromEntity.DynEntityAttribConfigs.ForEach(a => a.ActiveVersion = false);
            fromEntity.ActiveVersion = false;
            fromEntity.IsUnpublished = false;
            unitOfWork.DynEntityConfigRepository.Update(fromEntity);

            toEntity.DynEntityAttribConfigs.ForEach(a => a.ActiveVersion = true);
            toEntity.ActiveVersion = true;
            toEntity.IsUnpublished = false;
            unitOfWork.DynEntityConfigRepository.Update(toEntity);

            UpdateConnectedPanelsRelations(fromEntity, toEntity);

            unitOfWork.RebuildTriggers(toAssetTypeUid);
            unitOfWork.Commit();
        }

        private void ChangeSchema(Entities.DynEntityConfig fromEntity, Entities.DynEntityConfig toEntity)
        {
            var adapter = new DynColumnAdapter(new DataTypeService(_unitOfWork));
            // proccess new and changed columns
            foreach (var attribute in toEntity.DynEntityAttribConfigs)
            {
                var newColumn = adapter.ConvertDynEntityAttribConfigToDynColumn(attribute);

                var prevAttr = fromEntity.DynEntityAttribConfigs
                    .SingleOrDefault(a => a.DynEntityAttribConfigId == attribute.DynEntityAttribConfigId);

                if (prevAttr == null)
                {
                    // add a new column if it's new attribute
                    attribute.DBTableFieldname = newColumn.Name;
                    DBHelper.AlterTable(toEntity.DynEntityConfigUid, newColumn, _unitOfWork);
                }
                else if (attribute.DataTypeUid != prevAttr.DataTypeUid
                         || attribute.IsRequired != prevAttr.IsRequired)
                {
                    DBHelper.AlterTable(toEntity.DynEntityConfigUid, newColumn, _unitOfWork);
                }
            }

            // process removed columns
            foreach (
                var removedAttribute in
                    fromEntity.DynEntityAttribConfigs.Where(
                        a =>
                            toEntity.DynEntityAttribConfigs.All(
                                newatt => newatt.DynEntityAttribConfigId != a.DynEntityAttribConfigId)))
            {
                _unitOfWork.SqlProvider.ExecuteNonQuery(StoredProcedures.DisableColumn, new[]
                {
                    new SqlParameter("@table_name", toEntity.DBTableName),
                    new SqlParameter("@column_name", removedAttribute.DBTableFieldname),
                    new SqlParameter("@DynEntityAttribConfigId", removedAttribute.DynEntityAttribConfigId)
                }, System.Data.CommandType.StoredProcedure);
            }
        }

        private void UpdateConnectedPanelsRelations(
            Entities.DynEntityConfig fromEntity,
            Entities.DynEntityConfig toEntity)
        {
            _unitOfWork.DynEntityConfigRepository.LoadProperty(fromEntity, e => e.AttributePanel);
            var panelsUidsToExclude = new List<long>(fromEntity.AttributePanel.Select(p => p.AttributePanelUid));
            var attributesUidsToInclude =
                new List<long>(fromEntity.DynEntityAttribConfigs.Select(a => a.DynEntityAttribConfigUid));

            var relationsSource = _unitOfWork.AttributePanelAttributeRepository.AsQueryable();
            var configsSource = _unitOfWork.DynEntityConfigRepository.AsQueryable();
            var attributesSource = _unitOfWork.DynEntityAttribConfigRepository.AsQueryable();

            var relationsToUpdate = (from relation in relationsSource
                                     from c in configsSource
                                     from a in attributesSource
                                     where attributesUidsToInclude.Contains(relation.DynEntityAttribConfigUId) &&
                                           !panelsUidsToExclude.Contains(relation.AttributePanelUid) &&
                                           relation.DynEntityAttribConfigUId == a.DynEntityAttribConfigUid &&
                                           a.DynEntityConfigUid == c.DynEntityConfigUid &&
                                           c.ActiveVersion
                                     select relation).ToList();

            foreach (var relation in relationsToUpdate)
            {
                var oldAttribute =
                    fromEntity.DynEntityAttribConfigs.Single(
                        a => a.DynEntityAttribConfigUid == relation.DynEntityAttribConfigUId);
                var newAttribute =
                    toEntity.DynEntityAttribConfigs.SingleOrDefault(
                        a => a.DynEntityAttribConfigId == oldAttribute.DynEntityAttribConfigId);
                if (newAttribute != null)
                {
                    relation.DynEntityAttribConfigUId = newAttribute.DynEntityAttribConfigUid;
                    _unitOfWork.AttributePanelAttributeRepository.Update(relation);
                }
            }

            // update screens
            var screens = _unitOfWork.AssetTypeScreenRepository
                .Where(s => s.DynEntityConfigUid == fromEntity.DynEntityConfigUid).ToList();
            screens.ForEach(s =>
            {
                // update type UID in existing screens
                s.DynEntityConfigUid = toEntity.DynEntityConfigUid;
                _unitOfWork.AssetTypeScreenRepository.Update(s);

                // remove old panels and attribute panels                    
                var oldPanels =
                    _unitOfWork.AttributePanelRepository.Where(
                        p => p.ScreenId == s.ScreenId && p.DynEntityConfigUId != toEntity.DynEntityConfigUid).ToList();
                var oldPanelsUids = oldPanels.Select(p => p.AttributePanelUid).ToList();

                var oldPanelAttributes =
                    _unitOfWork.AttributePanelAttributeRepository.Where(
                        apa => oldPanelsUids.Contains(apa.AttributePanelUid)).ToList();

                _unitOfWork.AttributePanelAttributeRepository.Delete(oldPanelAttributes);
                _unitOfWork.AttributePanelRepository.Delete(oldPanels);
            });
        }
    }
}