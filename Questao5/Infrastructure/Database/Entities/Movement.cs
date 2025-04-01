namespace Questao5.Infrastructure.Database.Entities
{
    public class Movement
    {
        public string MovementId { get; private set; }
        public CheckingAccount CheckingAccount { get; set; }
        public DateTime MovementDate { get; private set; }
        public string TransactionType { get; private set; }
        public double Value { get; private set; }

        public Movement() { }

        public Movement(string movementId, string transactionType, DateTime movementDate, double value)
        {
            MovementId = movementId;
            TransactionType = transactionType;
            MovementDate = movementDate;
            Value = value;
        }
    }
}
