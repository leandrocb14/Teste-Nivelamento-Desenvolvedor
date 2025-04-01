using System.Data;
using System.Text.Json;
using Moq;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Exception;
using Questao5.Domain.Interfaces;
using Xunit;

namespace Questao5.Tests
{
    public class MovimentationCommandHandlerTest
    {
        private readonly Mock<IDbConnection> _dbConnection;
        private readonly Mock<ICheckingAccountRepository> _checkingAccountRepository;
        private readonly Mock<IMovementRepository> _movementRepository;
        private readonly Mock<IIdempotencyRepository> _idempotencyRepository;
        public MovimentationCommandHandlerTest()
        {
            _dbConnection = new Mock<IDbConnection>();
            _checkingAccountRepository = new Mock<ICheckingAccountRepository>();
            _movementRepository = new Mock<IMovementRepository>();
            _idempotencyRepository = new Mock<IIdempotencyRepository>();
        }

        [Fact]
        public async Task CheckInvalidValueWhenSendNegativeValue()
        {
            //Arrange
            var command = new CheckingAccountMovementRequest(Guid.NewGuid().ToString(), TransactionType.Credit, -20);

            var transactionMock = new Mock<IDbTransaction>();
            _dbConnection.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);

            //Act
            var handler = new CheckingAccountMovementHandler(_dbConnection.Object, _checkingAccountRepository.Object, _movementRepository.Object, _idempotencyRepository.Object);

            //Assert
            var exception = await Assert.ThrowsAsync<Domain.Exception.ApplicationException>(async () => await handler.Handle(command, CancellationToken.None));
            Assert.Equal(ApplicationErrorGenerator.INVALID_VALUE, exception.ErrorType);
        }

        [Fact]
        public async Task CheckInvalidAccountWhenSendNotRegisteredAccount()
        {
            //Arrange
            var command = new CheckingAccountMovementRequest(Guid.NewGuid().ToString(), TransactionType.Credit, 20);

            var transactionMock = new Mock<IDbTransaction>();
            _dbConnection.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);

            //Act
            var handler = new CheckingAccountMovementHandler(_dbConnection.Object, _checkingAccountRepository.Object, _movementRepository.Object, _idempotencyRepository.Object);

            //Assert
            var exception = await Assert.ThrowsAsync<Domain.Exception.ApplicationException>(async () => await handler.Handle(command, CancellationToken.None));
            Assert.Equal(ApplicationErrorGenerator.INVALID_ACCOUNT, exception.ErrorType);
        }

        [Fact]
        public async Task CheckInactiveAccountWhenSendInactiveAccount()
        {
            //Arrange
            var guid = Guid.NewGuid().ToString();
            var command = new CheckingAccountMovementRequest(guid, TransactionType.Credit, 20);

            var transactionMock = new Mock<IDbTransaction>();
            _dbConnection.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
            _checkingAccountRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(new Domain.Entities.CheckingAccount(guid, 10, "Leandro", false));

            //Act
            var handler = new CheckingAccountMovementHandler(_dbConnection.Object, _checkingAccountRepository.Object, _movementRepository.Object, _idempotencyRepository.Object);

            //Assert
            var exception = await Assert.ThrowsAsync<Domain.Exception.ApplicationException>(async () => await handler.Handle(command, CancellationToken.None));
            Assert.Equal(ApplicationErrorGenerator.INACTIVE_ACCOUNT, exception.ErrorType);
        }

        [Fact]
        public async Task CheckCreateMovement()
        {
            //Arrange
            var checkingAccountGuid = Guid.NewGuid().ToString();
            var transactionType = TransactionType.Credit;
            var transactionValue = 20;
            var command = new CheckingAccountMovementRequest(checkingAccountGuid, transactionType, transactionValue);

            var transactionMock = new Mock<IDbTransaction>();
            _dbConnection.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
            var guidMovement = Guid.NewGuid().ToString();
            var checkingAccount = new CheckingAccount(checkingAccountGuid, 10, "Leandro", true);
            _checkingAccountRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(checkingAccount);
            _movementRepository.Setup(x => x.Create(It.IsAny<Movement>())).ReturnsAsync(new Movement(guidMovement, DateTime.UtcNow, transactionType, transactionValue, checkingAccount));

            //Act
            var response = await new CheckingAccountMovementHandler(_dbConnection.Object, _checkingAccountRepository.Object, _movementRepository.Object, _idempotencyRepository.Object).Handle(command, CancellationToken.None);

            //Assert
            Assert.Equal(guidMovement, response.MovementId);
        }

        [Fact]
        public async Task CheckIdempotencyWhenMovimentationCommand()
        {
            //Arrange
            var checkingAccountGuid = Guid.NewGuid().ToString();
            var transactionType = TransactionType.Credit;
            var transactionValue = 20;
            var oldCommand = new CheckingAccountMovementRequest(checkingAccountGuid, transactionType, transactionValue);
            var movementId = Guid.NewGuid().ToString();
            var newCommand = new CheckingAccountMovementRequest(checkingAccountGuid, transactionType, transactionValue);
            var idempotencyKey = Guid.NewGuid().ToString();
            newCommand.IdempotencyKey = idempotencyKey;

            var transactionMock = new Mock<IDbTransaction>();
            _dbConnection.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
            _idempotencyRepository.Setup(x => x.GetIdempotency(It.IsAny<string>())).ReturnsAsync(new Idempotency(idempotencyKey, JsonSerializer.Serialize(oldCommand), JsonSerializer.Serialize(new CheckingAccountMovementResponse(movementId))));

            //Act
            var response = await new CheckingAccountMovementHandler(_dbConnection.Object, _checkingAccountRepository.Object, _movementRepository.Object, _idempotencyRepository.Object).Handle(newCommand, CancellationToken.None);

            //Assert
            Assert.Equal(movementId, response.MovementId);
        }
    }
}


