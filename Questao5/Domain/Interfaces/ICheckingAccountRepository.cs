using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces
{
    public interface ICheckingAccountRepository
    {
        Task<CheckingAccount> GetById(string id);
    }
}
