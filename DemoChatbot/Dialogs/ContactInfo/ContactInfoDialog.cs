using DemoChatbot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DemoChatbot.Dialogs
{
    public class ContactInfoDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(ContactInfoDialog);

        protected readonly ILogger Logger;

        private readonly UserState _userState;

        private const string DNI_STEP_ID = nameof(TextPrompt) + "DNI";
        private const string PHONE_STEP_ID = nameof(TextPrompt) + "Phone";
        private const string EMAIL_STEP_ID = nameof(TextPrompt) + "Email";

        private const string FIRST_NAME_SELECTED = "firstName";
        private const string LAST_NAME_SELECTED = "lastName";
        private const string DNI_SELECTED = "dni";
        private const string PHONE_SELECTED = "phone";
        private const string EMAIL_SELECTED = "email";

        public ContactInfoDialog(
            UserState userState,
            ILogger<MainDialog> logger,
            IBotTelemetryClient telemetryClient,
            string dialogId = DIALOG_ID) : base(dialogId)
        {
            Logger = logger;
            TelemetryClient = telemetryClient;

            _userState = userState;

            var waterfallSteps = new WaterfallStep[]
            {
                FullNameStepAsync,
                DNIStepAsync,
                PhoneStepAsync,
                EmailStepAsync,
                ReviewStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new TextPrompt(DNI_STEP_ID, ValidateDNI));
            AddDialog(new TextPrompt(PHONE_STEP_ID, ValidatePhone));
            AddDialog(new TextPrompt(EMAIL_STEP_ID, ValidateEmail));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> FullNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text("¡Hola! ¡Bienvenido al nuevo Chatbot URP! " +
                "Mi nombre es RichiBot y estoy para ayudarte. 🤖"), cancellationToken);

            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text("Eres un desconocido, así que necesito que llenes el siguiente formulario " +
                "para continuar con nuestra conversación. Pero descuida, sólo será por esta única vez. 🦾"), cancellationToken);

            await stepContext.Context.SendActivityAsync(new Activity { Type = ActivityTypes.Delay, Value = 500 }, cancellationToken);

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Por favor, ingresa tu nombre completo. 😁")
                },
                cancellationToken
            );
        }

        private async Task<DialogTurnResult> DNIStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string fullName = (stepContext.Result as string).Trim();

            var splitName = Regex.Split(fullName, "\\s+");

            int length = splitName.Length;
            int halfLength = (length / 2) + System.Convert.ToInt32(length == 1);

            stepContext.Values[FIRST_NAME_SELECTED] = string.Join(' ', splitName, 0, halfLength);
            stepContext.Values[LAST_NAME_SELECTED] = string.Join(' ', splitName, halfLength, length - halfLength);

            await stepContext.Context.SendActivityAsync(new Activity { Type = ActivityTypes.Delay, Value = 500 }, cancellationToken);

            return await stepContext.PromptAsync(
                DNI_STEP_ID,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text($"¡Gracias, {fullName}! " +
                    $"También necesito que ingreses tu DNI. 🆔"),
                    RetryPrompt = MessageFactory.Text("Por favor, ingresa un número de DNI válido.")
                },
                cancellationToken
            );
        }

        private async Task<bool> ValidateDNI(PromptValidatorContext<string> promptValidatorContext, CancellationToken cancellationToken)
        {
            return await Task.FromResult(promptValidatorContext.Recognized.Succeeded &&
                Regex.IsMatch(promptValidatorContext.Recognized.Value.Trim(), @"^\d{8}$"));
        }

        private async Task<DialogTurnResult> PhoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values[DNI_SELECTED] = (stepContext.Result as string).Trim();

            await stepContext.Context.SendActivityAsync(new Activity { Type = ActivityTypes.Delay, Value = 500 }, cancellationToken);

            return await stepContext.PromptAsync(
                PHONE_STEP_ID,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Ahora necesito que ingreses tu número celular. 📱"),
                    RetryPrompt = MessageFactory.Text("Ingresa un número de teléfono válido, por favor.")
                },
                cancellationToken
            );
        }

        private async Task<bool> ValidatePhone(PromptValidatorContext<string> promptValidatorContext, CancellationToken cancellationToken)
        {
            return await Task.FromResult(promptValidatorContext.Recognized.Succeeded &&
                Regex.IsMatch(promptValidatorContext.Recognized.Value.Trim(), @"^\d{9}$"));
        }

        private async Task<DialogTurnResult> EmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values[PHONE_SELECTED] = (stepContext.Result as string).Trim();

            await stepContext.Context.SendActivityAsync(new Activity { Type = ActivityTypes.Delay, Value = 500 }, cancellationToken);

            return await stepContext.PromptAsync(
                EMAIL_STEP_ID,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("También que ingreses tu email. 📧"),
                    RetryPrompt = MessageFactory.Text("Por favor, ingresa un email válido.")
                },
                cancellationToken
            );
        }

        private async Task<bool> ValidateEmail(PromptValidatorContext<string> promptValidatorContext, CancellationToken cancellationToken)
        {
            return await Task.FromResult(promptValidatorContext.Recognized.Succeeded &&
                Regex.IsMatch(promptValidatorContext.Recognized.Value.Trim(), @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"));
        }

        private async Task<DialogTurnResult> ReviewStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values[EMAIL_SELECTED] = (stepContext.Result as string).Trim();

            var conversationReference = stepContext.Context.Activity.GetConversationReference();

            var userAccessor = _userState.CreateProperty<UserInfo>(nameof(UserInfo));
            var userInfo = await userAccessor.GetAsync(
                stepContext.Context,
                () => new UserInfo
                {
                    FirstName = (string)stepContext.Values[FIRST_NAME_SELECTED],
                    LastName = (string)stepContext.Values[LAST_NAME_SELECTED],
                    DNI = (string)stepContext.Values[DNI_SELECTED],
                    Phone = (string)stepContext.Values[PHONE_SELECTED],
                    Email = (string)stepContext.Values[EMAIL_SELECTED],
                    AccountId = conversationReference.User.Id,
                    AccountName = conversationReference.User.Name,
                    ChannelId = conversationReference.ChannelId
                },
                cancellationToken);

            await userAccessor.SetAsync(stepContext.Context, userInfo, cancellationToken);

            TelemetryClient.TrackEvent("UserInfo", userInfo.ToDictionary());

            await stepContext.Context.SendActivityAsync(new Activity { Type = ActivityTypes.Delay, Value = 500 }, cancellationToken);

            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"¡Listo, {userInfo.FirstName}! 😄"), cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
