namespace AppFramework.Core.Classes
{
    using System.Collections.Generic;
    using System.Linq;
    using AppFramework.Core.ConstantsEnumerators;
    using AppFramework.Entities;

    /// <summary>
    /// Describes the row from Rights table
    /// </summary>
    public class RightsEntry
    {
        public long RightsUid { get; set; }
        public long AssetTypeID { get; set; }
        public long TaxonomyItemId { get; set; }
        public long DepartmentID { get; set; }
        public AC.Authentication.Permission Permission { get; set; }
        public bool IsDeny { get; set; }
        public long ViewID { get; set; }
        public long UserID { get; set; }

        private Rights _data;

        /// <summary>
        /// Class constructor
        /// </summary>
        public RightsEntry() { }

        /// <summary>
        /// Class constructor with initialization by DynEntity data
        /// </summary>
        public RightsEntry(Rights data)
        {
            _data = data;
            RightsUid = data.RightsUid;
            AssetTypeID = data.DynEntityConfigId;
            TaxonomyItemId = data.CategoryId;
            DepartmentID = data.DepartmentId;
            Permission = AC.Authentication.PermissionsProvider.GetByCode(data.Rights1);
            IsDeny = data.IsDeny;
            ViewID = data.ViewId;
            UserID = data.UserId;
        }
        
        /// <summary>
        /// Returns if this rule is matches with provided asset attributes or not
        /// </summary>
        /// <param name="asset">Asset for checking</param>
        /// <returns>True if appropriate rule is found and false if not</returns>
        public bool Matches(long dynEntityConfigId, long? deptId, List<long> taxonomyItemsIds)
        {
            bool[] matched = new bool[3];

            // if equals by AssetType or AssetType is not defined at all
            if (AssetTypeID == 0 ||
                AssetTypeID == dynEntityConfigId)
            {
                matched[0] = true;
            }

            // if one of assigned categories for this assetType
            // is in list
            if (TaxonomyItemId == 0 || taxonomyItemsIds.Any(ti => ti == TaxonomyItemId))
            {
                matched[1] = true;
            }

            // if department of this asset is in list
            if (!deptId.HasValue || DepartmentID == 0 || DepartmentID == deptId)
            {
                matched[2] = true;
            }

            // binary multiply all matches
            return matched[0] & matched[1] & matched[2];
        }
    }
}
