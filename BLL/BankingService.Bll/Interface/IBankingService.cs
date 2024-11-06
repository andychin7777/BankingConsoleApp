using BankingService.Bll.Model;
using Shared;

namespace BankingService.Bll.Interface;

public interface IBankingService
{
    public Task<Notification<Account>> ProcessTransaction(Account account);
    // public Notification<int> AddInterestRule();
    // public Notification<int> ApplyInterest();
}
