using BankingConsoleApp.Mapping;
using FluentAssertions;

namespace BankingUnitTests.BankingConsoleAppTests
{
    [TestFixture]
    public class MappingToTupleNameAndDateStartTests
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
            var result = BankActionValidateMapper.MapStringToTupleNameAndStartOfMonth(inputString);

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
            var result = BankActionValidateMapper.MapStringToTupleNameAndStartOfMonth(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*blank*");
        }

        [Test]
        public void MappingValidation_ShouldFail_InvalidParams()
        {
            //Arrange
            string? inputString = "BOB01";

            //Act
            var result = BankActionValidateMapper.MapStringToTupleNameAndStartOfMonth(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*invalid*");
        }

        [Test]
        public void MappingValidation_ShouldFail_InvalidYearMonth()
        {
            //Arrange
            string? inputString = "BOB01 19999999";

            //Act
            var result = BankActionValidateMapper.MapStringToTupleNameAndStartOfMonth(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*invalid date*");
        }

        [Test]
        public void MappingValidation_ShouldSucceed()
        {
            //Arrange
            string? inputString = "BOB01 199001";

            //Act
            var result = BankActionValidateMapper.MapStringToTupleNameAndStartOfMonth(inputString);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Messages.Should().BeEmpty();
            result.Value.Should().BeEquivalentTo(("BOB01", new DateOnly(1990, 01, 01)));
            
        }
    }
}
