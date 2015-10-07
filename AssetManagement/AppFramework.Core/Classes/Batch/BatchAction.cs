using System.Data;
using System.Data.SqlClient;
using System.Text;
using AppFramework.DataProxy;
using Common.Logging;
using System;
using AppFramework.ConstantsEnumerators;

namespace AppFramework.Core.Classes.Batch
{
    public abstract class BatchAction : IBatchAction
    {
        /// <summary>
        /// Runs this instance.
        /// </summary>
        public abstract void Run();

        protected Entities.BatchAction _base;
        private BatchJob batchJob;
        private BatchActionParameters _parameters;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        protected BatchAction(Entities.BatchAction batchAction)
        {
            if (batchAction == null)
                throw new ArgumentNullException("BatchAction");

            _base = batchAction;
        }

        public long BatchActionUid
        {
            get { return _base.BatchActionUid; }
        }

        /// <summary>
        /// Gets the action order.
        /// </summary>
        /// <value>The order.</value>
        public long Order
        {
            get { return this._base.OrderId; }
        }

        public bool IsMandatory
        {
            get { return _base.IsMandatory; }
        }

        /// <summary>
        /// Gets or sets the type of the action.
        /// </summary>
        /// <value>The type of the action.</value>
        public BatchActionType ActionType
        {
            get { return (BatchActionType)this._base.ActionType; }
            set { this._base.ActionType = (int)value; }
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public BatchStatus Status
        {
            get { return (BatchStatus)_base.Status; }
            set { _base.Status = (short)value; }
        }

        /// <summary>
        /// Gets the parameters of action.
        /// </summary>
        /// <value>The parameters.</value>
        public BatchActionParameters Parameters
        {
            get
            {
                if (this._parameters == null || this._parameters.Count == 0)
                {
                    if (this._parameters == null) this._parameters = new BatchActionParameters();
                    if (!string.IsNullOrEmpty(this._base.ActionParams))
                    {
                        this._parameters = BatchActionParameters.GetFromXml(this._base.ActionParams);
                    }
                }
                return this._parameters;
            }
            set
            {
                this._base.ActionParams = value.ToXml();
                this._parameters = value;
            }
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage
        {
            get { return _base.ErrorMessage; }
            set { _base.ErrorMessage = value; }
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Execute(IUnitOfWork unitOfWork)
        {
            try
            {
                ErrorMessage = string.Empty;
                Status = BatchStatus.Running;
                UpdateStatus(unitOfWork);
                Run();
                Status = BatchStatus.Finished;
            }
            catch (Exception e)
            {
                // check flag of batch job - need to stop executing with error or just ignore it   
                var messageBuilder = new StringBuilder(e.Message);
                if (e.InnerException != null)
                {
                    messageBuilder.AppendLine();
                    messageBuilder.AppendFormat("{0}{1}{0}{2}",
                        Environment.NewLine,
                        "Inner exception:",
                        e.InnerException.Message);
                }

                if (!String.IsNullOrEmpty(e.Source))
                {
                    messageBuilder.AppendLine();
                    messageBuilder.AppendFormat("{0}{1}{0}{2}",
                        Environment.NewLine,
                        "Source:",
                        e.Source);
                }

                if (!String.IsNullOrEmpty(e.StackTrace))
                {
                    messageBuilder.AppendLine();
                    messageBuilder.AppendFormat("{0}{1}{0}{2}",
                        Environment.NewLine,
                        "Stack trace:",
                        e.StackTrace);
                }

                this.ErrorMessage = messageBuilder
                                    + Environment.NewLine
                                    + "Please refer to the logfile for action parameters.";
                this.Status = BatchStatus.Error;

                messageBuilder.AppendLine("Action Parameters:");
                messageBuilder.AppendLine(Parameters.ToXml());
                _logger.Warn(messageBuilder.ToString());
            }
        }

        /// <summary>
        /// Saves this Action 
        /// </summary>
        public void UpdateStatus(IUnitOfWork unitOfWork, BatchStatus? status = null)
        {
            lock (this)
            {
                if (_base.BatchActionUid > 0)
                {
                    var actionStatus = status != null ? (int) status : (int) Status;

                    var paremeters = new IDataParameter[]
                    {
                        new SqlParameter("@Status", actionStatus),
                        new SqlParameter("@Error", ErrorMessage),
                        new SqlParameter("@ActionUid", _base.BatchActionUid)
                    };

                    unitOfWork.SqlProvider.ExecuteNonQuery(
                        "update BatchAction set Status=@Status, ErrorMessage=@Error where BatchActionUid=@ActionUid",
                        paremeters);

                    unitOfWork.BatchActionRepository.Update(_base);
                }
            }
        }
    }
}