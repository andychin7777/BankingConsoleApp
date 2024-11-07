using Shared;

namespace BankingConsoleApp.Interface;

public interface IBankActionService
{
    public Task<Notification> PerformTransaction(string inputString);
    public Task<Notification> PerformDefineInterestRules(string inputString);
    public Task<Notification> PerformPrintStatement(string inputString);
}
