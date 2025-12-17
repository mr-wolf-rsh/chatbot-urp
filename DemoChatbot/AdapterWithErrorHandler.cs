// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.13.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;

namespace DemoChatbot
{
    public class AdapterWithErrorHandler : CloudAdapter
    {
        //Create field for telemetry client. Add IBotTelemetryClient parameter to AdapterWithErrorHandler
        private readonly IBotTelemetryClient _adapterBotTelemetryClient;

        private const string ADAPTER_ID = nameof(AdapterWithErrorHandler);

        public AdapterWithErrorHandler(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory, 
            ILogger<IBotFrameworkHttpAdapter> logger,
            TelemetryInitializerMiddleware telemetryInitializerMiddleware,
            IBotTelemetryClient botTelemetryClient,
            ConversationState conversationState = default)
            : base(configuration, httpClientFactory, logger)
        {
            Use(telemetryInitializerMiddleware);

            //Use telemetry client so that we can trace exceptions into Application Insights
            _adapterBotTelemetryClient = botTelemetryClient;

            OnTurnError = async (turnContext, exception) =>
            {
                // Track exceptions into Application Insights
                // Set up some properties for our exception tracing to give more information
                var properties = new Dictionary<string, string>
                {{"Bot exception caught in", $"{ADAPTER_ID} - {nameof(OnTurnError)}"}};

                //Send the exception telemetry:
                _adapterBotTelemetryClient.TrackException(exception, properties);

                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                string errorMessageText = "Tenemos un problema, danos unos minutos, por favor. 😖";

                // Send a message to the user
                await turnContext.SendActivityAsync(errorMessageText, inputHint: InputHints.IgnoringInput);

                if (conversationState != null)
                {
                    try
                    {
                        // Delete the conversationState for the current conversation to prevent the
                        // bot from getting stuck in a error-loop caused by being in a bad state.
                        // ConversationState should be thought of as similar to "cookie-state" in a Web pages.
                        await conversationState.DeleteAsync(turnContext);
                    }
                    catch (System.Exception e)
                    {
                        logger.LogError(e, $"Exception caught on attempting to Delete ConversationState : {e.Message}");
                    }
                }

                // Send a trace activity, which will be displayed in the Bot Framework Emulator
                _adapterBotTelemetryClient.TrackTrace("OnTurnError Trace", Severity.Error, properties);
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
            };
        }
    }
}
