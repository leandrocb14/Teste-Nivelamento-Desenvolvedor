using System.Data;
using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;

namespace Questao5.Infrastructure.Database.Repositories;

public class IdempotencyRepository : IIdempotencyRepository
{
    private readonly IDbConnection _connection;

    public IdempotencyRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Idempotency> Create(Idempotency idempotency)
    {
        var sql = @"INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@IdempotencyKey, @Request, @Result)";

        await _connection.ExecuteAsync(sql, new
        {
            IdempotencyKey = idempotency.Key,
            Request = idempotency.Request,
            Result = idempotency.Result
        });

        return idempotency;
    }

    public async Task<Idempotency> GetIdempotency(string idempotencyKey)
    {
        var sql = @"SELECT chave_idempotencia as Key, requisicao as Request, resultado as Result FROM idempotencia WHERE chave_idempotencia = @IdempotencyKey";

        var idempotencyDB = await _connection.QueryFirstOrDefaultAsync<Entities.Idempotency>(sql, new { IdempotencyKey = idempotencyKey });

        return idempotencyDB != null ? new Idempotency(idempotencyDB.Key, idempotencyDB.Request, idempotencyDB.Result) : null;
    }
}
