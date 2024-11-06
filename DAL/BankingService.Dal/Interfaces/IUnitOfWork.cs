namespace BankingService.Dal.Interfaces
{
    public interface IUnitOfWork : IGenericUnitOfWork
    {
        IAccountRepository AccountRepository { get; }
        IAccountTransactionRepository AccountTransactionRepository { get; }
    }
}
