namespace BankingService.Dal.Interfaces
{
    public interface IGenericUnitOfWork
    {
        Task RunInTransaction(Func<Task> completeAction);
        Task SaveChanges();
    }
}
