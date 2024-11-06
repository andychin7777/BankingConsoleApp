namespace BankingService.Dal.Interfaces
{
    public interface IUnitOfWork : IGenericUnitOfWork
    {
        ISettingRepository SettingRepository { get; }
    }
}
