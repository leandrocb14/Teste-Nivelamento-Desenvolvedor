using System.Data;
using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Exception;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.Handlers
{
    public class CheckingAccountMovementHandler : IRequestHandler<CheckingAccountMovementRequest, CheckingAccountMovementResponse>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICheckingAccountRepository _checkingAccountRepository;
        private readonly IMovementRepository _movementrepository;
        private readonly IIdempotencyRepository _idempotencyRepository;
        public CheckingAccountMovementHandler(IDbConnection dbConnection, ICheckingAccountRepository checkingAccountRepository, IMovementRepository movementrepository, IIdempotencyRepository idempotencyRepository)
        {
            _dbConnection = dbConnection;
            _checkingAccountRepository = checkingAccountRepository;
            _movementrepository = movementrepository;
            _idempotencyRepository = idempotencyRepository;
        }

        public async Task<CheckingAccountMovementResponse> Handle(CheckingAccountMovementRequest request, CancellationToken cancellationToken)
        {
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                if (!string.IsNullOrEmpty(request.IdempotencyKey))
                {
                    var idempotency = await _idempotencyRepository.GetIdempotency(request.IdempotencyKey);
                    if (idempotency != null && !string.IsNullOrEmpty(idempotency.Result))
                        return JsonConvert.DeserializeObject<CheckingAccountMovementResponse>(idempotency.Result);
                }

                if (request.Value <= 0 || !Enum.IsDefined(typeof(TransactionType), request.TransactionType))
                    ApplicationErrorGenerator.Throw(ApplicationErrorGenerator.INVALID_VALUE);

                var checkingAccount = await _checkingAccountRepository.GetById(request.CheckingAccountId);

                if (checkingAccount == null)
                    ApplicationErrorGenerator.Throw(ApplicationErrorGenerator.INVALID_ACCOUNT);

                if (!checkingAccount.Active)
                    ApplicationErrorGenerator.Throw(ApplicationErrorGenerator.INACTIVE_ACCOUNT);

                var movement = new Domain.Entities.Movement(Guid.NewGuid().ToString(), DateTime.UtcNow, request.TransactionType, Math.Round(request.Value, 2), checkingAccount);
                var createdMovement = await _movementrepository.Create(movement);

                var result = new CheckingAccountMovementResponse(createdMovement.MovementId);
                if (!string.IsNullOrEmpty(request.IdempotencyKey))
                    await _idempotencyRepository.Create(new Domain.Entities.Idempotency(request.IdempotencyKey, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(result)));

                transaction.Commit();

                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }


        }
    }
}
