using DataAccessDemo.Persistence.Services.Contracts;
using System.Data;

namespace DataAccessDemo.Persistence.Services
{
    public class MsSqlScriptService : IScriptService
    {
        private readonly BloggingContext _context;

        public MsSqlScriptService(BloggingContext context)
        {
            _context = context;
        }

        public IAsyncEnumerable<T> RunStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object> parameters, Func<IDataReader, T> projection)
        {
            throw new NotImplementedException();
        }

        public Task<T> RunJsonStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<T> RunJsonRowSetStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> RunQueryableStoredProcedure<T>(string storedProcedureName, Dictionary<string, object> parameters) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
