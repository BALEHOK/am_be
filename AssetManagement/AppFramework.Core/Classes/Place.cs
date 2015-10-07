namespace AppFramework.Core.Classes
{
    using System.Collections.Generic;
    using System.Linq;

    public class Place
    {
        private Entities.Place _base;

        public string PlaceName
        {
            get { return _base.PlaceName; }
            set { _base.PlaceName = value; }
        }

        public long Id
        {
            get { return _base.PlaceId; }
        }

        public Place()
        {
            _base = new Entities.Place();
        }

        public Place(Entities.Place source)
        {
            _base = source;
        }

        public void Save()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            if (_base.PlaceId > 0)
            {
                unitOfWork.PlaceRepository.Update(_base);
            }
            else
            {
                unitOfWork.PlaceRepository.Insert(_base);
            }
            unitOfWork.Commit();
        }

        public static Place GetById(long placeId)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            Entities.Place place = unitOfWork.PlaceRepository.Single(p => p.PlaceId == placeId);
            if (place == null) return null;
            return new Place(place);
        }

        public static void Delete(long id)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            Entities.Place place = unitOfWork.PlaceRepository.Single(p => p.PlaceId == id);
            unitOfWork.PlaceRepository.LoadProperty(place, p => p.Place2Zip);
            unitOfWork.PlaceRepository.Delete(place);
            //unitOfWork.Commit();

            //var unitOfWork = new DataProxy.UnitOfWork();
            //var zip = unitOfWork.ZipCodeRepository.Single(z => z.ZipId == id);
            //unitOfWork.ZipCodeRepository.LoadProperty(zip, z => z.Place2Zip);
            //unitOfWork.ZipCodeRepository.Delete(zip);
        }

        public static IEnumerable<Place> GetAllPaged(int start, int pageLength)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            foreach (Entities.Place place in unitOfWork.PlaceRepository
                .Get(orderBy: places => places.OrderBy(p => p.PlaceName))
                .Skip(start)
                .Take(pageLength))
            {
                yield return new Place(place);
            }
        }

        public int GetCount()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            return unitOfWork.PlaceRepository.Get().Count();
        }

        public static IEnumerable<ZipCode> GetZipCodes(long placeId)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var place = unitOfWork.PlaceRepository.Single(p => p.PlaceId == placeId);
            unitOfWork.PlaceRepository.LoadProperty(place, p => p.Place2Zip);
            foreach (var p2z in place.Place2Zip)
            {
                //yield return new ZipCode(p2z.ZipCode);
                yield return ZipCode.GetById(p2z.ZipId);
            }
        }

        public static IEnumerable<ZipCode> GetZipCodes(string placeName)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var place = unitOfWork.PlaceRepository.SingleOrDefault(p => p.PlaceName == placeName);
            unitOfWork.PlaceRepository.LoadProperty(place, p => p.Place2Zip);
            if (place != null)
            {
                foreach (var item in place.Place2Zip)
                {
                    yield return new ZipCode(item.ZipCode);
                }
            }
        }
    }
}
