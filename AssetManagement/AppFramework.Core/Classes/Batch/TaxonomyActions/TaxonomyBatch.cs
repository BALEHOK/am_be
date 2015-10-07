using System;
using AppFramework.Core.Interceptors;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.Batch.TaxonomyActions
{
    /// <summary>
    /// MoveToHistory action get two parameters in Params dictionary - FromUid and ToUid. Both parameters is Taxonomy UIDs.
    /// This action set first taxonomy (with FromUid) inactive, remove Category flag and move all related assets to history table. Second taxonomy
    /// (with UID equal to ToUid) set as Active Version and category flag - as first category.
    /// </summary>
    public class TaxonomyBatch : BatchAction
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaxonomyBatch(Entities.BatchAction batchAction, IUnitOfWork unitOfWork)
            : base(batchAction)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        [Transaction]
        public override void Run()
        {
            var fromUid = long.Parse(Parameters["FromUid"]);
            var toUid = long.Parse(Parameters["ToUid"]);

            var unitOfWork = _unitOfWork;
            var sourceTaxonomy = unitOfWork.TaxonomyRepository.SingleWithAggregations(t => t.TaxonomyUid == fromUid);
            var targetTaxonomy = unitOfWork.TaxonomyRepository.SingleWithAggregations(t => t.TaxonomyUid == toUid);
            sourceTaxonomy.ActiveVersion = false;
            targetTaxonomy.ActiveVersion = true;
            sourceTaxonomy.IsDraft = false;
            targetTaxonomy.IsDraft = false;
            targetTaxonomy.IsCategory = sourceTaxonomy.IsCategory;
            sourceTaxonomy.TaxonomyItem.ForEachWithIndex((ti, idx) => ti.ActiveVersion = false);
            targetTaxonomy.TaxonomyItem.ForEachWithIndex((ti, idx) => ti.ActiveVersion = true);

            unitOfWork.TaxonomyRepository.Update(targetTaxonomy);
            unitOfWork.TaxonomyRepository.Update(sourceTaxonomy);
            unitOfWork.Commit();
        }
    }
}
