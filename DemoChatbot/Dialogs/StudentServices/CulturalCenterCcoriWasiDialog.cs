using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoChatbot.Dialogs
{
    public class CulturalCenterCcoriWasiDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(CulturalCenterCcoriWasiDialog);

        public const string SUBTITLE = "Centro Cultural Ccori Wasi";

        protected readonly ILogger Logger;

        private const string CCORI_WASI_SELECTED = "CcoriWasiSelected";
        private const string CCORI_WASI_OPTION_SELECTED = "CcoriWasiOptionSelected";

        private const string INFORMATION = "Información";
        private const string AMAUTA_LIBRARY = "Bibl. El Amauta";
        private const string AMAUTA_LIBRARY_INFO = "Info. B. El Amauta";
        private const string AMAUTA_LIBRARY_SCHEDULE = "Horarios (Atención)";
        private const string CONTACT_INFO = "Info. de Contacto";

        private readonly string[] Choices = new string[]
        {
            INFORMATION,
            AMAUTA_LIBRARY,
            AMAUTA_LIBRARY_INFO,
            AMAUTA_LIBRARY_SCHEDULE,
            CONTACT_INFO
        };

        public CulturalCenterCcoriWasiDialog(
            UserState userState,
            ILogger<MainDialog> logger,
            IBotTelemetryClient telemetryClient,
            string dialogId = DIALOG_ID) : base(dialogId)
        {
            Logger = logger;
            TelemetryClient = telemetryClient;

            var waterfallSteps = new WaterfallStep[]
            {
                CulturalCenterCcoriWasiStepAsync,
                CulturalCenterCcoriWasiOptionStepAsync,
                CulturalCenterCcoriWasiInfoStepAsync,
                DialogHelper.LastGoBackStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> CulturalCenterCcoriWasiStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Options as IDictionary<string, string> ?? new Dictionary<string, string>();

            stepContext.Values[DialogHelper.OPTIONS_SELECTED] = optionsSelected;

            if (optionsSelected.ContainsKey(CCORI_WASI_SELECTED))
            {
                var ccoriWasiSelected = optionsSelected[CCORI_WASI_SELECTED];

                return await stepContext.NextAsync(ccoriWasiSelected, cancellationToken);
            }

            var ccoriWasiAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = SUBTITLE,
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = INFORMATION,
                            Value = INFORMATION
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = AMAUTA_LIBRARY,
                            Value = AMAUTA_LIBRARY
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = CONTACT_INFO,
                            Value = CONTACT_INFO
                        }
                    }
                }.ToAttachment()
            };

            var messageReply = MessageFactory.Text($"Estas son las opciones en {SUBTITLE}. 📖🏫");

            var reply = MessageFactory.Attachment(ccoriWasiAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> CulturalCenterCcoriWasiOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                return await stepContext.ReplaceDialogAsync(
                    StudentServicesDialog.DIALOG_ID,
                    optionsSelected,
                    cancellationToken);
            }

            if (!Choices.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            if (optionsSelected.ContainsKey(CCORI_WASI_OPTION_SELECTED))
            {
                var univChapOptionSelected = optionsSelected[CCORI_WASI_OPTION_SELECTED];

                return await stepContext.NextAsync(univChapOptionSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(CCORI_WASI_SELECTED))
            {
                optionsSelected.Add(CCORI_WASI_SELECTED, resultSelected);
            }

            switch (resultSelected)
            {
                case INFORMATION:
                    //get from api
                    var info = "El **Centro Cultural Ccori Wasi** es un espacio de encuentro de actividades de investigación, " +
                        "difusión y discusión del conocimiento científico y humanístico, de exposición de obras de arte, " +
                        "de arquitectura y, en general, de las más importantes expresiones de nuestra cultura y de la cultura universal; " +
                        "es un lugar que acoge la producción académica de la propia Universidad Ricardo Palma, " +
                        "así como de otras instituciones, tanto públicas como privadas.";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);

                    break;
                case AMAUTA_LIBRARY:
                    var amautaLibraryAttachments = new List<Attachment>
                    {
                        new HeroCard
                        {
                            Title = AMAUTA_LIBRARY,
                            Buttons = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = AMAUTA_LIBRARY_INFO,
                                    Value = AMAUTA_LIBRARY_INFO
                                },
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = AMAUTA_LIBRARY_SCHEDULE,
                                    Value = AMAUTA_LIBRARY_SCHEDULE
                                }
                            }
                        }.ToAttachment()
                    };

                    var amautaLibraryMessageReply = MessageFactory.Text($"Estas son las opciones en {AMAUTA_LIBRARY}. 📖🏫");

                    var amautaLibraryReply = MessageFactory.Attachment(amautaLibraryAttachments);

                    await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, amautaLibraryMessageReply, amautaLibraryReply);

                    return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
                case CONTACT_INFO:
                    var contactInfoLines = new string[]
                    {
                        $"**Dirección:** {"Av. Arequipa 5198, Miraflores - Lima 18"}",
                        $"**Correo:** {"capellania@urp.edu.pe"}",
                        $"**Teléfono:** {"445-7335"}",
                        $"**Facebook:** {"@ccoriwasi"}"
                    };

                    string contactInfoText = string.Join("\n\n", contactInfoLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(contactInfoText), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> CulturalCenterCcoriWasiInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(CCORI_WASI_SELECTED);

                return await stepContext.ReplaceDialogAsync(DIALOG_ID, optionsSelected, cancellationToken);
            }

            if (!Choices.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            switch (resultSelected)
            {
                case AMAUTA_LIBRARY_INFO:
                    var amautaLibraryInfoLines = new string[]
                    {
                        "La **Biblioteca “EL AMAUTA”** se encuentra ubicada en el 2do. piso del Centro Cultural Ccori Wasi – Av. Arequipa 5198, Miraflores.",
                        "En la actualidad cuenta con temas de arquitectura, museología, historia, diseño de interiores, construcción, ciencias sociales, " +
                        "literatura, geografía entre otros, un total de 1442 libros, 240 revistas, folletos, catálogos, " +
                        "hemeroteca en arquitectura y diseño, atlas y 160 libros de Museología."
                    };

                    string amautaLibraryInfoText = string.Join("\n\n", amautaLibraryInfoLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(amautaLibraryInfoText), cancellationToken);

                    break;
                case AMAUTA_LIBRARY_SCHEDULE:
                    var amautaLibraryScheduleLines = new string[]
                    {
                        "**Horarios de atención**",
                        "Lunes y martes de 8:00 a 19:00 hrs.",
                        "Miércoles de 15:00 a 17:30 hrs.",
                        "Jueves de 10:00 a 19:00 hrs.",
                        "Viernes de 13: a 17:00 hrs."
                    };

                    string amautaLibraryScheduleText = string.Join("\n\n", amautaLibraryScheduleLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(amautaLibraryScheduleText), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }
    }
}
