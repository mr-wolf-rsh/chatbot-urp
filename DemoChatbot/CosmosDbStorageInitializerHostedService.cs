using Microsoft.Azure.Cosmos;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace DemoChatbot
{
    public class CosmosDbStorageInitializerHostedService : IHostedService
    {
        readonly CosmosDbPartitionedStorageOptions _conversationStateOptions;
        readonly CosmosDbPartitionedStorageOptions _privateConversationStateOptions;

        readonly int _cosmosDbConversationStateTimeToLive;
        readonly int _cosmosDbPrivateConversationStateTimeToLive;

        public CosmosDbStorageInitializerHostedService(IConfiguration configuration)
        {
            _conversationStateOptions = new CosmosDbPartitionedStorageOptions()
            {
                CosmosDbEndpoint = configuration["CosmosDbEndpoint"],
                AuthKey = configuration["CosmosDbAuthKey"],
                DatabaseId = configuration["CosmosDbDatabaseId"],
                ContainerId = configuration["CosmosDbConversationStateContainerId"]
            };

            _privateConversationStateOptions = new CosmosDbPartitionedStorageOptions()
            {
                CosmosDbEndpoint = configuration["CosmosDbEndpoint"],
                AuthKey = configuration["CosmosDbAuthKey"],
                DatabaseId = configuration["CosmosDbDatabaseId"],
                ContainerId = configuration["CosmosDbPrivateConversationStateContainerId"]
            };

            _cosmosDbConversationStateTimeToLive = configuration.GetValue<int>("CosmosDbConversationStateTimeToLive");
            _cosmosDbPrivateConversationStateTimeToLive = configuration.GetValue<int>("CosmosDbPrivateConversationStateTimeToLive");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var conversationStateClient = new CosmosClient(
                _conversationStateOptions.CosmosDbEndpoint,
                _conversationStateOptions.AuthKey,
                _conversationStateOptions.CosmosClientOptions ?? new CosmosClientOptions());

            // Create the contaier with the provided TTL
            var conversationStateContainerResponse = await conversationStateClient
                .GetDatabase(_conversationStateOptions.DatabaseId)
                .DefineContainer(_conversationStateOptions.ContainerId, "/id")
                .WithDefaultTimeToLive(_cosmosDbConversationStateTimeToLive)
                .WithIndexingPolicy().WithAutomaticIndexing(false).Attach()
                .CreateIfNotExistsAsync(_conversationStateOptions.ContainerThroughput)
                .ConfigureAwait(false);

            using var privateConversationStateClient = new CosmosClient(
                _privateConversationStateOptions.CosmosDbEndpoint,
                _privateConversationStateOptions.AuthKey,
                _privateConversationStateOptions.CosmosClientOptions ?? new CosmosClientOptions());

            // Create the contaier with the provided TTL
            var privateConversationStateContainerResponse = await privateConversationStateClient
                .GetDatabase(_privateConversationStateOptions.DatabaseId)
                .DefineContainer(_privateConversationStateOptions.ContainerId, "/id")
                .WithDefaultTimeToLive(_cosmosDbPrivateConversationStateTimeToLive)
                .WithIndexingPolicy().WithAutomaticIndexing(false).Attach()
                .CreateIfNotExistsAsync(_privateConversationStateOptions.ContainerThroughput)
                .ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
