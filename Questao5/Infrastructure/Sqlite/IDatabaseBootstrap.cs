using System.Data;

namespace Questao5.Infrastructure.Sqlite
{
    public interface IDatabaseBootstrap
    {
        void Setup();
        IDbConnection GetConnection();
    }
}