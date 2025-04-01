namespace Questao5.Infrastructure.Database.Entities
{
    public class CheckingAccount
    {
        public string CheckingAccountId { get; private set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public List<Movement>? Movements { get; set; }
    }
}
