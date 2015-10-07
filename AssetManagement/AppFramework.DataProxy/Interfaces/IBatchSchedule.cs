namespace AppFramework.DataProxy.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IBatchSchedule
    {
        IEnumerable<IBatchJob> BatchJob { get; set; }
        int ExecutionInterval { get; set; }
        DateTime? LastStart { get; set; }
        long ScheduleId { get; set; }
        int ScheduleType { get; set; }
    }
}
