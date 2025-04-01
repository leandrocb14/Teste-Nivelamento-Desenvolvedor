using System.Data;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using Dapper;
using Questao5.Domain.Enumerators;

namespace Questao5.Infrastructure.Database.Repositorie
{
    public class MovementRepository : IMovementRepository
    {
        private readonly IDbConnection _connection;

        public MovementRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Movement> Create(Movement movement)
        {
            var query = @"INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) 
              VALUES (@Id, @CheckingAccountId, @MovementDate, @TransactionType, @Value)";

            await _connection.ExecuteAsync(query, new
            {
                Id = movement.MovementId,
                CheckingAccountId = movement.CheckingAccount.CheckingAccountId,
                MovementDate = movement.MovementDate,
                TransactionType = movement.TransactionType.GetEnumMemberValue(),
                Value = movement.Value
            });

            return movement;
        }

        public async Task<IEnumerable<Movement>> GetMovementsByCheckingAccountId(string checkingAccountId)
        {
            var sql = @"SELECT 
                            movimento.idmovimento as MovementId, 
                            movimento.datamovimento as MovementDate, 
                            movimento.tipomovimento as TransactionType, 
                            movimento.valor as Value 
                        FROM movimento 
                        WHERE movimento.idcontacorrente = @CheckingAccountId";

            var result = await _connection.QueryAsync<Entities.Movement>(sql, new { CheckingAccountId = checkingAccountId });

            return result != null && result.Count() > 0 ? Enumerable.Select(result, m =>
                new Movement(m.MovementId, m.MovementDate, m.TransactionType.ToEnum<TransactionType>(), m.Value)
            ) : null;
        }
    }
}
