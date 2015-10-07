namespace AppFramework.Core.Classes.Batch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Collec information about currently running batch jobs
    /// </summary>
    public sealed class BatchJobObserver
    {
        private Dictionary<long, WeakReference> jobs;
        
        private BatchJobObserver()
        {
            jobs = new Dictionary<long, WeakReference>();
        }

        public void AddJob(BatchJob job)
        {
            WeakReference r = new WeakReference(job);
            lock (this.jobs)
            {
                if (!this.jobs.ContainsKey(job.UID))
                    this.jobs.Add(job.UID, r);
            }
        }

        /// <summary>
        /// Gets the status of job with given UID.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public BatchJob GetStatus(long uid)
        {
            lock (this.jobs)
            {
                //WeakReference r = this.jobs.FirstOrDefault(wr => wr.IsAlive && (wr.Target is BatchJob) && (wr.Target as BatchJob).UID == uid);
                //return (r == null ? null : r.Target as BatchJob);
                BatchJob res = null;
                if (jobs.ContainsKey(uid))
                {
                    WeakReference r = jobs[uid];
                    if (r.IsAlive)
                    {
                        res = r.Target as BatchJob;
                    }
                    else
                    {
                        jobs.Remove(uid);
                    }
                }
                return res;
            }
        }

        #region Singleton
        private static readonly BatchJobObserver instance = new BatchJobObserver();

        static BatchJobObserver()
        {
        }

        public static BatchJobObserver Instance
        {
            get
            {
                return instance;
            }
        } 
        #endregion

    }
}
