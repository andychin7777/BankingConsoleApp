using Shared;

namespace BankingConsoleApp.Interface;

public interface IBankActionService
{
    public Notification PerformTransaction(string inputString);
}
