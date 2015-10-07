namespace AppFramework.Core.Classes
{
    using System.Collections.Generic;
    using System.Linq;

    public class MeasureUnit
    {
        public MeasureUnit()
        {
        }

        public MeasureUnit(Entities.MeasureUnit mu)
            : this()
        {
            UID = mu.MeasureUnitUid;
            Name = mu.Name;
            Description = mu.Description;
            Comment = mu.Comment;
        }

        public long UID
        {
            private set;
            get;
        }

        public string Name
        {
            private set;
            get;
        }

        public string Description
        {
            get;
            private set;
        }

        public string Comment
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all MeasurementUnits
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MeasureUnit> GetAll()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            return
                unitOfWork.MeasureUnitRepository.Get()
                    .Select(mu => new MeasureUnit(mu));
        }

        public static MeasureUnit GetByUid(long uid)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            Entities.MeasureUnit mu = unitOfWork.MeasureUnitRepository.Single(m => m.MeasureUnitUid == uid);
            return new MeasureUnit(mu);
        }
    }
}
