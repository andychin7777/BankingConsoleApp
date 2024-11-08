using BankingService.Bll.Model;
using BankingService.Dal.Interfaces;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using Shared.Mapping;

namespace BankingUnitTests.BLLTests.BankingServiceTests
{
    [TestFixture]
    public class ProcessTransactionTests
    {
        private Mock<IUnitOfWork> _moqUnitOfWork;
        private Mock<BankingService.Bll.Service.BankingService> _moqBankingService;
        [SetUp]
        public void SetUp()
        {
            _moqUnitOfWork = new Mock<IUnitOfWork>();
            _moqBankingService = new Mock<BankingService.Bll.Service.BankingService>(_moqUnitOfWork.Object)
            {
                CallBase = true
            };

            //setup this function to call internal function.
            _moqUnitOfWork.Setup(x => x.RunInTransaction(It.IsAny<Func<Task>>())).Callback(async (Func<Task> function) =>
            {
                await function();
            });
        }

        [Test]
        public async Task MappingValidation_NoTransactions_ShouldFail()
        {
            //Arrange
            var account = new Account()
            {
                AccountId = 0,
                AccountName = "BOB",
                AccountTransactions = new List<AccountTransaction>()
            };

            //Act
            var result = await _moqBankingService.Object.ProcessTransaction(account);

            //Assert
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*no transaction*");
            _moqUnitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public async Task MappingValidation_FirstWithdrawal_ShouldFail()
        {
            //Arrange
            var account = new Account()
            {
                AccountId = 0,
                AccountName = "BOB",
                AccountTransactions = new List<AccountTransaction>()
                {
                    new AccountTransaction()
                    {
                        Amount = 999.99m,
                        AccountTransactionId = 0,
                        AccountId = 0,
                        Date = new DateOnly(1990, 01, 01),
                        Type = Shared.AccountTransactionType.Withdrawal
                    }
                }
            };
            //setup no return data in database            
            _moqUnitOfWork.Setup(x => x.AccountRepository.FindWithAccountTransactions(It.IsAny<Expression<Func<BankingService.Sql.Model.Account, bool>>>(),
                It.IsAny<bool>())).ReturnsAsync(() => new List<BankingService.Sql.Model.Account>
                {
                });

            //Act
            var result = await _moqBankingService.Object.ProcessTransaction(account);

            //Assert
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*below 0*");
            _moqUnitOfWork.Verify(x => x.RunInTransaction(It.IsAny<Func<Task>>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.AccountRepository.FindWithAccountTransactions(It.IsAny<Expression<Func<BankingService.Sql.Model.Account, bool>>>(),
                It.IsAny<bool>()), Times.Once);
            _moqUnitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public async Task MappingValidation_WithdrawTooMuch_ShouldFail()
        {
            //Arrange
            var account = new Account()
            {
                AccountId = 0,
                AccountName = "BOB",
                AccountTransactions = new List<AccountTransaction>()
                {
                    new AccountTransaction()
                    {
                        Amount = 999.99m,
                        AccountTransactionId = 0,
                        AccountId = 0,
                        Date = new DateOnly(1990, 01, 01),
                        Type = Shared.AccountTransactionType.Withdrawal
                    }
                }
            };
            //setup no return data in database            
            _moqUnitOfWork.Setup(x => x.AccountRepository.FindWithAccountTransactions(It.IsAny<Expression<Func<BankingService.Sql.Model.Account, bool>>>(),
                It.IsAny<bool>())).ReturnsAsync(() => new List<BankingService.Sql.Model.Account>
                {
                    new BankingService.Sql.Model.Account()
                    {
                        AccountId = 0,
                        AccountName = "BOB",
                        AccountTransactions = new List<BankingService.Sql.Model.AccountTransaction>
                        {
                            new BankingService.Sql.Model.AccountTransaction
                            {
                                Amount = 10.0m,
                                AccountTransactionId = 0,
                                AccountId = 0,
                                Date = new DateOnly(1990, 01, 01).ToString("yyyyMMdd"),
                                Type = Shared.AccountTransactionType.Deposit.ToString()
                            }
                        }
                    }
                });

            //Act
            var result = await _moqBankingService.Object.ProcessTransaction(account);

            //Assert
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*below 0*");
            _moqUnitOfWork.Verify(x => x.RunInTransaction(It.IsAny<Func<Task>>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.AccountRepository.FindWithAccountTransactions(It.IsAny<Expression<Func<BankingService.Sql.Model.Account, bool>>>(),
                It.IsAny<bool>()), Times.Once);
            _moqUnitOfWork.VerifyNoOtherCalls();
        }
    }
}
