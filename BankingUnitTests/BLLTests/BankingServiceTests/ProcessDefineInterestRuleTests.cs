using BankingService.Bll.Mapping;
using BankingService.Bll.Model;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BankingUnitTests.BLLTests.BankingServiceTests
{
    public class ProcessDefineInterestRuleTests : BaseBankingServiceTest
    {
        [Test]
        public async Task NullEntry_ThrowNullReferenceException()
        {
            //Arrange
            InterestRule? interestRule = null;

            //Act
            var result = await _moqBankingService.Object.ProcessDefineInterestRule(interestRule);

            //Assert
            result.Success.Should().BeFalse();
            result.Messages.Select(x => x.ToLowerInvariant()).Should().ContainMatch("*interest rule is null*");
        }

        [Test]
        public async Task NewRecord_ShouldSucceed()
        {
            //Arrange
            var interestRule = new InterestRule()
            {
                InterestRate = 99.9m,
                InterestRuleDateActive = new DateOnly(1990, 01, 01),
                InterestRuleName = "RuleName01"
            };

            _moqUnitOfWork.Setup(x => x.InterestRuleRepository.Find(It.IsAny<Expression<Func<BankingService.Sql.Model.InterestRule, bool>>>(),
                It.IsAny<bool>())).ReturnsAsync(() => new List<BankingService.Sql.Model.InterestRule>());
            _moqUnitOfWork.Setup(x => x.InterestRuleRepository.All(It.IsAny<bool>())).ReturnsAsync(() => new List<BankingService.Sql.Model.InterestRule>()
            {
                interestRule.MapToSqlInterestRule()
            });
            
            //Act
            var result = await _moqBankingService.Object.ProcessDefineInterestRule(interestRule);

            //Assert
            result.Success.Should().BeTrue();
            _moqUnitOfWork.Verify(x => x.RunInTransaction(It.IsAny<Func<Task>>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.InterestRuleRepository.Find(It.IsAny<Expression<Func<BankingService.Sql.Model.InterestRule, bool>>>(),
                It.IsAny<bool>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.InterestRuleRepository.Add(It.IsAny<BankingService.Sql.Model.InterestRule>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
            _moqUnitOfWork.Verify(x => x.InterestRuleRepository.All(It.IsAny<bool>()), Times.Once);
            _moqUnitOfWork.VerifyNoOtherCalls();
            result.Value.Should().BeEquivalentTo(new List<InterestRule> { interestRule });
        }

        [Test]
        public async Task DeleteExistingRecordAndWrite_ShouldSucceed()
        {
            //Arrange
            var interestRule = new InterestRule()
            {
                InterestRate = 99.9m,
                InterestRuleDateActive = new DateOnly(1990, 01, 01),
                InterestRuleName = "RuleName01"
            };

            _moqUnitOfWork.Setup(x => x.InterestRuleRepository.Find(It.IsAny<Expression<Func<BankingService.Sql.Model.InterestRule, bool>>>(),
                It.IsAny<bool>())).ReturnsAsync(() => new List<BankingService.Sql.Model.InterestRule>()
                {
                    new BankingService.Sql.Model.InterestRule
                    {
                        InterestRuleId = 999
                    }
                });
            _moqUnitOfWork.Setup(x => x.InterestRuleRepository.All(It.IsAny<bool>())).ReturnsAsync(() => new List<BankingService.Sql.Model.InterestRule>()
            {
                interestRule.MapToSqlInterestRule()
            });            

            //Act
            var result = await _moqBankingService.Object.ProcessDefineInterestRule(interestRule);

            //Assert
            result.Success.Should().BeTrue();
            _moqUnitOfWork.Verify(x => x.RunInTransaction(It.IsAny<Func<Task>>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.InterestRuleRepository.Find(It.IsAny<Expression<Func<BankingService.Sql.Model.InterestRule, bool>>>(),
                It.IsAny<bool>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.InterestRuleRepository.Add(It.IsAny<BankingService.Sql.Model.InterestRule>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
            _moqUnitOfWork.Verify(x => x.InterestRuleRepository.All(It.IsAny<bool>()), Times.Once);
            _moqUnitOfWork.Verify(x => x.InterestRuleRepository.Delete(999), Times.Once);
            _moqUnitOfWork.VerifyNoOtherCalls();
            result.Value.Should().BeEquivalentTo(new List<InterestRule> { interestRule });
        }
    }
}
