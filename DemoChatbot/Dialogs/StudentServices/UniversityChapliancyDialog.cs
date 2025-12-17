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
    public class UniversityChapliancyDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(UniversityChapliancyDialog);

        public const string SUBTITLE = "Capellanía Universitaria";

        protected readonly ILogger Logger;

        private const string UNIV_CHAP_SELECTED = "UnivChapSelected";
        private const string UNIV_CHAP_OPTION_SELECTED = "UnivChapOptionSelected";

        private const string INFORMATION = "Información";
        private const string POPE_WEEKLY_PHRASE = "Frase sem. del Papa";
        private const string SERVICES = "Servicios";
        private const string HOLY_MASS = "Santa Misa";
        private const string CONFESSIONS = "Confesiones";
        private const string SACRAMENTS = "Sacramentos";
        private const string CONTACT_INFO = "Info. de Contacto";

        private readonly string[] Choices = new string[]
        {
            INFORMATION,
            POPE_WEEKLY_PHRASE,
            SERVICES,
            HOLY_MASS,
            CONFESSIONS,
            SACRAMENTS,
            CONTACT_INFO
        };

        public UniversityChapliancyDialog(
            UserState userState,
            ILogger<MainDialog> logger,
            IBotTelemetryClient telemetryClient,
            string dialogId = DIALOG_ID) : base(dialogId)
        {
            Logger = logger;
            TelemetryClient = telemetryClient;

            var waterfallSteps = new WaterfallStep[]
            {
                UniversityChapliancyStepAsync,
                UniversityChapliancyOptionStepAsync,
                UniversityChapliancyInfoStepAsync,
                DialogHelper.LastGoBackStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> UniversityChapliancyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Options as IDictionary<string, string> ?? new Dictionary<string, string>();

            stepContext.Values[DialogHelper.OPTIONS_SELECTED] = optionsSelected;

            if (optionsSelected.ContainsKey(UNIV_CHAP_SELECTED))
            {
                var univChapSelected = optionsSelected[UNIV_CHAP_SELECTED];

                return await stepContext.NextAsync(univChapSelected, cancellationToken);
            }

            var chapliancyAttachments = new List<Attachment>
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
                            Title = POPE_WEEKLY_PHRASE,
                            Value = POPE_WEEKLY_PHRASE
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = SERVICES,
                            Value = SERVICES
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

            var reply = MessageFactory.Attachment(chapliancyAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> UniversityChapliancyOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

            if (optionsSelected.ContainsKey(UNIV_CHAP_OPTION_SELECTED))
            {
                var univChapOptionSelected = optionsSelected[UNIV_CHAP_OPTION_SELECTED];

                return await stepContext.NextAsync(univChapOptionSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(UNIV_CHAP_SELECTED))
            {
                optionsSelected.Add(UNIV_CHAP_SELECTED, resultSelected);
            }

            switch (resultSelected)
            {
                case INFORMATION:
                    //get from api
                    var info = "La vida universitaria está acompañada de un proceso de aprendizaje, " +
                        "que debe ser enriquecida con la presencia de Dios en nuestras vidas, con su Palabra y " +
                        "por qué no con el testimonio de su mensaje con los compañeros, profesores y " +
                        "en los trabajos que nos tocan desempeñar a lo largo de nuestra carrera universitaria. " +
                        "Dios está con nosotros, sólo tienes que darle el lugar que se merece.";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);

                    break;
                case POPE_WEEKLY_PHRASE:
                    var weeklyPhraseLines = new string[]
                    {
                        "**Frase de la semana**",
                        "\"¿No tienes tentaciones? O eres un ángel o un poco tonto\" -. Papa Francisco",
                        "El pecado más grande hoy es que los hombres han perdido el sentido del pecado."
                    };

                    string weeklyPhraseText = string.Join("\n\n", weeklyPhraseLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(weeklyPhraseText), cancellationToken);

                    break;
                case SERVICES:
                    var servicesAttachments = new List<Attachment>
                    {
                        new HeroCard
                        {
                            Title = SERVICES,
                            Buttons = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = HOLY_MASS,
                                    Value = HOLY_MASS
                                },
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = CONFESSIONS,
                                    Value = CONFESSIONS
                                },
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = SACRAMENTS,
                                    Value = SACRAMENTS
                                }
                            }
                        }.ToAttachment()
                    };

                    var servicesMessageReply = MessageFactory.Text($"Estos son {SERVICES} en {SUBTITLE}:");

                    var servicesReply = MessageFactory.Attachment(servicesAttachments);

                    await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, servicesMessageReply, servicesReply);

                    return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
                case CONTACT_INFO:
                    var contactInfoLines = new string[]
                    {
                        $"**Teléfono:** {"708-0000"}",
                        $"**Anexo:** {"8304"}",
                        $"**Correo:** {"capellania@urp.edu.pe"}"
                    };

                    string contactInfoText = string.Join("\n\n", contactInfoLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(contactInfoText), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> UniversityChapliancyInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(UNIV_CHAP_SELECTED);

                return await stepContext.ReplaceDialogAsync(DIALOG_ID, optionsSelected, cancellationToken);
            }

            if (!Choices.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            switch (resultSelected)
            {
                case HOLY_MASS:
                    var holyMassLines = new string[]
                    {
                        "**Santa Misa**",
                        "La Liturgia es el culto oficial de la Iglesia. " +
                        "Una Liturgia no es un «evento» que consista en buenas ideas y canciones estupendas. " +
                        "La Liturgia no se hace ni se inventa. Es algo vivo que ha creció en la fe a lo largo de los siglos. " +
                        "Un acto de culto es un acontecimiento sagrado y venerable."
                    };

                    string holyMassText = string.Join("\n\n", holyMassLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(holyMassText), cancellationToken);

                    break;
                case CONFESSIONS:
                    var confessionsLines = new string[]
                    {
                        "**Confesiones**",
                        "La confesión es reconciliarnos con Dios ante nuestra miseria de pecado, " +
                        "es estar de nuevo en paz con Dios, además de ello también es un reconciliarnos con nosotros mismos, " +
                        "es darme la oportunidad de empezar de nuevo con la gracia (ayuda) de Dios."
                    };

                    string confessionsText = string.Join("\n\n", confessionsLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(confessionsText), cancellationToken);

                    break;
                case SACRAMENTS:
                    var sacramentsLines = new string[]
                    {
                        "**Sacramentos**",
                        "Todos los sacramentos son un encuentro con Cristo, que es él mismo el sacramento original. " +
                        "Hay sacramentos de iniciación, que introducen en la fe: Bautismo, Confirmación y Eucaristía. " +
                        "Hay sacramentos de curación: Penitencia y Unción de enfermos. " +
                        "Y hay sacramentos que están al servicio de la comunión y misión de los fieles: Matrimonio y Orden"
                    };

                    string sacramentsText = string.Join("\n\n", sacramentsLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(sacramentsText), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }
    }
}
