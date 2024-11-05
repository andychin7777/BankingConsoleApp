using System;
using BankingConsoleApp.Interface;
using BankingConsoleApp.Mapping;
using BankingService.Bll.Interface;
using Shared;

namespace BankingConsoleApp.Service;

public class BankActionService : IBankActionService
{
    private IBankingService _bankingService;

    public BankActionService(IBankingService bankingService)
    {
        _bankingService = bankingService;
    }

    public Notification PerformTransaction(string inputString)
    {
        var bankActionHelperResponse = BankActionValidateMapper.MapStringToAccountAndTransaction(inputString);
        if (!bankActionHelperResponse.Success)
        {
            return bankActionHelperResponse;
        }
        _bankingService.ProcessTransaction(bankActionHelperResponse.Value);

        //TODO: dont return null here.
        return null;
    }
}

