using BankingService.Interface;
using BankingService.Model;
using Shared;

namespace BankingService.Services;

public class BankingService : IBankingService
{
    public Notification<Account> ProcessTransaction()
    {
        throw new NotImplementedException();
    }
}
