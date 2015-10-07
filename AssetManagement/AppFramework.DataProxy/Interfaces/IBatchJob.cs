namespace AppFramework.DataProxy.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public interface IBatchJob
    {
        ReadOnlyCollection<IBatchAction> Actions { get; }
        IEnumerable<IBatchAction> BatchAction { get; set; }
        IBatchSchedule BatchSchedule { get; set; }
        long BatchUid { get; set; }
        DateTime? EndDate { get; set; }
        long OwnerId { get; set; }
        DateTime? ScheduleDate { get; set; }
        long? ScheduleId { get; set; }
        bool SkipErrors { get; set; }
        DateTime? StartDate { get; set; }
        short Status { get; set; }
        string Title { get; set; }
    }
}
