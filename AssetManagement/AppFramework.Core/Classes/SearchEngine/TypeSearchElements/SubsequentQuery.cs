
namespace AppFramework.Core.Classes.SearchEngine.TypeSearchElements
{
    /// <summary>
    /// Class for Search by Type (complex search when MultipleAssets and DynLists are involved)
    /// Holds the information about subsequent query to combine all the results together
    /// </summary>
    public class SubsequentQuery
    {
        public AttributeElement AttributeElement { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public SubsequentQuery()
        {
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="chain">search condition</param>
        public SubsequentQuery(AttributeElement chain)
            : base()
        {
            AttributeElement = chain;
        }
    }
}
