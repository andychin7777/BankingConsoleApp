using BankingService.Model;
using Shared;

namespace BankingService.Interface;

public interface IBankingService
{
    public Notification<Account> ProcessTransaction();
    // public Notification<int> AddInterestRule();
    // public Notification<int> ApplyInterest();
}
