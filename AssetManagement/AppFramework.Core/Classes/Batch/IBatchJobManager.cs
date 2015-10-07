using AppFramework.ConstantsEnumerators;

namespace AppFramework.Core.Classes.Batch
{
    public interface IBatchJobManager
    {
        void AddAction(BatchJob job, BatchActionType actionType, BatchActionParameters @params, bool isMandatory = true);

        /// <summary>
        /// Permanently deletes the finished jobs
        /// </summary>
        void DeleteAllJobs();

        /// <summary>
        /// Permanently deletes the finished jobs
        /// </summary>
        void DeleteFinishedJobs();

        /// <summary>
        /// Returns job instance by its UID
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        BatchJob GetByUid(long uid);

        /// <summary>
        /// Saves job instance
        /// </summary>
        /// <param name="job"></param>
        void SaveJob(BatchJob job);

        /// <summary>
        /// Mark job as enqueued
        /// </summary>
        void StackForExecution(BatchJob job);
    }
}