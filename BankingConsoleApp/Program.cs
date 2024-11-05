using Shared;

internal class Program
{
    private static void Main(string[] args)
    {
        bool displayedWelcome = false;

        while (true)
        {
            DisplayMessage(displayedWelcome);
            //set the value of displayed to true once the initial welcome is displayed.
            if (!displayedWelcome)
            {
                displayedWelcome = true;
            }
            var line = Console.ReadLine();
            var result = HandleLineActionInput(line);

            //display messages to user
            if (result.Messages != null && result.Messages.Any())
            {
                foreach (var message in result.Messages)
                {
                    Console.WriteLine(message);
                }
            }
            //break the loop here.
            if (!result.Value)
            {
                break;
            }
        }

        //End application run
        Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
        Console.WriteLine("Have a nice day!");

        //keep console open
        Console.ReadLine();
    }

    /// <summary>
    /// Return true in notification if required to continue;
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private static Notification<bool> HandleLineActionInput(string? line)
    {
        if (String.IsNullOrEmpty(line))
        {
            return new Notification<bool>
            {
                Messages = new List<string>() { "Invalid input please try again." },
                Value = true
            };
        }

        var bankingAction = BankActionTypeMapper.MapToBankActionType(line);
        switch (bankingAction)
        {
            case BankActionType.Quit:
                {
                    return new Notification<bool>
                    {
                        Value = false
                    };
                }
            default:
                {
                    return new Notification<bool>
                    {
                        Value = true
                    };
                }
        }
    }

    private static void HandleActionInputValue()
    {

    }

    private static void DisplayMessage(bool displayedWelcome)
    {
        if (!displayedWelcome)
        {
            Console.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
        }
        else
        {
            Console.WriteLine("Is there anything else you'd like to do?");
        }
        PrintLinesActions();
    }

    private static void PrintLinesActions()
    {
        Console.WriteLine("[T] Input transactions");
        Console.WriteLine("[I] Define interest rules");
        Console.WriteLine("[P] Print statement");
        Console.WriteLine("[Q] Quit");
    }
}