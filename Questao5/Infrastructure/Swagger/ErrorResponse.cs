using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Questao5.Infrastructure.Swagger
{
    public class ErrorResponse
    {
        [Required]
        [Description("Error type")]
        public string ErrorType { get; private set; }
        [Required]
        [Description("Error message")]
        public string Message { get; private set; }

        public ErrorResponse(string errorType, string message)
        {
            ErrorType = errorType;
            Message = message;
        }
    }
}
