using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Enumerators;

namespace Questao5.Application.Commands.Requests
{
    public class CheckingAccountMovementRequest : IRequest<CheckingAccountMovementResponse>
    {
        [Required]
        [StringLength(37)]
        [Description("Checking Account Id")]
        public string CheckingAccountId { get; private set; }
        [Required]
        [Description("Transaction Type")]
        public TransactionType TransactionType { get; private set; }
        [Required]
        [Description("Value of movement")]
        public double Value { get; private set; }
        [JsonIgnore]
        public string? IdempotencyKey { get; set; }

        public CheckingAccountMovementRequest(string checkingAccountId, TransactionType transactionType, double value)
        {
            CheckingAccountId = checkingAccountId;
            TransactionType = transactionType;
            Value = value;
        }
    }
}
