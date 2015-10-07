using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    /// <summary>
    /// Describes the row from TaskRights table
    /// </summary>
    public class TaskRightsEntry
    {
        public long TaskRightsId { get; set; }
        public long? TaxonomyItemId { get; set; }
        public long? DynEntityConfigId { get; set; }
        public long UserID { get; set; }
        public long ViewID { get; set; }
        public bool IsDeny { get; set; }

        private TaskRights _data;

        /// <summary>
        /// Class constructor
        /// </summary>
        public TaskRightsEntry() { }

        /// <summary>
        /// Class constructor with initialization by DynEntity data
        /// </summary>
        public TaskRightsEntry(TaskRights data)
        {
            _data = data;
            TaskRightsId = data.TaskRightsId;
            TaxonomyItemId = data.TaxonomyItemId;
            DynEntityConfigId = data.DynEntityConfigId;
            UserID = data.UserId;
            ViewID = data.ViewId;
            IsDeny = data.IsDeny;
        }
    }
}
