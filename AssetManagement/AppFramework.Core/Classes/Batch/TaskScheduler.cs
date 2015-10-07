using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using thread = System.Threading;

namespace AppFramework.Core.Classes.Batch
{
    /// <summary>
    /// Task Events Handler
    /// </summary>
    /// <param name="sender">Sender Object (TaskScheduler) </param>
    /// <param name="taskName">Name of Started/Ended Task</param>
    public delegate void TaskHandler(object sender, string taskName);

    /// <summary>
    /// Event of Task
    /// </summary>
    /// <param name="task"></param>
    public delegate void TaskEvent(TaskOfScheduler task);

    /// <summary>
    /// Method of Compare the DateTimes
    /// </summary>
    public enum TaskOfSchedulerTimeCompare
    {
        CompareHoursMinutesAndSeconds,
        CompareHoursAndMinutes,
        CompareOnlyHours,
        CompareOnlyDate
    }

    /// <summary>
    /// Task
    /// </summary>
    public struct TaskOfScheduler
    {
        /// <summary>
        /// DateTime to fire the task
        /// </summary>
        public DateTime ScheduledFor { get; set; }

        /// <summary>
        /// Simple Task Identifier
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// Start Callback
        /// </summary>
        public TaskEvent OnStart { get; set; }

        /// <summary>
        /// Finalize Callback
        /// </summary>
        public TaskEvent OnEnd { get; set; }

        /// <summary>
        /// Cache 
        /// </summary>
        public IDictionary<string,object> Cache { get; set; }
    }

    /// <summary>
    /// Task Manager
    /// </summary>
    public class TaskScheduler
    {
        public TaskScheduler()
        {
            UseBackgroundThreads = true;
            Tasks = new List<TaskOfScheduler>();
            IntervalWork = 60000;
            AutoDeletedExecutedTasks = true;
            StoreThreadsInPool = true;
            OnStopSchedulerAutoCancelThreads = true;
        }

        private List<TaskOfScheduler> task = new List<TaskOfScheduler>();
        private Timer taskWorker = new Timer();
        private List<thread.Thread> poolTasks = new List<thread.Thread>();

