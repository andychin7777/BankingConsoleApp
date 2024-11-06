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
    public static BankActionType? MapToBankActionType(this string mappingString)
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
    public static string ToString(this BankActionType bankActionType)
    {
        switch (bankActionType)
        {
            case BankActionType.Transaction:
                return "T";
            case BankActionType.InterestRules:
                return "I";
            case BankActionType.PrintStatement:
                return "P";
            case BankActionType.Quit:
                return "Q";            
            default:
                return null;
        }
    }
}