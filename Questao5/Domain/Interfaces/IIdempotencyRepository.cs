using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces;

public interface IIdempotencyRepository
{
    Task<Idempotency> Create(Idempotency idempotency);
    Task<Idempotency> GetIdempotency(string idepotencyKey);
}
