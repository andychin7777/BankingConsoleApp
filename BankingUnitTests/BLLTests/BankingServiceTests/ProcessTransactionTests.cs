using BankingService.Bll.Model;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using BankingService.Bll.Mapping;

namespace BankingUnitTests.BLLTests.BankingServiceTests
{
    [TestFixture]
    public class ProcessTransactionTests : BaseBankingServiceTest
    {
        [SetUp]
        public void SetUp()
        {            
        }

        [Test]
        public void ValidateCanRunTransaction_ShouldBeTrue()
        {
            //Arrange
            BankingService.Sql.Model.Account sqlDatabaseAccount = null;

            var account = new Account()
            {
                AccountTransactions = new List<AccountTransaction>
                {
                    new AccountTransaction()
                    {
                        Type = Shared.AccountTransactionType.Deposit,
                        Amount = 999.9m
                    }
                }
            };

            //Act
            var result = _moqBankingService.Object.ValidateCanRunTransaction(sqlDatabaseAccount, account);

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ValidateCanRunTransaction_ShouldBeFalse()
        {
            //Arrange
            BankingService.Sql.Model.Account sqlDatabaseAccount = null;

            var account = new Account()
            {
                AccountTransactions = new List<AccountTransaction>
                {
                    new AccountTransaction()
                    {
                        Type = Shared.AccountTransactionType.Withdrawal,
                        Amount = 999.9m
                    }
                }
            };

            //Act
            var result = _moqBankingService.Object.ValidateCanRunTransaction(sqlDatabaseAccount, account);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidateCanRunTransaction_Exact0_ShouldBe()
        {
            //Arrange
            BankingService.Sql.Model.Account sqlDatabaseAccount = new BankingService.Sql.Model.Account()
            {
                AccountTransactions = new List<BankingService.Sql.Model.AccountTransaction>
                {
                    new BankingService.Sql.Model.AccountTransaction()
                    {
                        Date = "19900101",
                        Type = Shared.AccountTransactionType.Deposit.ToString(),
                        Amount = 999.9m
                    }                    
                }
            };

            var account = new Account()
            {
                AccountTransactions = new List<AccountTransaction>
                {
                    new AccountTransaction()
                    {
                        Type = Shared.AccountTransactionType.Withdrawal,
                        Amount = 999.9m
                    }
                }
            };

            //Act
            var result = _moqBankingService.Object.ValidateCanRunTransaction(sqlDatabaseAccount, account);

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task ProcessTransaction_NullEntry_ThrowNullReferenceException()
        {
            //Arrange
            Account account = null;

            //Act
            var result = await _moqBankingService.Object.ProcessTransaction(account);

            //Assert
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*account is null*");
        }

        [Test]
        public async Task ProcessTransaction_NoTransactions_ShouldFail()
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
        public async Task ProcessTransaction_FirstWithdrawal_ShouldFail()
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
        public async Task ProcessTransaction_WithdrawTooMuch_ShouldFail()
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

        [Test]
        public async Task ProcessTransaction_Deposit_Succeed()
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
                        Type = Shared.AccountTransactionType.Deposit
                    }
                }
            };
            //setup no return data in database            
            _moqUnitOfWork.Setup(x => x.AccountRepository.FindWithAccountTransactions(It.IsAny<Expression<Func<BankingService.Sql.Model.Account, bool>>>(),
                It.IsAny<bool>())).ReturnsAsync(() => new List<BankingService.Sql.Model.Account>
                {
                });

            var returnAccount = new BankingService.Sql.Model.Account()
            {
                AccountId = 999,
                AccountTransactions = new List<BankingService.Sql.Model.AccountTransaction>()
                {
                    new BankingService.Sql.Model.AccountTransaction()
                    {
                        Amount = 999.99m,
                        AccountTransactionId = 0,
                        AccountId = 0,
                        Date = new DateOnly(1990, 01, 01).ToString("yyyyMMdd"),
                        Type = Shared.AccountTransactionType.Deposit.ToString()
                    }                    
                }
            };
            _moqUnitOfWork.Setup(x => x.AccountRepository.Add(It.IsAny<BankingService.Sql.Model.Account>())).ReturnsAsync(() => true)
                    .Callback((BankingService.Sql.Model.Account addAccount) => {
                        //set the accountId
                        addAccount.AccountId = returnAccount.AccountId;
                    });

            //setup only return the returnAccount if the returnAccountId is passed in.
            _moqUnitOfWork.Setup(x => x.AccountRepository.GetByIdWithAccountTransactions(returnAccount.AccountId)).ReturnsAsync(returnAccount);

            //Act
            var result = await _moqBankingService.Object.ProcessTransaction(account);

            //Assert
            result.Success.Should().BeTrue();
            result.Messages.Should().BeEmpty();
            _moqUnitOfWork.Verify(x => x.RunInTransaction(It.IsAny<Func<Task>>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.AccountRepository.FindWithAccountTransactions(It.IsAny<Expression<Func<BankingService.Sql.Model.Account, bool>>>(),
                It.IsAny<bool>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);

            _moqUnitOfWork.Verify(x => x.AccountRepository.Add(It.IsAny<BankingService.Sql.Model.Account>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.AccountRepository.GetByIdWithAccountTransactions(returnAccount.AccountId), Times.Once);
            _moqUnitOfWork.VerifyNoOtherCalls();

            result.Value.Should().BeEquivalentTo(returnAccount.MapToBllAccount());
        }
    }
}
