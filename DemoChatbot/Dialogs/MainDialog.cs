using CognitiveModels;
using DemoChatbot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DemoChatbot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(MainDialog);

        protected readonly UniversityRecognizer LuisRecognizer;

        protected readonly ILogger Logger;

        private readonly UserState _userState;
        private readonly IStatePropertyAccessor<UserInfo> _userAccessor;

        private readonly PrivateConversationState _privateConversationState;

        public MainDialog(
            UniversityRecognizer luisRecognizer,
            UserState userState,
            PrivateConversationState privateConversationState,
            ILogger<MainDialog> logger,
            IBotTelemetryClient telemetryClient,
            string dialogId = DIALOG_ID) : base(dialogId)
        {
            Logger = logger;
            TelemetryClient = telemetryClient;
            LuisRecognizer = luisRecognizer;

            _userState = userState;
            _userAccessor = _userState.CreateProperty<UserInfo>(nameof(UserInfo));

            _privateConversationState = privateConversationState;

            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                MainMenuStepAsync,
                FinalStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            // Custom Dialogs
            AddDialog(new ContactInfoDialog(userState, logger, telemetryClient));
            AddDialog(new MainMenuDialog(userState, privateConversationState, logger, telemetryClient));
            AddDialog(new AdmissionDialog(userState, privateConversationState, logger, telemetryClient));
            AddDialog(new UndergraduateCareerDialog(userState, privateConversationState, logger, telemetryClient));
            AddDialog(new PostgraduateCareerDialog(userState, privateConversationState, logger, telemetryClient));
            AddDialog(new ProfessionalStudiesWorkExperienceDialog(userState, privateConversationState, logger, telemetryClient));
            AddDialog(new SecondSpecialtyDialog(userState, privateConversationState, logger, telemetryClient));
            AddDialog(new ProgramsSpecializationsDialog(userState, privateConversationState, logger, telemetryClient));
            AddDialog(new StudentServicesDialog(userState, privateConversationState, logger, telemetryClient));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInfo = await _userAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            if (userInfo == null)
            {
                return await stepContext.BeginDialogAsync(
                    ContactInfoDialog.DIALOG_ID,
                    cancellationToken: cancellationToken);
            }

            return await stepContext.NextAsync(stepContext.Context?.Activity?.Text, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> MainMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result == null)
            {
                if (!LuisRecognizer.IsConfigured)
                {
                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.Text("Estamos teniendo complicaciones técnicas, por favor comunicarse con soporte.",
                        inputHint: InputHints.IgnoringInput), cancellationToken);
                }

                return await BeginMainMenuDialog(stepContext, cancellationToken);
            }

            var luisResult = await LuisRecognizer.RecognizeAsync<University>(stepContext.Context, cancellationToken);
            var (intent, _) = luisResult.TopIntent();

            switch (intent)
            {
                case University.Intent.Admission:
                    return await stepContext.BeginDialogAsync(
                        AdmissionDialog.DIALOG_ID,
                        cancellationToken: cancellationToken);
                    
                case University.Intent.Greeting:
                    var userInfo = await _userAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

                    string name = userInfo?.FirstName ?? "amigo";

                    var greetingIntentText = $"¡Hola {name}! Listo para ayudarte. 😁";

                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.Text(greetingIntentText, InputHints.IgnoringInput),
                        cancellationToken);

                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

                case University.Intent.MainMenu:
                    return await BeginMainMenuDialog(stepContext, cancellationToken);

                case University.Intent.None:
                default:
                    var noneIntentText = "Lo siento, no entendí lo que dijiste, aún estoy aprendiendo. 😥";

                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.Text(noneIntentText, InputHints.IgnoringInput),
                        cancellationToken);

                    return await BeginMainMenuDialog(stepContext, cancellationToken);

                case University.Intent.PostgraduateCareer:
                    return await stepContext.BeginDialogAsync(
                        PostgraduateCareerDialog.DIALOG_ID,
                        cancellationToken: cancellationToken);

                case University.Intent.ProfessionalStudiesWorkExperience:
                    return await stepContext.BeginDialogAsync(
                        ProfessionalStudiesWorkExperienceDialog.DIALOG_ID,
                        cancellationToken: cancellationToken);

                case University.Intent.ProgramsSpecializations:
                    return await stepContext.BeginDialogAsync(
                        ProgramsSpecializationsDialog.DIALOG_ID,
                        cancellationToken: cancellationToken);

                case University.Intent.SecondSpecialty:
                    return await stepContext.BeginDialogAsync(
                        SecondSpecialtyDialog.DIALOG_ID,
                        cancellationToken: cancellationToken);

                case University.Intent.StudentServices:
                    return await stepContext.BeginDialogAsync(
                        StudentServicesDialog.DIALOG_ID,
                        cancellationToken: cancellationToken);

                case University.Intent.UndergraduateCareer:
                    return await stepContext.BeginDialogAsync(
                        UndergraduateCareerDialog.DIALOG_ID,
                        cancellationToken: cancellationToken);

                case University.Intent.YourCreator:
                    var yourCreatorIntentText = "Mis creadores son los Ganadores Absolutos de FERSOFT 2017-II 🦾🔥";

                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.Text(yourCreatorIntentText, InputHints.IgnoringInput),
                        cancellationToken);

                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var privateAccessor = _privateConversationState.CreateProperty<DialogChoice>(DialogHelper.DIALOG_CHOICE);
            await privateAccessor.DeleteAsync(stepContext.Context, cancellationToken);

            if (stepContext.Values.Count > 0 &&
                stepContext.Values[DialogHelper.OPTIONS_SELECTED] is IDictionary<string, string> optionsSelected)
            {
                optionsSelected.Clear();
            }

            return await stepContext.ReplaceDialogAsync(DIALOG_ID, stepContext.Options, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> BeginMainMenuDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(
                MainMenuDialog.DIALOG_ID,
                cancellationToken: cancellationToken);
        }
    }
}
