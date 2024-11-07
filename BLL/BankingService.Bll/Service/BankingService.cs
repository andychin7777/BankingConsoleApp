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

    public async Task<Notification<Account>> ProcessTransaction(Account toActionAccount)
    {
        var returnNotification = new Notification<Account>()
        {
            Success = true
        };
        var accountId = 0;

        //validate there are transactions to add
        if (!toActionAccount.AccountTransactions.Any())
        {
            returnNotification.Success = false;
            returnNotification.Messages.Add("No transactions to add");
            return returnNotification;
        }

        await _unitOfWork.RunInTransaction(async () =>
        {
            //match exact on account name
            var databaseAccount = (await _unitOfWork.AccountRepository
                .FindWithAccountTransactions(x => x.AccountName == toActionAccount.AccountName, withTracking: false))
                .FirstOrDefault();
            var validateAccountTransactionCanRun = ValidateCanRunTransaction(databaseAccount, toActionAccount);

            if (!ValidateCanRunTransaction(databaseAccount, toActionAccount))
            {
                returnNotification.Success = false;
                returnNotification.Messages.Add("Unable to apply transactions as it would reduce the balance to below 0");
                return;
            }

            if (databaseAccount != null)
            {                
                //update the account transactions to have the respective ID value 
                toActionAccount.AccountTransactions.ForEach(x => x.AccountId = databaseAccount.AccountId);
                //append records to existing account
                await _unitOfWork.AccountTransactionRepository.AddRange(toActionAccount.AccountTransactions.Select(x => x.MapToSqlAccountTransaction()));                
            }
            else
            {
                //create new account
                databaseAccount = toActionAccount.MapToSqlAccount();
                await _unitOfWork.AccountRepository.Add(databaseAccount);
            }
            await _unitOfWork.SaveChanges();

            //get ID of new account
            accountId = databaseAccount.AccountId;
        });

        //check if there was any errors
        if (!returnNotification.Success)
        {
            return returnNotification;
        }

        //get list of transactions
        var newAccountResult = await _unitOfWork.AccountRepository.GetByIdWithAccountTransactions(accountId);
        returnNotification.Value = newAccountResult.MapToBllAccount();
        return returnNotification;
    }

    private bool ValidateCanRunTransaction(Sql.BankingService.Model.Account databaseAccount, Account currentToActionAccount)
    {
        var total = 0m;
        if (databaseAccount == null)
        {
            total = currentToActionAccount.GetTotalValueOfTransactions();
        }
        else
        {
            total = databaseAccount.MapToBllAccount().GetTotalValueOfTransactions() + currentToActionAccount.GetTotalValueOfTransactions();
        }        

        return total >= 0;
    }
}
