using BankingConsoleApp.Mapping;
using BankingService.Bll.Model;
using FluentAssertions;

namespace BankingUnitTests.BankingConsoleAppTests
{
    [TestFixture]
    public class MappingToInterestRuleTests
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
            var result = BankActionValidateMapper.MapStringToInterestRuleAndTransaction(inputString);

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
            var result = BankActionValidateMapper.MapStringToInterestRuleAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*blank*");
        }

        [Test]
        public void MappingValidation_ShouldFail_MismatchParams()
        {
            //Arrange
            string? inputString = "19900101 RULE01 9.99 111";

            //Act
            var result = BankActionValidateMapper.MapStringToInterestRuleAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*invalid*");
        }

        [Test]
        public void MappingValidation_ShouldFail_InvalidDate()
        {
            //Arrange
            string? inputString = "19900101h RULE01 9.99";

            //Act
            var result = BankActionValidateMapper.MapStringToInterestRuleAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*invalid date*");
        }

        [Test]
        public void MappingValidation_ShouldFail_InvalidAmountTooBig()
        {
            //Arrange
            string? inputString = "19900101 RULE01 100";

            //Act
            var result = BankActionValidateMapper.MapStringToInterestRuleAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*rate must be between*");
        }

        [Test]
        public void MappingValidation_ShouldFail_InvalidAmountTooSmall()
        {
            //Arrange
            string? inputString = "19900101 RULE01 0";

            //Act
            var result = BankActionValidateMapper.MapStringToInterestRuleAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*rate must be between*");
        }

        [Test]
        public void MappingValidation_ShouldSucceed()
        {
            //Arrange
            string? inputString = "19900101 RULE01 1.2";

            //Act
            var result = BankActionValidateMapper.MapStringToInterestRuleAndTransaction(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Messages.Should().BeEmpty();

            result.Value.Should().BeEquivalentTo(new InterestRule()
            {
                InterestRate = 1.2m,
                InterestRuleDateActive = new DateOnly(1990, 01, 01),
                InterestRuleName = "RULE01"
            });
        }
    }
}
