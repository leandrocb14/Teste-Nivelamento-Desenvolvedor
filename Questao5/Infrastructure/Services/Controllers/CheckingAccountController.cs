using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Application.Services;
using Questao5.Infrastructure.Constants;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckingAccountController : ControllerBase
    {
        private readonly CheckingAccountService _accountService;

        public CheckingAccountController(CheckingAccountService accountService)
        {
            this._accountService = accountService;
        }

        [HttpGet("{checkingAccountId}/balance")]
        public async Task<GetCheckingAccountBalanceResponse> GetBalance(string checkingAccountId)
        {
            var headerValue = HttpContext.Request.Headers.TryGetValue(HeaderConstants.IDEMPOTENCY_KEY, out var value) ? value.ToString() : null;
            return await _accountService.GetCheckingAccountBalance(new GetCheckingAccountBalanceRequest(checkingAccountId, headerValue));
        }

        [HttpPost("move")]
        public async Task<CheckingAccountMovementResponse> Move(CheckingAccountMovementRequest request)
        {
            request.IdempotencyKey = HttpContext.Request.Headers.TryGetValue(HeaderConstants.IDEMPOTENCY_KEY, out var value) ? value.ToString() : null;
            return await _accountService.MakeMovement(request);
        }
    }
}
