using BankingService.Bll.Model;
using Shared;

namespace BankingConsoleApp.Interface;

public interface IBankActionService
{
    public Notification<Account?> PerformTransaction(string inputString);
}
