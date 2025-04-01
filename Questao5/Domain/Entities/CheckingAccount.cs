namespace Questao5.Domain.Entities;

public class CheckingAccount
{
    public string CheckingAccountId { get; private set; }
    public int Number { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
    public List<Movement>? Movements { get; set; }
    
    public CheckingAccount() { }
    public CheckingAccount(string checkingAccountId, int number, string name, bool active)
    {
        CheckingAccountId = checkingAccountId;
        Number = number;
        Name = name;
        Active = active;
    }
}
