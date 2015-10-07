namespace AppFramework.DataProxy.Interfaces
{
    public interface IBatchAction
    {
        string ActionParams { get; set; }
        int ActionType { get; set; }
        long BatchActionUid { get; set; }
        IBatchJob BatchJob { get; set; }
        long BatchUid { get; set; }
        string ErrorMessage { get; set; }
        long Order { get; }
        long OrderId { get; set; }
        short Status { get; set; }
    }
}
