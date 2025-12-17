// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using DemoChatbot.Bots;
using DemoChatbot.Dialogs;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoChatbot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Add Application Insights services into service collection
            services.AddApplicationInsightsTelemetry();

            // Create the telemetry client.
            services.AddSingleton<IBotTelemetryClient, BotTelemetryClient>();

            // Add telemetry initializer that will set the correlation context for all telemetry items.
            services.AddSingleton<ITelemetryInitializer, OperationCorrelationTelemetryInitializer>();

            // Add telemetry initializer that sets the user ID and session ID (in addition to other bot-specific properties such as activity ID)
            services.AddSingleton<ITelemetryInitializer, TelemetryBotIdInitializer>();

            // Create the telemetry middleware to initialize telemetry gathering
            services.AddSingleton<TelemetryInitializerMiddleware>();

            // Create the telemetry middleware (used by the telemetry initializer) to track conversation events
            services.AddSingleton(sp =>
            {
                var telemetryClient = sp.GetService<IBotTelemetryClient>();
                return new TelemetryLoggerMiddleware(telemetryClient, logPersonalInformation: true);
            });

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            //services.AddSingleton<IStorage, MemoryStorage>();

            // Add the Initializer as a HostedService (so it is called during the app service startup)
            services.AddHostedService<CosmosDbStorageInitializerHostedService>();

            // Create the storage options for User state
            var userStorageOptions = new CosmosDbPartitionedStorageOptions()
            {
                CosmosDbEndpoint = Configuration["CosmosDbEndpoint"],
                AuthKey = Configuration["CosmosDbAuthKey"],
                DatabaseId = Configuration["CosmosDbDatabaseId"],
                ContainerId = Configuration["CosmosDbUserStateContainerId"],
                CompatibilityMode = false
            };

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton(new UserState(new CosmosDbPartitionedStorage(userStorageOptions)));

            // Create the storage options for Conversation state
            var conversationStorageOptions = new CosmosDbPartitionedStorageOptions()
            {
                CosmosDbEndpoint = Configuration["CosmosDbEndpoint"],
                AuthKey = Configuration["CosmosDbAuthKey"],
                DatabaseId = Configuration["CosmosDbDatabaseId"],
                ContainerId = Configuration["CosmosDbConversationStateContainerId"],
                CompatibilityMode = false
            };

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton(new ConversationState(new CosmosDbPartitionedStorage(conversationStorageOptions)));

            // Create the storage options for Private Conversation state
            var privateConversationStorageOptions = new CosmosDbPartitionedStorageOptions()
            {
                CosmosDbEndpoint = Configuration["CosmosDbEndpoint"],
                AuthKey = Configuration["CosmosDbAuthKey"],
                DatabaseId = Configuration["CosmosDbDatabaseId"],
                ContainerId = Configuration["CosmosDbPrivateConversationStateContainerId"],
                CompatibilityMode = false
            };

            // Create the Private Conversation state. (Used in this bot's Dialog implementation.)
            services.AddSingleton(new PrivateConversationState(new CosmosDbPartitionedStorage(privateConversationStorageOptions)));

            // Register LUIS recognizer
            services.AddSingleton<UniversityRecognizer>();

            // The MainDialog that will be run by the bot.
            services.AddSingleton<MainDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogBot<MainDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
