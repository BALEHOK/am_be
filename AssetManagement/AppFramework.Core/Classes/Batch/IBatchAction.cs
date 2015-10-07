using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.Batch
{
    interface IBatchAction
    {
        AppFramework.ConstantsEnumerators.BatchActionType ActionType { get; set; }
        string ErrorMessage { get; set; }
        void Execute(IUnitOfWork unitOfWork);
        long Order { get; }
        BatchActionParameters Parameters { get; set; }
        void Run();
        AppFramework.ConstantsEnumerators.BatchStatus Status { get; set; }
    }
}
