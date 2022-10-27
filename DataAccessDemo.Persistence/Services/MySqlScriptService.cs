using DataAccessDemo.Persistence.Services.Contracts;
using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using DataAccessDemo.Shared.Helpers;

namespace DataAccessDemo.Persistence.Services
{
    // TODO: Add method to use reflection
    // https://stackoverflow.com/questions/1464883/how-can-i-easily-convert-datareader-to-listt
    public class MySqlScriptService : IScriptService
    {
        private readonly ILogger<MySqlScriptService> _logger;
        private readonly string _connectionString;
        private readonly BloggingContext _dbContext;

        public MySqlScriptService(BloggingContext dbContext, ILogger<MySqlScriptService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _connectionString = dbContext.Database.GetConnectionString()!;
        }

        /// <summary>
        ///     Runs a stored procedure that returns a JSON string and deserializes it into the specified object.
        ///     The stored procedure must return a single row with a single column that contains the JSON string.
        /// </summary>
        public async Task<T> RunJsonStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object> parameters)
        {
            List<MySqlParameter> mySqlParameters = parameters.Select(p => new MySqlParameter(p.Key, p.Value)).ToList();
            T results;

            using (MySqlConnection connection = CreateSqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(mySqlParameters.ToArray());

                    await connection.OpenAsync();

                    StringBuilder jsonResult = new StringBuilder();
                    MySqlDataReader reader = command.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        if (typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                            jsonResult.Append("[]");
                        else
                            jsonResult.Append("");
                    }
                    else
                    {
                        while (await reader.ReadAsync())
                        {
                            jsonResult.Append(reader.GetValue(0).ToString());
                        }
                    }

                    results = JsonConvert.DeserializeObject<T>(jsonResult.ToString(), new DateOnlyJsonConverter())!;

                    await connection.CloseAsync();
                }
            }

            return results;
        }

        /// <summary>
        ///     Runs a stored procedure that returns a RowSet of JSON strings and deserializes them into the specified object.
        ///     The stored procedure can return one or more rows with a single column that contains the JSON string.
        /// </summary>
        public async IAsyncEnumerable<T> RunJsonRowSetStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object> parameters)
        {
            List<MySqlParameter> mySqlParameters = parameters.Select(p => new MySqlParameter(p.Key, p.Value)).ToList();

            using (MySqlConnection connection = CreateSqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(mySqlParameters.ToArray());

                    await connection.OpenAsync();

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            yield return JsonConvert.DeserializeObject<T>(
                                (reader.GetString(0).ToString()),
                                new DateOnlyJsonConverter())!;
                        }
                    }

                    await connection.CloseAsync();
                }
            }
        }

        /// <summary>
        ///     Runs a stored procedure that returns a rowset and projects each row into the specified object using the provided mapping.
        /// </summary>
        public async IAsyncEnumerable<T> RunStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object> parameters, Func<IDataReader, T> projection)
        {
            List<MySqlParameter> mySqlParameters = parameters.Select(p => new MySqlParameter(p.Key, p.Value)).ToList();

            using (MySqlConnection connection = CreateSqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(mySqlParameters.ToArray());

                    await connection.OpenAsync();

                    StringBuilder jsonResult = new StringBuilder();
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            yield return projection(reader);
                        }
                    }

                    await connection.CloseAsync();
                }
            }
        }

        /// <summary>
        ///     Runs a stored procedure that returns a rowset and returns an IQueryable that can be used to query the results.
        /// </summary>
        // TODO: Generic T constraint should be IdentityEntity but no reference to PromComm.Domain from PromComm.Persistence.
        public IQueryable<T> RunQueryableStoredProcedure<T>(string storedProcedureName, Dictionary<string, object> parameters) where T : class
        {
            List<MySqlConnector.MySqlParameter> mySqlParameters = parameters.Select(p => new MySqlConnector.MySqlParameter(p.Key, p.Value)).ToList();

            string sql = $"CALL {storedProcedureName}({string.Join(",", mySqlParameters.Select(x => $"@{x.ParameterName}"))})";
            return _dbContext.Set<T>().FromSqlRaw(sql, mySqlParameters.ToArray());
        }

        private MySqlConnection CreateSqlConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
