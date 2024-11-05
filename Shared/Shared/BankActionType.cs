namespace Shared;

public enum BankActionType
{
    Transaction,
    InterestRules,
    PrintStatement,
    Quit
}

public static class BankActionTypeMapper
{
    public static BankActionType? MapToBankActionType(string mappingString)
    {
        var toLowerString = mappingString.ToLowerInvariant();

        switch (toLowerString)
        {
            case "t":
                return BankActionType.Transaction;
            case "i":
                return BankActionType.InterestRules;
            case "p":
                return BankActionType.PrintStatement;
            case "q":
                return BankActionType.Quit;            
            default:
                return null;
        }

    }
}