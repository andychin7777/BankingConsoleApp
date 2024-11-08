using BankingConsoleApp.Mapping;
using BankingService.Bll.Model;
using FluentAssertions;

namespace BankingUnitTests.BankingConsoleAppTests
{
    [TestFixture]
    public class MappingToAccountAndTransactionTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void MappingValidation_ShouldFail_BlankString()
        {
            //Arrange
            var inputString = "";

            //Act
            var result = BankActionValidateMapper.MapStringToAccountAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*blank*");
        }

        [Test]
        public void MappingValidation_ShouldFail_NullString()
        {
            //Arrange
            string? inputString = null;

            //Act
            var result = BankActionValidateMapper.MapStringToAccountAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*blank*");
        }

        [Test]
        public void MappingValidation_ShouldFail_MismatchParams()
        {
            //Arrange
            string? inputString = "19900101 BOB1 W 9999 999";

            //Act
            var result = BankActionValidateMapper.MapStringToAccountAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*invalid*");
        }

        [Test]
        public void MappingValidation_ShouldFail_InvalidDate()
        {
            //Arrange
            string? inputString = "19900101f BOB1 W 9999";

            //Act
            var result = BankActionValidateMapper.MapStringToAccountAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*invalid date*");
        }

        [Test]
        public void MappingValidation_ShouldFail_InvalidType()
        {
            //Arrange
            string? inputString = "19900101 BOB1 X 9999";

            //Act
            var result = BankActionValidateMapper.MapStringToAccountAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*invalid type*");
        }

        [Test]
        public void MappingValidation_ShouldFail_InvalidAmount()
        {
            //Arrange
            string? inputString = "19900101 BOB1 D 999.994547";

            //Act
            var result = BankActionValidateMapper.MapStringToAccountAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*invalid amount*");
        }

        [Test]
        public void MappingValidation_ShouldSucceed()
        {
            //Arrange
            string? inputString = "19900101 BOB1 D 999.99";

            //Act
            var result = BankActionValidateMapper.MapStringToAccountAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Messages.Should().BeEmpty();

            result.Value.Should().BeEquivalentTo(new Account()
            {
                AccountId = 0,
                AccountName = "BOB1",
                AccountTransactions = new List<AccountTransaction>
                {
                    new AccountTransaction()
                    {
                        Amount = 999.99m,
                        Date = new DateOnly(1990, 01, 01),
                        Type = Shared.AccountTransactionType.Deposit
                    }
                }
            });
        }
    }
}
