using MediatR;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Services
{
    public class CheckingAccountService
    {
        private readonly IMediator _mediator;
        public CheckingAccountService(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Method to make a movement
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CheckingAccountMovementResponse> MakeMovement(Commands.Requests.CheckingAccountMovementRequest request)
        {
            return await _mediator.Send(request);
        }

        /// <summary>
        /// Method to get balance
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetCheckingAccountBalanceResponse> GetCheckingAccountBalance(GetCheckingAccountBalanceRequest request)
        {
            return await _mediator.Send(request);
        }
    }
}
