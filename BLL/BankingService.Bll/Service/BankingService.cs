using BankingService.Bll.Interface;
using BankingService.Bll.Model;
using Shared;

namespace BankingService.Bll.Service;

public class BankingService : IBankingService
{
    public Notification<Account> ProcessTransaction(Account account)
    {
        throw new NotImplementedException();
    }
}
