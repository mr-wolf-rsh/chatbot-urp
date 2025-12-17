using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DemoChatbot.Bots
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly Dialog Dialog;

        protected readonly BotState ConversationState;
        protected readonly BotState UserState;
        protected readonly BotState PrivateConversationState;

        protected readonly ILogger Logger;
        protected readonly IBotTelemetryClient TelemetryClient;

        protected static IConfiguration Configuration;

        private const string DIALOG_STATE_NAME = nameof(DialogState);

        public DialogBot(IConfiguration configuration,
            ConversationState conversationState,
            UserState userState,
            PrivateConversationState privateConversationState,
            T dialog,
            ILogger<DialogBot<T>> logger,
            IBotTelemetryClient telemetryClient)
        {
            Configuration = configuration;
            Logger = logger;
            TelemetryClient = telemetryClient;

            ConversationState = conversationState;
            UserState = userState;
            PrivateConversationState = privateConversationState;

            Dialog = dialog;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            await PrivateConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(DIALOG_STATE_NAME), cancellationToken);
                }
            }
        }
        
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(DIALOG_STATE_NAME), cancellationToken);
        }
    }
}
