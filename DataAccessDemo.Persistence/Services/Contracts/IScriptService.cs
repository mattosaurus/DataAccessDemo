using System.Data;

namespace DataAccessDemo.Persistence.Services.Contracts
{
    public interface IScriptService
    {
        IAsyncEnumerable<T> RunStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object> parameters, Func<IDataReader, T> projection);
        Task<T> RunJsonStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object> parameters);
        IAsyncEnumerable<T> RunJsonRowSetStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object> parameters);
        IQueryable<T> RunQueryableStoredProcedure<T>(string storedProcedureName, Dictionary<string, object> parameters) where T : class;
    }
}
