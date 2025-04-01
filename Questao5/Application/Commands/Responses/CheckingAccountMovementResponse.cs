using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Questao5.Application.Commands.Responses
{
    public class CheckingAccountMovementResponse
    {
        [Required]
        [StringLength(37)]
        [Description("Movement ID")]
        public string MovementId { get; private set; }
        public CheckingAccountMovementResponse(string movementId)
        {
            MovementId = movementId;
        }
    }
}
