namespace AppFramework.Core.Classes
{
    using System.Collections.Generic;
    using System.Linq;

    public class ZipCode
    {
        private Entities.ZipCode _base;

        public long Id
        {
            get { return _base.ZipId; }
        }

        public string Code
        {
            get { return _base.Code; }
            set { _base.Code = value; }
        }

        public ZipCode()
        {
            _base = new Entities.ZipCode();
        }

        public ZipCode(Entities.ZipCode source)
        {
            _base = source;
        }

        public void Save()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            if (_base.ZipId > 0)
            {
                unitOfWork.ZipCodeRepository.Update(_base);
            }
            else
            {
                unitOfWork.ZipCodeRepository.Insert(_base);
            }
            unitOfWork.Commit();
        }

        public static void Delete(long id)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var zip = unitOfWork.ZipCodeRepository.Single(z => z.ZipId == id);
            unitOfWork.ZipCodeRepository.LoadProperty(zip, z => z.Place2Zip);
            unitOfWork.ZipCodeRepository.Delete(zip);
        }

        public static ZipCode GetById(long id)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            Entities.ZipCode zip = unitOfWork.ZipCodeRepository.Single(z => z.ZipId == id);
            return new ZipCode(zip);
        }

        public static IEnumerable<Place> GetPlaces(string zipCode)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            Entities.ZipCode zip = unitOfWork.ZipCodeRepository.Single(z => z.Code == zipCode);
            unitOfWork.ZipCodeRepository.LoadProperty(zip, z => z.Place2Zip);
            if (zip != null)
            {
                foreach (var item in zip.Place2Zip)
                {
                    yield return new Place(item.Place);
                }
            }
        }
    }
}
