namespace AppFramework.Core.Classes.SearchEngine.TypeSearchElements
{
    using System.Collections.Generic;

    public class Operator
    {
        public long OperatorValue { get; set; }
        public string OperatorText { get; set; }
        public string OperatorMethod { get; set; }
        //public bool IsDropDown { get; set; }
        public bool IsDynListDropDown { get; set; }
        public bool IsAssetListDropDown { get; set; }
        public List<KeyValuePair<string, long>> Items { get; set; }

        public Operator(Entities.SearchOperators data)
        {
            OperatorText = data.Operator;
            OperatorValue = data.SearchOperatorUid;
            OperatorMethod = data.ServiceMethod;
        }
    }
}
