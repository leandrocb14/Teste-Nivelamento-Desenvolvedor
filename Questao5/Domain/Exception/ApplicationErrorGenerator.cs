namespace Questao5.Domain.Exception
{
    public static class ApplicationErrorGenerator
    {
        public const string INTERNAL_ERROR = "INTERNAL_ERROR";
        public const string INVALID_ACCOUNT = "INVALID_ACCOUNT";
        public const string INACTIVE_ACCOUNT = "INACTIVE_ACCOUNT";
        public const string INVALID_VALUE = "INVALID_VALUE";
        public const string INVALID_TYPE = "INVALID_TYPE";

        public static void Throw(string errorType)
        {
            var message = errorType switch
            {
                INTERNAL_ERROR => "A internal error occurred",
                INVALID_ACCOUNT => "Invalid account",
                INACTIVE_ACCOUNT => "Inactive account",
                INVALID_VALUE => "Invalid value",
                INVALID_TYPE => "Invalid type",
                _ => "Unknown error"
            };

            throw new ApplicationException(errorType, message);
        }
    }
}
