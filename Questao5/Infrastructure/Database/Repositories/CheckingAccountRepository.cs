using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using Dapper;
using System.Data;

namespace Questao5.Infrastructure.Database.Repositories
{
    public class CheckingAccountRepository : ICheckingAccountRepository
    {
        private readonly IDbConnection _connection;

        public CheckingAccountRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<CheckingAccount> GetById(string id)
        {
            var sql = @"
                    SELECT 
                        contacorrente.idcontacorrente as CheckingAccountId, 
                        contacorrente.numero as Number, 
                        contacorrente.nome as Name, 
                        contacorrente.ativo as Active
                    FROM contacorrente 
                    WHERE contacorrente.idcontacorrente = @Id AND contacorrente.ativo = 1";

            var result = await _connection.QueryFirstOrDefaultAsync<Entities.CheckingAccount>(
                sql,
                new { Id = id }
            );

            return result != null ? new CheckingAccount(result.CheckingAccountId, result.Number, result.Name, result.Active) : null;
        }
    }
}
