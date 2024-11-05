﻿using BankingConsoleApp;
using BankingConsoleApp.Interface;
using BankingConsoleApp.Mapping;
using BankingService.Bll.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;

internal class Program
{
    private static ServiceProvider _serviceProvider;

    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        Ioc.Init(builder.Services);
        _serviceProvider = builder.Services.BuildServiceProvider();

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
            if (result.notification.Messages != null && result.notification.Messages.Any())
            {
                foreach (var message in result.notification.Messages)
                {
                    Console.WriteLine(message);
                }
            }
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

    /// <summary>
    /// Return true in notification if required to continue;
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private static (Notification notification, bool isExit) HandleLineActionInput(string? line)
    {
        if (String.IsNullOrEmpty(line))
        {
            return (new Notification
            {
                Messages = new List<string>() { "Invalid input please try again." },
                Success = true
            }, false);
        }

        var bankAction = BankActionTypeMapper.MapToBankActionType(line);
        switch (bankAction)
        {
            case BankActionType.Transaction:
            case BankActionType.InterestRules:
            case BankActionType.PrintStatement:
                {
                    return (HandleActionInputValue(bankAction.Value), false);
                }
            case BankActionType.Quit:
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

    private static Notification HandleActionInputValue(BankActionType bankActionType)
    {
        if (bankActionType == BankActionType.Transaction)
        {
            Console.WriteLine("Please enter transaction details in <Date> <Account> <Type> <Amount> format");
        }
        else if (bankActionType == BankActionType.InterestRules)
        {
            Console.WriteLine("Please enter interest rules details in <Date> <RuleId> <Rate in %> format ");
        }
        else if (bankActionType == BankActionType.PrintStatement)
        {
            Console.WriteLine("Please enter account and month to generate the statement <Account> <Year><Month>");
        }
        Console.WriteLine("or enter blank to go back to main menu):");

        var newLineRead = Console.ReadLine();
        if (!string.IsNullOrEmpty(newLineRead))
        {
            if(bankActionType == BankActionType.Transaction)
            {
                return _serviceProvider.GetRequiredService<IBankActionService>().PerformTransaction(newLineRead);
            }

            //TODO process action here.
        }
        return new Notification
        {
            Success = true
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