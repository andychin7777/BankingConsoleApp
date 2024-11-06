using BankingService.Bll.Interface;
using BankingService.Bll.Mapping;
using BankingService.Bll.Model;
using BankingService.Dal.Interfaces;
using Shared;

namespace BankingService.Bll.Service;

public class BankingService : IBankingService
{
    private IUnitOfWork _unitOfWork { get; set; }

    public BankingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Notification<Account>> ProcessTransaction(Account account)
    {
        var returnNotification = new Notification<Account>();
        var accountId = 0;

        //validate there are transactions to add
        if (!account.AccountTransactions.Any())
        {
            returnNotification.Success = false;
            returnNotification.Messages.Add("No transactions to add");
            return returnNotification;
        }

        await _unitOfWork.RunInTransaction(async () =>
        {
            //match exact on account name
            var existingAccount = (await _unitOfWork.AccountRepository.Find(x => x.AccountName == account.AccountName, withTracking: false)).First();
            if (existingAccount != null)
            {
                //update the account transactions to have the respective ID value 
                account.AccountTransactions.ForEach(x => x.AccountId = existingAccount.AccountId);
                //append records to existing account
                await _unitOfWork.AccountTransactionRepository.AddRange(account.AccountTransactions.Select(x => x.MapToSqlAccountTransaction()));
            }
            else
            {
                //create new account
                var newAccount = account.MapToSqlAccount();
                await _unitOfWork.AccountRepository.Add(newAccount);

                //get ID of new account
                accountId = newAccount.AccountId;
            }
            await _unitOfWork.SaveChanges();
        });

        //get list of transactions
        var newAccountResult = await _unitOfWork.AccountRepository.GetByIdWithAccountTransactions(accountId);
        returnNotification.Value = newAccountResult.MapToBllAccount();

        return returnNotification;
    }
}
