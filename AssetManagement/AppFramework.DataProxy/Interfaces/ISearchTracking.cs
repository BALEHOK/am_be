using System;

namespace AppFramework.DataProxy.Interfaces
{
    public interface ISearchTracking
    {
        /// <summary>			
        /// Id : 
        /// </summary>
        /// <remarks>Member of the primary key of the underlying table "SearchTracking"</remarks>
        long Id { get; set; }



        /// <summary>
        /// SearchType : 
        /// </summary>
        System.Int16 SearchType { get; set; }

        /// <summary>
        /// Parameters : 
        /// </summary>
        string Parameters { get; set; }

        /// <summary>
        /// ResultCount : 
        /// </summary>
        System.Int64 ResultCount { get; set; }

        /// <summary>
        /// UpdateUser : 
        /// </summary>
        System.Int64 UpdateUser { get; set; }

        /// <summary>
        /// UpdateDate : 
        /// </summary>
        System.DateTime UpdateDate { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        System.Object Clone();

        #region Data Properties

        #endregion Data Properties

    }
}
