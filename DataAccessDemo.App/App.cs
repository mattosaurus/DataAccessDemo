using DataAccessDemo.Domain.Entities;
using DataAccessDemo.Persistence.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DataAccessDemo.App
{
    public class App
    {
        private readonly IConfigurationRoot _config;
        private readonly ILogger<App> _logger;
        private readonly IScriptService _scriptService;

        public App(IConfigurationRoot config, ILoggerFactory loggerFactory, IScriptService scriptService)
        {
            _logger = loggerFactory.CreateLogger<App>();
            _config = config;
            _scriptService = scriptService;
        }

        public async Task Run()
        {
            // Get a single object where the stored procedure returns a JSON string
            Post singlePost = await _scriptService.RunJsonStoredProcedureAsync<Post>("sp__get_single_post", new Dictionary<string, object>() { { "id", 1 } });

            // Get a list of objects where the stored procedure returns a JSON string row for each object
            List<Post> posts = new List<Post>();

            await foreach (Post post in _scriptService.RunJsonRowSetStoredProcedureAsync<Post>("sp__get_all_posts", new Dictionary<string, object>()))
            {
                posts.Add(post);
            }

            // Get a rowset and map each column to the object by position
            List<Post> mappedPosts = new List<Post>();
            await foreach (Post post in _scriptService.RunStoredProcedureAsync<Post>(
                "sp__get_all_posts",
                new Dictionary<string, object>(),
                (reader) =>
                {
                    return new Post()
                    {
                        PostId = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Content = reader.GetString(2),
                    };
                })
            )
            {
                mappedPosts.Add(post);
            }

            // Run a stored procedure and return the result as an IQueryable
            IQueryable<Post> queryablePosts = _scriptService.RunQueryableStoredProcedure<Post>("sp__get_all_posts", new Dictionary<string, object>());
        }
    }
}
