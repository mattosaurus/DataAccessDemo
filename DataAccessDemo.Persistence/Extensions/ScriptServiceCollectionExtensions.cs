using DataAccessDemo.Persistence.Models.Options;
using DataAccessDemo.Persistence.Services;
using DataAccessDemo.Persistence.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessDemo.Persistence.Extensions
{
    public static class ScriptServiceCollectionExtensions
    {
        public static IServiceCollection AddMySqlScriptService(this IServiceCollection collection, Action<ScriptOptions> setupAction)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            collection.Configure(setupAction);
            return collection.AddSingleton<IScriptService, MySqlScriptService>();
        }

        public static IServiceCollection AddMySqlScriptService(this IServiceCollection collection, string connectionString)
        {
            return AddMySqlScriptService(collection, builder => {
                builder.ConnectionString = connectionString;
            });
        }

        public static IServiceCollection AddMySqlScriptService(this IServiceCollection collection)
        {
            return AddMySqlScriptService(collection, builder => { });
        }
    }
}
