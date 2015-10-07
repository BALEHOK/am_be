using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.AC.Authentication;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    public class TaxonomyService : ITaxonomyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaxonomyService(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public Taxonomy GetByUid(long uid)
        {
            var data = _unitOfWork.TaxonomyRepository.SingleWithAggregations(t => t.TaxonomyUid == uid);
            return new Taxonomy(data, from taxItem in data.TaxonomyItem
                                      select new TaxonomyItem(taxItem));
        }

        public void Save(Entities.Taxonomy taxonomy, long currentUserId)
        {
            _activeVersionFixup();

            var updateDate = DateTime.Now;
            if (taxonomy.ChangeTracker.State == ObjectState.Added)
            {
                taxonomy.UpdateDate = updateDate;
                taxonomy.UpdateUserId = currentUserId;
                _unitOfWork.TaxonomyRepository.Insert(taxonomy);
            }
            else
            {
                var revTax = new Entities.Taxonomy()
                {
                    Revision = taxonomy.Revision + 1,
                    ActiveVersion = false,
                    IsDraft = true,
                    Description = taxonomy.Description,
                    IsActive = taxonomy.IsActive,
                    IsCategory = taxonomy.IsCategory,
                    Name = taxonomy.Name,
                    NameTranslationId = taxonomy.NameTranslationId,
                    TaxonomyId = taxonomy.TaxonomyId,
                    UpdateDate = updateDate,
                    UpdateUserId = currentUserId,
                };

                var taxonomyItems = _copyTaxonomyItems(
                    taxonomy.TaxonomyItem.Where(ti => ti.ParentTaxonomyItemUid.HasValue == false), 
                    currentUserId, 
                    updateDate).ToList();
                _linkToTaxonomy(revTax, taxonomyItems);
                _unitOfWork.TaxonomyRepository.Insert(revTax);

                // ??? what's this ?
                // to prevent updating TaxonomyItems as well
                using (var uof = new UnitOfWork())
                {
                    var entity = uof.TaxonomyRepository.Single(t => t.TaxonomyId == taxonomy.TaxonomyId && t.ActiveVersion);
                    entity.IsDraft = true;
                    entity.UpdateDate = updateDate;
                    entity.UpdateUserId = currentUserId;
                    uof.TaxonomyRepository.Update(entity);
                    uof.Commit();
                }
            }
            _unitOfWork.Commit();
        }

        /// <summary>
        /// Returns major edited version of taxonomy by given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Taxonomy GetLastById(long id)
        {
            var data = _unitOfWork.TaxonomyRepository.GetWithAggregations(
                t => t.TaxonomyId == id,
                taxs => taxs.OrderByDescending(t => t.Revision))
                .FirstOrDefault();
            return data != null
                ? new Taxonomy(data, from taxItem in data.TaxonomyItem
                    select new TaxonomyItem(taxItem))
                : null;
        }

        /// <summary>
        /// Gets the active version of taxonomy
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Taxonomy GetById(long id)
        {
            var data = _unitOfWork.TaxonomyRepository
                .SingleOrDefaultWithAggregations(t => t.ActiveVersion && t.TaxonomyId == id);
            return data != null
                ? new Taxonomy(data, from taxItem in data.TaxonomyItem
                                     select new TaxonomyItem(taxItem))
                : null;
        }

        /// <summary>
        /// Gets all taxonomies / categories in system
        /// </summary>      
        /// <returns></returns>
        public IEnumerable<Taxonomy> GetAll()
        {
            return _unitOfWork.TaxonomyRepository.Get(
                t => t.ActiveVersion && t.IsActive,
                include: t => t.TaxonomyItem)
                .OrderBy(t => t.Name)
                .Select(item => new Taxonomy(item, from taxItem in item.TaxonomyItem
                    select new TaxonomyItem(taxItem)));

        }

        /// <summary>
        /// Deletes the current taxonomy by making it inactive
        /// </summary>
        public void Delete(Entities.Taxonomy taxonomy)
        {
            taxonomy.IsActive = false;
            _unitOfWork.TaxonomyRepository.Update(taxonomy);
            _unitOfWork.Commit();
        }

        public void SetCategoryByUid(long uid)
        {
            var toUnset = _unitOfWork.TaxonomyRepository.Get(t => t.IsCategory).ToList();
            toUnset.ForEach(t =>
            {
                t.IsCategory = false;
                _unitOfWork.TaxonomyRepository.Update(t);
            });
            var toSet = _unitOfWork.TaxonomyRepository.Single(t => t.TaxonomyUid == uid);
            toSet.IsCategory = true;
            _unitOfWork.TaxonomyRepository.Update(toSet);
            _unitOfWork.Commit();
        }



        /// <summary>
        /// Gets the taxonomy marked as Category
        /// </summary>
        /// <returns>Category</returns>
        public Taxonomy GetCategory()
        {
            var entity = _unitOfWork.TaxonomyRepository
                .SingleOrDefaultWithAggregations(t => t.IsCategory && t.IsActive && t.ActiveVersion);
            return entity != null
                ? new Taxonomy(entity, from taxItem in entity.TaxonomyItem
                    select new TaxonomyItem(taxItem))
                : null;
        }

        /// <summary>
        /// Normally this method should do nothing, but if there's any integrity problems with multiple active versions,
        /// this will fix an issue.
        /// </summary>
        private void _activeVersionFixup()
        {
            var taxonomies = _unitOfWork.TaxonomyRepository.AsQueryable();
            foreach (var entry in from taxonomy in taxonomies
                                  where taxonomy.IsActive
                                  group taxonomy by taxonomy.TaxonomyId into grouping
                                  where grouping.Count() > 1
                                  select grouping)
            {
                var tofix = (from taxonomy in taxonomies.OrderBy(t => t.TaxonomyUid)
                             where taxonomy.TaxonomyId == entry.Key && taxonomy.ActiveVersion
                             select taxonomy).ToList();
                for (int i = 0; i < tofix.Count - 1; i++)
                {
                    tofix[i].ActiveVersion = false;
                    _unitOfWork.TaxonomyRepository.Update(tofix[i]);
                }
                _unitOfWork.Commit();
            }
        }

        private void _linkToTaxonomy(Entities.Taxonomy revTax, IEnumerable<Entities.TaxonomyItem> taxonomyItems)
        {
            foreach (var ti in taxonomyItems)
            {
                ti.Taxonomy = revTax;
                _linkToTaxonomy(revTax, ti.ChildItems);
            }
        }

        private IEnumerable<Entities.TaxonomyItem> _copyTaxonomyItems(IEnumerable<Entities.TaxonomyItem> source, long updateUserId, DateTime updateDate)
        {
            foreach (var ti in source)
            {
                var taxonomyItem = new Entities.TaxonomyItem()
                {
                    ActiveVersion = false,
                    TaxonomyItemId = ti.TaxonomyItemId,
                    Name = ti.Name,
                    NameTranslationId = ti.NameTranslationId,
                    DisplayOrder = ti.DisplayOrder,
                    Number = ti.Number,
                    DynEntityId = ti.DynEntityId,
                    ImageName = ti.ImageName,
                    Description = ti.Description,
                    Comment = ti.Comment,
                    UpdateUserId = updateUserId,
                    UpdateDate = updateDate,
                    //ParentTaxonomyItemUid = ti.ParentTaxonomyItemUid,
                };
                _copyTaxonomyItems(ti.ChildItems, updateUserId, updateDate)
                    .ToList()
                    .ForEach(item => taxonomyItem.ChildItems.Add(item));
                yield return taxonomyItem;
            }
        }
    }
}
