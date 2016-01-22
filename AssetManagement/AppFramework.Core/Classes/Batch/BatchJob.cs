
using Common.Logging;

namespace AppFramework.Core.Classes.Batch
{
    using AppFramework.ConstantsEnumerators;
    using AppFramework.DataProxy;
    using AppFramework.Entities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;


    /// TODO: refactor to remove
    /// 
    /// <summary>
    /// Single job containing set of actions to update database
    /// </summary>
    [DataObject, System.Runtime.InteropServices.GuidAttribute("917B2B8A-C853-4282-8085-8706711B97B8")]
    public class BatchJob
    {
        public Entities.BatchJob Base { get { return _base; } }
        protected Entities.BatchJob _base;
        protected List<BatchAction> _actions;
        protected volatile CultureInfo _jobCultureInfo;
        private readonly IBatchActionFactory _batchActionFactory;

        BatchJob(IBatchActionFactory batchActionFactory)
        {
            if (batchActionFactory == null)
                throw new ArgumentNullException("batchActionFactory");
            _batchActionFactory = batchActionFactory;
        }

        public BatchJob(string title, long ownerId, IBatchActionFactory batchActionFactory)
            : this(batchActionFactory)
        {
            _base = new Entities.BatchJob()
            {
                Title = title,
                OwnerId = ownerId
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchJob"/> class.
        /// </summary>
        /// <param name="job">The job.</param>
        public BatchJob(Entities.BatchJob job, IBatchActionFactory batchActionFactory)
            : this(batchActionFactory)
        {
            if (job == null)
                throw new ArgumentNullException("BatchJob");
            
            //unitOfWork.BatchJobRepository.LoadProperty(job, e => e.BatchActions);
            _base = job;
            _base.StartTracking();
            if (_base.BatchSchedule != null)
                _base.BatchSchedule.StartTracking();
        }

        /// <summary>
        /// Gets or sets the UID.
        /// </summary>
        /// <value>The UID.</value>
        public long UID
        {
            get { return this._base.BatchUid; }
            set { this._base.BatchUid = value; }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return this._base.Title; }
            set { this._base.Title = value; }
        }

        /// <summary>
        /// Navigates the URL to batch job actions view page
        /// </summary>
        /// <returns></returns>
        public string NavigateUrl
        {
            get
            {
                return string.Format("/admin/Batch/Actions.aspx?BatchUid={0}", this.UID);
            }
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime? StartDate
        {
            get { return this._base.StartDate; }
            set { this._base.StartDate = value; }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime? EndDate
        {
            get { return this._base.EndDate; }
            set { this._base.EndDate = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether need to skip errors in actions and continue running job.
        /// </summary>
        /// <value><c>true</c> if skip errors; otherwise, <c>false</c>.</value>
        public bool SkipErrors
        {
            get { return this._base.SkipErrors; }
            set { this._base.SkipErrors = value; }
        }

        /// <summary>
        /// Gets the current batch job status.
        /// </summary>
        /// <value>The get current status.</value>
        public BatchStatus CurrentStatus
        {
            get
            {
                BatchStatus status = BatchStatus.Created;
                if (GetActions().Count > 0)
                {
                    if (GetActions().Any(a => a.Status == BatchStatus.Running))
                    {
                        status = BatchStatus.Running;
                    }
                    else
                    {
                        if (_base.Status == (int)BatchStatus.InStack)
                            status = BatchStatus.InStack;
                        else if (GetActions().All(a => a.Status == BatchStatus.Created))
                            status = BatchStatus.Created;
                        else if (GetActions().All(a => a.Status == BatchStatus.Finished))
                            status = BatchStatus.Finished;
                        else if (GetActions().Any(a => a.Status == BatchStatus.Skipped) && GetActions().Any(a => a.Status == BatchStatus.Finished) && SkipErrors)
                            status = BatchStatus.FinishedWithErrors;
                        else
                            status = BatchStatus.Error;
                    }
                }
                return status;
            }
        }

        public bool IsAwaiting
        {
            get
            {
                return CurrentStatus == BatchStatus.Running ||
                       CurrentStatus == BatchStatus.Created ||
                       CurrentStatus == BatchStatus.InStack;
            }
        }

        public Entities.BatchSchedule BatchSchedule
        {
            get { return _base.BatchSchedule; }
        }
        
        public void Schedule(double repeatHours, DateTime dateTime, string notes)
        {
            _base.Status = (short)BatchStatus.InStack;
            _base.BatchSchedule = new BatchSchedule()
            {
                RepeatInHours = repeatHours,
                IsEnabled = true,
                ExecuteAt = dateTime
            };
        }

        public void UnSchedule()
        {
            _base.BatchSchedule = null;
            _base.Status = (short)BatchStatus.Created;
        }


        /// <summary>
        /// Gets the actions associated with job.
        /// </summary>
        /// <returns>The actions.</returns>
        public List<BatchAction> GetActions()
        {
            if (_actions == null)
            {
                return _base.BatchActions.Select(a =>
                    _batchActionFactory.GetAction(a))
                    .Where(a => a != null)
                    .ToList();
            }
            return _actions;
        }
    }
}