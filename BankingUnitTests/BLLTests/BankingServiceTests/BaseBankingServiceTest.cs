using BankingService.Dal.Interfaces;
using Moq;

namespace BankingUnitTests.BLLTests.BankingServiceTests
{
    [TestFixture]
    public class BaseBankingServiceTest
    {
        internal Mock<IUnitOfWork> _moqUnitOfWork;
        internal Mock<BankingService.Bll.Service.BankingService> _moqBankingService;
        [SetUp]
        public void SetUp()
        {
            _moqUnitOfWork = new Mock<IUnitOfWork>();
            _moqBankingService = new Mock<BankingService.Bll.Service.BankingService>(_moqUnitOfWork.Object)
            {
                CallBase = true
            };

            //setup here requires the async function be returned correctly in the return function of MOQ otherwise using callback will
            //cause the unit test to no run fully if there are exception in the inner function
            _moqUnitOfWork.Setup(x => x.RunInTransaction(It.IsAny<Func<Task>>())).Returns(async (Func<Task> function) =>
            {
                await function();
            });
        }
    }
}
