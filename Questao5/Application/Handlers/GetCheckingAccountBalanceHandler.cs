using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Exception;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.Handlers
{
    public class GetCheckingAccountBalanceHandler : IRequestHandler<GetCheckingAccountBalanceRequest, GetCheckingAccountBalanceResponse>
    {
        private readonly ICheckingAccountRepository _checkingAccountRepository;
        private readonly IMovementRepository _movementRepository;
        private readonly IIdempotencyRepository _idempotencyRepository;

        public GetCheckingAccountBalanceHandler(ICheckingAccountRepository checkingAccountRepository, IMovementRepository movementrepository, IIdempotencyRepository idempotencyRepository)
        {
            _checkingAccountRepository = checkingAccountRepository;
            _movementRepository = movementrepository;
            _idempotencyRepository = idempotencyRepository;
        }

        public async Task<GetCheckingAccountBalanceResponse> Handle(GetCheckingAccountBalanceRequest request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.IdempotencyKey))
            {
                var idempotency = await _idempotencyRepository.GetIdempotency(request.IdempotencyKey);
                if (idempotency != null && !string.IsNullOrEmpty(idempotency.Result))
                    return JsonConvert.DeserializeObject<GetCheckingAccountBalanceResponse>(idempotency.Result);
            }

            var checkingAccount = await _checkingAccountRepository.GetById(request.CheckingAccountId);

            if (checkingAccount == null)
                ApplicationErrorGenerator.Throw(ApplicationErrorGenerator.INVALID_ACCOUNT);

            if (!checkingAccount.Active)
                ApplicationErrorGenerator.Throw(ApplicationErrorGenerator.INACTIVE_ACCOUNT);

            var movementsAccount = await _movementRepository.GetMovementsByCheckingAccountId(checkingAccount.CheckingAccountId);

            var balance = movementsAccount?.Sum(movement =>
                 movement.TransactionType == Domain.Enumerators.TransactionType.Credit ? movement.Value : movement.Value * -1
            ) ?? 0;
            var result = new GetCheckingAccountBalanceResponse(checkingAccount.CheckingAccountId, checkingAccount.Name, DateTime.UtcNow, balance);

            if (!string.IsNullOrEmpty(request.IdempotencyKey))
                await _idempotencyRepository.Create(new Domain.Entities.Idempotency(request.IdempotencyKey, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(result)));

            return result;
        }
    }
}
