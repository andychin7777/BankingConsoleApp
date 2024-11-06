using BankingService.Bll.Model;
using Shared;

namespace BankingConsoleApp.Interface;

public interface IBankActionService
{
    public Task<Notification<Account?>> PerformTransaction(string inputString);
}
