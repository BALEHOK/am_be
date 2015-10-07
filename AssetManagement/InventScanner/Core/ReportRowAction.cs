namespace InventScanner.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;

    public enum ReportRowActionId
    {
        Leave = 0,
        Move,        
        Miss
    }

    [DataObject]
    public class ReportRowAction
    {
        [DataObjectField(true)]
        public ReportRowActionId ID { get; set; }

        [DataObjectField(false)]
        public string Title { get; set; }

        public static IEnumerable<ReportRowAction> GetActions()
        {
            var result = new List<ReportRowAction>();
            foreach (ReportRowActionId item in System.Enum.GetValues(typeof(ReportRowActionId)))
            {
                result.Add(new ReportRowAction() { ID = item, Title = item.ToString() });
            }
            return result;
        }
    }
}
