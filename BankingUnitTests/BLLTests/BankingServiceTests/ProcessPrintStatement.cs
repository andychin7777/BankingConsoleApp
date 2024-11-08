using BankingService.Bll.Model;
using FluentAssertions;
using Moq;
using System.Data;
using System.Linq.Expressions;

namespace BankingUnitTests.BLLTests.BankingServiceTests
{
    [TestFixture]
    public class ProcessPrintStatement : BaseBankingServiceTest
    {
        [Test]
        public void AddInterest_NoTransaction_NoAdd()
        {
            //Arrange
            var account = new Account();
            var interestRules = new List<InterestRule>();
            var dateOnly = new DateOnly();
            
            //Act
            var action = () => _moqBankingService.Object.AddInterest(account, interestRules, dateOnly);

            //Assert
            action.Should().Throw<ArgumentNullException>();
            _moqUnitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public void AddInterest_NoAccountTransactions_NoAdd()
        {
            //Arrange
            var account = new Account()
            {
                AccountTransactions = new List<AccountTransaction>()
            };
            var interestRules = new List<InterestRule>();
            var dateOnly = new DateOnly(1990, 01, 01);

            //Act
            _moqBankingService.Object.AddInterest(account, interestRules, dateOnly);

            //Assert
            account.AccountTransactions.Should().BeEmpty();
            _moqUnitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public void AddInterest_NoInterest_NoAdd_BalanceUpdated()
        {
            //Arrange
            var account = new Account()
            {
                AccountTransactions = new List<AccountTransaction>()
                {
                    new AccountTransaction()
                    {
                        Amount = 999.9m,
                        Balance = 0,
                        Date = new DateOnly(1990, 01, 01),
                        Type = Shared.AccountTransactionType.Deposit
                    }
                }
            };
            var interestRules = new List<InterestRule>();
            var dateOnly = new DateOnly(1990, 01, 01);

            //Act
            _moqBankingService.Object.AddInterest(account, interestRules, dateOnly);

            //Assert
            account.AccountTransactions.Should().NotBeEmpty();
            account.AccountTransactions.First().Balance.Should().Be(999.9m);
            _moqUnitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public void AddInterest_InterestInvalidRange_NoAdd_BalanceUpdated()
        {
            //Arrange
            var account = new Account()
            {
                AccountTransactions = new List<AccountTransaction>()
                {
                    new AccountTransaction()
                    {
                        Amount = 999.9m,
                        Balance = 0,
                        Date = new DateOnly(1990, 01, 01),
                        Type = Shared.AccountTransactionType.Deposit
                    }
                }
            };
            var interestRules = new List<InterestRule>()
            {
                new InterestRule()
                {
                    InterestRuleDateActive = new DateOnly(1990, 02, 01),
                    InterestRate = 99.9m
                }
            };
            var dateOnly = new DateOnly(1990, 01, 01);

            //Act
            _moqBankingService.Object.AddInterest(account, interestRules, dateOnly);

            //Assert
            account.AccountTransactions.Should().NotBeEmpty();
            account.AccountTransactions.First().Balance.Should().Be(999.9m);
            _moqUnitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public void AddInterest_Interest_BalanceUpdated()
        {
            //Arrange
            var account = new Account()
            {
                AccountTransactions = new List<AccountTransaction>()
                {
                    new AccountTransaction()
                    {
                        Amount = 999.9m,
                        Balance = 0,
                        Date = new DateOnly(1990, 01, 01),
                        Type = Shared.AccountTransactionType.Deposit
                    }
                }
            };
            var interestRules = new List<InterestRule>()
            {
                new InterestRule()
                {
                    InterestRuleDateActive = new DateOnly(1990, 01, 30),
                    InterestRate = 99.9m
                }
            };
            var dateOnly = new DateOnly(1990, 01, 01);

            //Act
            _moqBankingService.Object.AddInterest(account, interestRules, dateOnly);

            //Assert
            account.AccountTransactions.Should().NotBeEmpty();
            account.AccountTransactions.First().Balance.Should().Be(999.9m);
            var second = account.AccountTransactions.Skip(1).First();

            second.Balance.Should().BeGreaterThan(999.9m);
            second.Balance.Should().Be(999.9m + second.Amount);
            second.Amount.Should().Be(Math.Round(999.9m * 1 / 365 * interestRules.First().InterestRate / 100, 2, MidpointRounding.ToPositiveInfinity));
            second.Type.Should().Be(Shared.AccountTransactionType.Interest);

            _moqUnitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public void AddInterest_InterestMultiple_BalanceUpdated()
        {

        //Arrange
        var account = new Account()
            {
                AccountTransactions = new List<AccountTransaction>()
                {                    
                    new AccountTransaction()
                    {
                        AccountTransactionId = 1,
                        Amount = 100m,                        
                        Date = new DateOnly(2023, 06, 01),
                        Type = Shared.AccountTransactionType.Deposit
                    },
                    new AccountTransaction()
                    {
                        AccountTransactionId = 2,
                        Amount = 150m,
                        Date = new DateOnly(2023, 06, 01),
                        Type = Shared.AccountTransactionType.Deposit
                    },
                    new AccountTransaction()
                    {
                        AccountTransactionId = 3,
                        Amount = 20m,
                        Date = new DateOnly(2023, 06, 26),
                        Type = Shared.AccountTransactionType.Withdrawal
                    },
                    new AccountTransaction()
                    {
                        AccountTransactionId = 4,
                        Amount = 100m,
                        Date = new DateOnly(2023, 06, 26),
                        Type = Shared.AccountTransactionType.Withdrawal
                    }
                }
            };

            var interestRules = new List<InterestRule>()
            {
                new InterestRule()
                {
                    InterestRuleDateActive = new DateOnly(2023, 01, 01),
                    InterestRate = 1.95m
                },
                new InterestRule()
                {
                    InterestRuleDateActive = new DateOnly(2023, 05, 20),
                    InterestRate = 1.90m
                },
                new InterestRule()
                {
                    InterestRuleDateActive = new DateOnly(2023, 06, 15),
                    InterestRate = 2.20m
                }
            };
            var dateOnly = new DateOnly(2023, 06, 01);

            //Act
            _moqBankingService.Object.AddInterest(account, interestRules, dateOnly);

            //Assert
            account.AccountTransactions.Should().NotBeEmpty();
            var first = account.AccountTransactions.First();
            first.Balance.Should().Be(100m);
            first.Amount.Should().Be(100m);
            first.Type.Should().Be(Shared.AccountTransactionType.Deposit);

            var second = account.AccountTransactions.Skip(1).First();
            second.Balance.Should().Be(250m);
            second.Amount.Should().Be(150m);
            second.Type.Should().Be(Shared.AccountTransactionType.Deposit);

            var third = account.AccountTransactions.Skip(2).First();
            third.Balance.Should().Be(230m);
            third.Amount.Should().Be(20m);
            third.Type.Should().Be(Shared.AccountTransactionType.Withdrawal);

            var forth = account.AccountTransactions.Skip(3).First();
            forth.Balance.Should().Be(130m);
            forth.Amount.Should().Be(100m);
            forth.Type.Should().Be(Shared.AccountTransactionType.Withdrawal);

            var fifth = account.AccountTransactions.Skip(4).First();
            fifth.Balance.Should().Be(130.39m);
            fifth.Amount.Should().Be(0.39m);
            fifth.Type.Should().Be(Shared.AccountTransactionType.Interest);

            _moqUnitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public async Task PrintStatement_NoAccount_ShouldFail()
        {
            //Arrange
            string accountName = "BOB";
            DateOnly date = new DateOnly(1990, 01, 01);

            _moqUnitOfWork.Setup(x => x.AccountRepository.FindWithAccountTransactions(It.IsAny<Expression<Func<BankingService.Sql.Model.Account, bool>>>(),
                It.IsAny<bool>())).ReturnsAsync(() => new List<BankingService.Sql.Model.Account>());

            //Act
            var result = await _moqBankingService.Object.ProcessPrintStatement(accountName, date);

            //Assert
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*account does not exist*");
            _moqUnitOfWork.Verify(x => x.AccountRepository.FindWithAccountTransactions(It.IsAny<Expression<Func<BankingService.Sql.Model.Account, bool>>>(),
                It.IsAny<bool>()), Times.Once);
            _moqUnitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public async Task PrintStatement_ShouldSucceed()
        {
            //Arrange
            string accountName = "BOB";
            DateOnly date = new DateOnly(1990, 01, 01);

            _moqUnitOfWork.Setup(x => x.AccountRepository.FindWithAccountTransactions(It.IsAny<Expression<Func<BankingService.Sql.Model.Account, bool>>>(),
                It.IsAny<bool>())).ReturnsAsync(() => new List<BankingService.Sql.Model.Account>()
                {
                    new BankingService.Sql.Model.Account()
                });

            _moqUnitOfWork.Setup(x => x.InterestRuleRepository.All(It.IsAny<bool>()))
                .ReturnsAsync(() => new List<BankingService.Sql.Model.InterestRule>()
                {
                });

            _moqBankingService.Setup(x => x.AddInterest(It.IsAny<Account>(), It.IsAny<List<InterestRule>>(), It.IsAny<DateOnly>()));

            //Act
            var result = await _moqBankingService.Object.ProcessPrintStatement(accountName, date);

            //Assert
            result.Success.Should().BeTrue();
            result.Messages.Should().BeEmpty();
            _moqUnitOfWork.Verify(x => x.AccountRepository.FindWithAccountTransactions(It.IsAny<Expression<Func<BankingService.Sql.Model.Account, bool>>>(),
                It.IsAny<bool>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.InterestRuleRepository.All(It.IsAny<bool>()), Times.Once);
            _moqBankingService.Verify(x => x.AddInterest(It.IsAny<Account>(), It.IsAny<List<InterestRule>>(), It.IsAny<DateOnly>()), Times.Once);
            _moqUnitOfWork.VerifyNoOtherCalls();
        }
    }
}
