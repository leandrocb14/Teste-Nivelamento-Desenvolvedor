namespace Questao5.Domain.Exception
{
    public class ApplicationException : System.Exception
    {
        public string ErrorType { get; set; }
        public ApplicationException(string errorType, string message) : base(message)
        {
            ErrorType = errorType;
        }
    }
}
