using System.Text.Json;
using Moq;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Exception;
using Questao5.Domain.Interfaces;
using Xunit;

namespace Questao5.Tests
{
    public class GetCheckingAccountBalanceHandlerTest
    {
        private readonly Mock<ICheckingAccountRepository> _checkingAccountRepository;
        private readonly Mock<IMovementRepository> _movementRepository;
        private readonly Mock<IIdempotencyRepository> _idempotencyRepository;
        public GetCheckingAccountBalanceHandlerTest()
        {
            _checkingAccountRepository = new Mock<ICheckingAccountRepository>();
            _movementRepository = new Mock<IMovementRepository>();
            _idempotencyRepository = new Mock<IIdempotencyRepository>();
        }

        [Fact]
        public async Task CheckInvalidAccountWhenSendNotRegisteredAccount()
        {
            //Arrange
            var query = new GetCheckingAccountBalanceRequest(Guid.NewGuid().ToString());

            //Act
            var handler = new GetCheckingAccountBalanceHandler(_checkingAccountRepository.Object, _movementRepository.Object, _idempotencyRepository.Object);

            //Assert
            var exception = await Assert.ThrowsAsync<Domain.Exception.ApplicationException>(() => handler.Handle(query, CancellationToken.None));
            Assert.Equal(ApplicationErrorGenerator.INVALID_ACCOUNT, exception.ErrorType);
        }

        [Fact]
        public async Task CheckInactiveAccountWhenSendInactiveAccount()
        {
            //Arrange
            var query = new GetCheckingAccountBalanceRequest(Guid.NewGuid().ToString());

            _checkingAccountRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(new Domain.Entities.CheckingAccount(query.CheckingAccountId, 10, "Leandro", false));

            //Act
            var handler = new GetCheckingAccountBalanceHandler(_checkingAccountRepository.Object, _movementRepository.Object, _idempotencyRepository.Object);

            //Assert
            var exception = await Assert.ThrowsAsync<Domain.Exception.ApplicationException>(async () => await handler.Handle(query, CancellationToken.None));
            Assert.Equal(ApplicationErrorGenerator.INACTIVE_ACCOUNT, exception.ErrorType);
        }

        [Fact]
        public async Task CheckBalanceZeroWhenNotExistsMovements()
        {
            //Arrange
            var query = new GetCheckingAccountBalanceRequest(Guid.NewGuid().ToString());

            _checkingAccountRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(new Domain.Entities.CheckingAccount(query.CheckingAccountId, 10, "Leandro", true));

            //Act
            var response = await new GetCheckingAccountBalanceHandler(_checkingAccountRepository.Object, _movementRepository.Object, _idempotencyRepository.Object).Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(0, response.AccountBalance);
        }

        [Fact]
        public async Task CheckSumBalanceWhenExistsMovements(){
            //Arrange
            var guid = Guid.NewGuid().ToString();
            var query = new GetCheckingAccountBalanceRequest(guid);

            var checkingAccount = new Domain.Entities.CheckingAccount(query.CheckingAccountId, 10, "Leandro", true);
            _checkingAccountRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(checkingAccount);
            _movementRepository.Setup(x => x.GetMovementsByCheckingAccountId(It.IsAny<string>())).ReturnsAsync(new List<Domain.Entities.Movement>(){
                new Domain.Entities.Movement(Guid.NewGuid().ToString(), DateTime.UtcNow, TransactionType.Credit, 100, checkingAccount),
                new Domain.Entities.Movement(Guid.NewGuid().ToString(), DateTime.UtcNow, TransactionType.Debit, 50, checkingAccount) 
            });

            //Act
            var response = await new GetCheckingAccountBalanceHandler(_checkingAccountRepository.Object, _movementRepository.Object, _idempotencyRepository.Object).Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(50, response.AccountBalance);
        }

        [Fact]
        public async Task CheckIdempotencyWhenGetBalanceFromCheckingAccount(){
            //Arrange
            var checkingAccountGuid = Guid.NewGuid().ToString();
            var checkingAccountResponsible = "Leandro";
            var idempotencyGuid = Guid.NewGuid().ToString();
            var query = new GetCheckingAccountBalanceRequest(checkingAccountGuid, idempotencyGuid);
            
            _idempotencyRepository.Setup(x => x.GetIdempotency(It.IsAny<string>())).ReturnsAsync(new Domain.Entities.Idempotency(idempotencyGuid, JsonSerializer.Serialize(new GetCheckingAccountBalanceRequest(checkingAccountGuid, idempotencyGuid)), JsonSerializer.Serialize(new GetCheckingAccountBalanceResponse(checkingAccountGuid, checkingAccountResponsible, DateTime.UtcNow, 15))));

            //Act
            var response = await new GetCheckingAccountBalanceHandler(_checkingAccountRepository.Object, _movementRepository.Object, _idempotencyRepository.Object).Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(15, response.AccountBalance);
        }
    }
}