        /// <summary>
        /// Is Working?
        /// </summary>
        public bool IsWorking
        {
            get
            {
                if (taskWorker != null && taskWorker.Enabled)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Enable to Auto Cancel Threads on Scheduler Stop.
        /// </summary>
        public bool OnStopSchedulerAutoCancelThreads { get; set; }

        /// <summary>
        /// Used with BackgroundTasks enable to store the tasks threads on pool of threads.
        /// </summary>
        public bool StoreThreadsInPool { get; set; }

        /// <summary>
        /// Fired on Task Start
        /// </summary>
        public event TaskHandler OnTaskStart;

        /// <summary>
        /// Fire on Task End
        /// </summary>
        public event TaskHandler OnTaskEnd;

        /// <summary>
        /// Fired when all tasks are performed.
        /// </summary>
        public event EventHandler OnAllTasksEnds;

        /// <summary>
        /// Use Background Threads to Execute a Task.
        /// </summary>
        public bool UseBackgroundThreads { get; set; }

        /// <summary>
        /// Enable the Auto-Delete of Executed Tasks
        /// </summary>
        public bool AutoDeletedExecutedTasks { get; set; }

        /// <summary>
        /// Sheduled Tasks
        /// </summary>
        public List<TaskOfScheduler> Tasks
        {
            get { return task; }
            set { task = value; }
        }

        /// <summary>
        /// Time (seconds) between checks for tasks (recommended no less than one minute).
        /// </summary>
        public int IntervalWork { get; set; }

        /// <summary>
        /// Method Comparison Time
        /// </summary>
        public TaskOfSchedulerTimeCompare TimeCompareMethod { get; set; }

        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            startTasks();
        }

        /// <summary>
        /// Stop
        /// </summary>
        public void Stop()
        {
            if (taskWorker != null)
                taskWorker.Stop();

            if (StoreThreadsInPool && OnStopSchedulerAutoCancelThreads)
            {
                foreach (var pooledTask in poolTasks)
                {
                    if (pooledTask.IsAlive)
                    {
                        try { pooledTask.Abort(); }
                        catch { }
                    }
                }
            }
        }

        private void startTasks()
        {
            taskWorker = new Timer(IntervalWork * 1000);
            taskWorker.Elapsed += new ElapsedEventHandler(taskWorker_Elapsed);
            taskWorker.Start();
        }

        void taskWorker_Elapsed(object sender, ElapsedEventArgs e)
        {
            doTasks();
        }

        private void doTasks()
        {
            DateTime now = DateTime.Now;
            List<TaskOfScheduler> taskToDo = new List<TaskOfScheduler>();
            IEnumerable<TaskOfScheduler> query;

            query = GetTasksToDo(now);

            if (query != null && query.Count() >= 1)
            {
                taskToDo = query.ToList();
            }

            foreach (var item in taskToDo)
            {
                if (item.OnStart != null)
                {
                    if (UseBackgroundThreads)
                        ExecuteThreadedTask(item);
                    else
                        ExecuteTask(item);
                }
            }
        }

        private void ExecuteThreadedTask(TaskOfScheduler item)
        {
            thread.Thread tr = new thread.Thread(new thread.ParameterizedThreadStart(
                    (o) =>
                    {
                        ExecuteTask(item);

                        if (StoreThreadsInPool)
                        {
                            poolTasks.RemoveAt((int)o);
                        }
                    }
                ));


            tr.IsBackground = true;
            tr.Priority = thread.ThreadPriority.Normal;

            if (StoreThreadsInPool)
            {
                poolTasks.Add(tr);
                tr.Start(poolTasks.IndexOf(tr));
            }
            else
            {
                tr.Start(-1);
            }
        }

        private TaskOfScheduler ExecuteTask(TaskOfScheduler item)
        {
            if (this.OnTaskStart != null)
                OnTaskStart(this, item.TaskName);

            item.OnStart(item);

            if (item.OnEnd != null)
                item.OnEnd(item);

            if (this.OnTaskEnd != null)
                this.OnTaskEnd(this, item.TaskName);

            // Remove the Task//
            if (AutoDeletedExecutedTasks)
                Tasks.Remove(item);

            // Call the Event //
            if (Tasks.Count == 0)
            {
                if (this.OnAllTasksEnds != null)
                    this.OnAllTasksEnds(this, new EventArgs());
            }
            return item;
        }

        private IEnumerable<TaskOfScheduler> GetTasksToDo(DateTime now)
        {
            IEnumerable<TaskOfScheduler> query = new List<TaskOfScheduler>().AsEnumerable();

            if (TimeCompareMethod == TaskOfSchedulerTimeCompare.CompareHoursMinutesAndSeconds)
            {
                query = Tasks.Where(tsk => tsk.ScheduledFor == now);
            }
            else if (TimeCompareMethod == TaskOfSchedulerTimeCompare.CompareHoursAndMinutes)
            {
                query = Tasks.Where(tsk =>
                    tsk.ScheduledFor.Date == now.Date
                    && tsk.ScheduledFor.Hour == now.Hour
                    && tsk.ScheduledFor.Minute == now.Minute
                    );
            }
            else if (TimeCompareMethod == TaskOfSchedulerTimeCompare.CompareOnlyDate)
            {
                query = Tasks.Where(tsk =>
                    tsk.ScheduledFor.Date == now.Date
                    );
            }
            else if (TimeCompareMethod == TaskOfSchedulerTimeCompare.CompareOnlyHours)
            {
                query = Tasks.Where(tsk =>
                    tsk.ScheduledFor.Date == now.Date
                    && tsk.ScheduledFor.Hour == now.Hour
                    );
            }

            return query;
        }
    }
}
