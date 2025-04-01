using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces;

public interface IMovementRepository
{
    Task<Movement> Create(Movement movement);
    Task<IEnumerable<Movement>> GetMovementsByCheckingAccountId(string checkingAccountId);
}
