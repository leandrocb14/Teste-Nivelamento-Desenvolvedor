using MediatR;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Queries.Requests
{
    public class GetCheckingAccountBalanceRequest : IRequest<GetCheckingAccountBalanceResponse>
    {
        public string CheckingAccountId { get; private set; }
        public string IdempotencyKey { get; private set; }

        public GetCheckingAccountBalanceRequest(string checkingAccountId) : this(checkingAccountId, null) { }

        public GetCheckingAccountBalanceRequest(string checkingAccountId, string idempotencyKey)
        {
            CheckingAccountId = checkingAccountId;
            IdempotencyKey = idempotencyKey;
        }
    }
}
