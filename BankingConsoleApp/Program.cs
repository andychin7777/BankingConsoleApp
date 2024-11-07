using BankingConsoleApp;
using BankingConsoleApp.Interface;
using Bootstrap;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;
using Shared.Mapping;

internal class Program
{
    private static IBankActionService _iBankActionService;
    private static async Task Main(string[] args)
    {
        var iHostBuilder = Host.CreateDefaultBuilder();
        Startup.Init(iHostBuilder);
        iHostBuilder.ConfigureServices((hostingContext, services) =>
                    {
                        services.AddProgram();
                    });

        using var host = iHostBuilder.Build();

        //begin scope
        using var serviceScope = host.Services.CreateScope();
        var _serviceProvider = serviceScope.ServiceProvider;

        _iBankActionService = _serviceProvider.GetRequiredService<IBankActionService>();

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
            var result = await HandleLineActionInput(line);

            DisplayMessagesOnConsole(result.notification);
            //break the loop here.
            if (result.isExit)
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

    private static void DisplayMessagesOnConsole(Notification notification)
    {
        //display messages to user
        if (notification.Messages != null && notification.Messages.Any())
        {
            foreach (var message in notification.Messages)
            {
                Console.WriteLine(message);
            }
        }
    }

    /// <summary>
    /// Return true in notification if required to continue;
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private static async Task<(Notification notification, bool isExit)> HandleLineActionInput(string? line)
    {
        if (String.IsNullOrEmpty(line))
        {
            return (new Notification
            {
                Messages = new List<string>() { "Invalid input please try again." },
                Success = true
            }, false);
        }

        var bankAction = EnumnMapping.MapToBankActionType(line);
        switch (bankAction)
        {
            case BankingActionType.Transaction:
            case BankingActionType.InterestRules:
            case BankingActionType.PrintStatement:
                {
                    return (await HandleActionInputValue(bankAction.Value), false);
                }
            case BankingActionType.Quit:
                {
                    return (new Notification
                    {
                        Success = true
                    }, true);
                }
            default:
                {
                    return (new Notification
                    {
                        Success = true
                    }, false);
                }
        }
    }

    private static async Task<Notification> HandleActionInputValue(BankingActionType bankActionType)
    {
        while(true)
        {
            if (bankActionType == BankingActionType.Transaction)
            {
                Console.WriteLine("Please enter transaction details in <Date> <Account> <Type> <Amount> format");
            }
            else if (bankActionType == BankingActionType.InterestRules)
            {
                Console.WriteLine("Please enter interest rules details in <Date> <RuleId> <Rate in %> format ");
            }
            else if (bankActionType == BankingActionType.PrintStatement)
            {
                Console.WriteLine("Please enter account and month to generate the statement <Account> <Year><Month>");
            }
            Console.WriteLine("or enter blank to go back to main menu):");

            var newLineRead = Console.ReadLine();
            if (!string.IsNullOrEmpty(newLineRead))
            {
                var result = await ProcessInSubMenuLineRead(newLineRead, bankActionType);
                if (!result.Success)
                {
                    DisplayMessagesOnConsole(result);
                }
            }
            //only break this loop if the user enters 0 to exit
            else
            {
                break;
            }
        }        

        return new Notification
        {
            Success = true
        };
    }

    private static async Task<Notification> ProcessInSubMenuLineRead(string newLineRead, BankingActionType bankActionType)
    {
        if (bankActionType == BankingActionType.Transaction)
        {
            return await _iBankActionService.PerformTransaction(newLineRead);
        }

        if (bankActionType == BankingActionType.InterestRules)
        {
            return await _iBankActionService.PerformDefineInterestRules(newLineRead);
        }

        if (bankActionType == BankingActionType.PrintStatement)
        {
            return await _iBankActionService.
        }
        return new Notification
        {
            Messages = new List<string> { "Invalid Action" },
            Success = false
        };
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