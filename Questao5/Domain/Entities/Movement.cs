using Questao5.Domain.Enumerators;

namespace Questao5.Domain.Entities
{
    public class Movement
    {
        public string MovementId { get; private set; }
        public CheckingAccount CheckingAccount { get; set; }
        public DateTime MovementDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public double Value { get; set; }

        public Movement() { }

        public Movement(string movementId, DateTime movementDate, TransactionType transactionType, double value, CheckingAccount checkingAccount)
        {
            MovementId = movementId;
            MovementDate = movementDate;
            TransactionType = transactionType;
            Value = value;
            CheckingAccount = checkingAccount;
        }

        public Movement(string movementId, DateTime movementDate, TransactionType transactionType, double value) : this(movementId, movementDate, transactionType, value, null)
        {

        }
    }
}
