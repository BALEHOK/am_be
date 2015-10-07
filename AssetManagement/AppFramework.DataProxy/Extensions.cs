using System.Collections;

namespace AppFramework.DataLayer
{
    public partial class BatchAction
    {
        public static BatchAction CreateBatchAction(System.Int64 _batchUid, System.Int32 _actionType, System.Int64 _orderId,
            System.Int16 _status, System.String _errorMessage, string _actionParams)
        {
            BatchAction newBatchAction = new BatchAction();
            newBatchAction.BatchUid = _batchUid;
            newBatchAction.ActionType = _actionType;
            newBatchAction.OrderId = _orderId;
            newBatchAction.Status = _status;
            newBatchAction.ErrorMessage = _errorMessage;
            newBatchAction.ActionParams = _actionParams;
            return newBatchAction;
        }
    }

    public partial class BatchJob
    {
        public static BatchJob CreateBatchJob(System.String _title, System.Int64 _ownerId, System.DateTime? _scheduleDate,
        System.DateTime? _startDate, System.DateTime? _endDate, System.Int16 _status, System.Boolean _skipErrors,
        System.Int64? _scheduleId)
        {
            BatchJob newBatchJob = new BatchJob();
            newBatchJob.Title = _title;
            newBatchJob.OwnerId = _ownerId;
            newBatchJob.ScheduleDate = _scheduleDate;
            newBatchJob.StartDate = _startDate;
            newBatchJob.EndDate = _endDate;
            newBatchJob.Status = _status;
            newBatchJob.SkipErrors = _skipErrors;
            newBatchJob.ScheduleId = _scheduleId;
            return newBatchJob;
        }
    }

    public partial class Report
    {
        public static Report CreateReport(System.String _name, System.Boolean _isFinancial, System.String _reportFile,
            System.Int64? _dynConfigId)
        {
            Report newReport = new Report();
            newReport.Name = _name;
            newReport.IsFinancial = _isFinancial;
            newReport.ReportFile = _reportFile;
            newReport.DynConfigId = _dynConfigId;
            return newReport;
        }
    }

    public partial class IndexActiveDynEntities
    {
        public int Rank { get; set; }

        //public long DynEntityConfigUid { get; set; }

        //public long DynEntityUid { get; set; }
    }

    public partial class SearchTracking
    {
        ///<summary>
        ///  Returns a Typed SearchTracking Entity 
        ///</summary>
        protected virtual SearchTracking Copy(IDictionary existingCopies)
        {
            if (existingCopies == null)
            {
                // This is the root of the tree to be copied!
                existingCopies = new Hashtable();
            }

            //shallow copy entity
            SearchTracking copy = new SearchTracking();
            existingCopies.Add(this, copy);
            copy.Id = this.Id;
            copy.SearchType = this.SearchType;
            copy.Parameters = this.Parameters;
            copy.ResultCount = this.ResultCount;
            copy.UpdateUser = this.UpdateUser;
            copy.UpdateDate = this.UpdateDate;

            return copy;
        }



        ///<summary>
        ///  Returns a Typed SearchTracking Entity 
        ///</summary>
        public virtual SearchTracking Copy()
        {
            return this.Copy(null);
        }

        ///<summary>
        /// ICloneable.Clone() Member, returns the Shallow Copy of this entity.
        ///</summary>
        public object Clone()
        {
            return this.Copy(null);
        }

        ///<summary>
        /// ICloneableEx.Clone() Member, returns the Shallow Copy of this entity.
        ///</summary>
        public object Clone(IDictionary existingCopies)
        {
            return this.Copy(existingCopies);
        }
    }

    public partial class ReportField
    {
        public static ReportField CreateReportField(System.Int64 _reportUid, System.String _name, System.Boolean _isVisible,
            System.Boolean _isFilter)
        {
            ReportField newReportField = new ReportField();
            newReportField.ReportUid = _reportUid;
            newReportField.Name = _name;
            newReportField.IsVisible = _isVisible;
            newReportField.IsFilter = _isFilter;
            return newReportField;
        }
    }

    public static class Extensions
    {
        //public static bool IsUserInRole(this UserInRole role, System.Int64 _userId, System.Int64 _roleId)
        //{
        //    BUBnSOBEntities db = new BUBnSOBEntities();
        //    UserInRole uir = db.UserInRole.FirstOrDefault(u => u.RoleId == _roleId && u.UserId == _userId);
        //    db.Dispose();

        //    return uir != null;
        //}
    }
}
