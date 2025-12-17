using DemoChatbot.Models;
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
    public class ResumptionOfStudiesDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(ResumptionOfStudiesDialog);

        public const string SUBTITLE = "Continuación de Estudios";

        protected readonly ILogger Logger;

        private const string RESUMPTION_OF_STUDIES_SELECTED = "ResumptionOfStudiesSelected";
        private const string RESUMPTION_OF_STUDIES_OPTION_SELECTED = "ResumptionOfStudiesOptionSelected";

        private const string INFORMATION = "Información";
        private const string REQUIREMENTS = "Requisitos";
        private const string REGISTRATION_INVESTMENT = "Inscr. e Inversión";

        private readonly PrivateConversationState _privateConversationState;
        private readonly IStatePropertyAccessor<DialogChoice> _privateConversationAccessor;

        private readonly string[] Choices = new string[]
        {
            INFORMATION,
            REQUIREMENTS,
            REGISTRATION_INVESTMENT
        };

        public ResumptionOfStudiesDialog(
            UserState userState,
            PrivateConversationState privateConversationState,
            ILogger<MainDialog> logger,
            IBotTelemetryClient telemetryClient,
            string dialogId = DIALOG_ID) : base(dialogId)
        {
            Logger = logger;
            TelemetryClient = telemetryClient;

            _privateConversationState = privateConversationState;
            _privateConversationAccessor = _privateConversationState.CreateProperty<DialogChoice>(DialogHelper.DIALOG_CHOICE);

            var waterfallSteps = new WaterfallStep[]
            {
                ResumptionStepAsync,
                ResumptionOptionStepAsync,
                ResumptionInfoStepAsync,
                DialogHelper.LastGoBackStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> ResumptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Options as IDictionary<string, string> ?? new Dictionary<string, string>();

            stepContext.Values[DialogHelper.OPTIONS_SELECTED] = optionsSelected;

            var dialogChoice = new DialogChoice
            {
                DialogId = DIALOG_ID,
                ChoiceList = new List<string>
                {
                    "Traslados Externos",
                    "Grad. y/o Titulados",
                    "Dip. cont. estudios"
                }
            };

            await _privateConversationAccessor.SetAsync(stepContext.Context, dialogChoice, cancellationToken);

            if (optionsSelected.ContainsKey(RESUMPTION_OF_STUDIES_SELECTED))
            {
                var resumptionSelected = optionsSelected[RESUMPTION_OF_STUDIES_SELECTED];

                return await stepContext.NextAsync(resumptionSelected, cancellationToken);
            }

            var resumptionAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = "Traslados Externos",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/600/hm/450/we/675/he/" +
                            "450/x/-38/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/27115/n/167779-ovi6ve-368.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Traslados Externos",
                            Value = "Traslados Externos"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/admision/traslados-externos"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Graduados y/o Titulados",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/779/hm/584/we/1158/he/" +
                            "584/x/-190/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/12518/n/captura.png"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Grad. y/o Titulados",
                            Value = "Grad. y/o Titulados"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/admision/graduados-yo-titulados"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Diplomáticos para continuar estudios",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/480/hm/360/we/480/he/" +
                            "371/x/0/y/-6/s/0/q/60/zc/3/f/0/rgb/000000/src/23991/n/transformacion-digital.png"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Dip. cont. estudios",
                            Value = "Dip. cont. estudios"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/admision/diplomaticos-para-continuar-estudios"
                        }
                    }
                }.ToAttachment()
            };

            var messageReply = MessageFactory.Text($"Estas son nuestras {SUBTITLE}. 📚💯");

            var reply = MessageFactory.Carousel(resumptionAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> ResumptionOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                return await DialogHelper.ReplaceMainMenuDialogAsync(stepContext, cancellationToken);
            }

            var previousDialogChoice = await _privateConversationAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            if (previousDialogChoice == null || !previousDialogChoice.ChoiceList.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            if (optionsSelected.ContainsKey(RESUMPTION_OF_STUDIES_OPTION_SELECTED))
            {
                var resumptionOptionSelected = optionsSelected[RESUMPTION_OF_STUDIES_OPTION_SELECTED];

                return await stepContext.NextAsync(resumptionOptionSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(RESUMPTION_OF_STUDIES_SELECTED))
            {
                optionsSelected.Add(RESUMPTION_OF_STUDIES_SELECTED, resultSelected);
            }

            var resumptionOptionAttachments = new List<Attachment>
            {
                new HeroCard
                {
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
                            Title = REQUIREMENTS,
                            Value = REQUIREMENTS
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = REGISTRATION_INVESTMENT,
                            Value = REGISTRATION_INVESTMENT
                        }
                    }
                }.ToAttachment()
            };

            var messageReply = MessageFactory.Text($"Tenemos las siguientes opciones para ti en {resultSelected}:");

            var reply = MessageFactory.Attachment(resumptionOptionAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> ResumptionInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(RESUMPTION_OF_STUDIES_SELECTED);

                return await stepContext.ReplaceDialogAsync(DIALOG_ID, optionsSelected, cancellationToken);
            }

            if (!Choices.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            switch (resultSelected)
            {
                case INFORMATION:
                    //get from api
                    var info = "Dirigida a los alumnos que hayan aprobado por lo menos cuatro periodos semestrales completos o " +
                        "dos anuales o setenta y dos (72) créditos, en universidades del país o del extranjero. " +
                        "También pueden postular por esta modalidad, los estudiantes de los centros de Educación Superior mencionados " +
                        "en la Segunda y Tercera Disposición Complementaria Final de la Ley Universidad N° 30220";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);

                    break;
                case REQUIREMENTS:
                    //get from api
                    var requirementsLines = new string[]
                    {
                        "Partida de nacimiento",
                        "Documento nacional de identidad (DNI). Los postulantes extranjeros presentarán copia legalizada del carné de extranjería o pasaporte",
                        "Certificados originales de los cinco años de estudios de educación secundaria. " +
                        "Los postulantes que hayan realizado sus estudios secundarios en el extranjero, " +
                        "presentaran la resolución de convalidación correspondiente emitida por el Ministerio de Educación del Perú",
                        "Certificados originales de estudios superiores, con un mínimo de setenta y dos (72) créditos aprobados o " +
                        "cuatro períodos semestrales o dos anuales aprobados en universidades de Perú o del extranjero " +
                        "en concordancia con el plan de estudios (adjuntar plan de estudios)",
                        "Constancia de no haber sido separado del centro superior de estudios de origen por motivos académicos ni disciplinarios. " +
                        "La fecha de expedición debe ser posterior a la finalización del último semestre de estudios",
                        "Sílabos anillados de los cursos aprobados, con la firma y sello de la autoridad competente " +
                        "del centro de estudios superiores de procedencia. Su omisión no impide la inscripción, " +
                        "pero si la convalidación, serán entregados solo por los alumnos ingresantes el lunes siguiente a la publicación de los resultados",
                        "Para traslados externos internacionales, los documentos señalados en los incisos anteriores, " +
                        "deberán estar certificados por el Consulado Peruano del país de origen y el Ministerio de Relaciones Exteriores del Perú y " +
                        "traducidos oficialmente al español si fueron expedidos en otro idioma"
                    };

                    var requirementsMapped = requirementsLines.Select(requirement => $"- {requirement}");

                    string requirementsText = string.Join("\n\n", requirementsMapped);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(requirementsText), cancellationToken);

                    break;
                case REGISTRATION_INVESTMENT:
                    var registrationInvestmentLines = new string[]
                    {
                        $"**Prospecto y revisión de documentos:** {"S/. 50.00"}",
                        $"**Inscripción y recepción de documentos:** {"Del 03 de enero al 31 de enero de 2022"}",
                        $"**Evaluación:** {"Lunes 14 y martes 15 de febrero de 2022"}",
                        $"**Envía tus documentos a:** {"inscripcionexonerados@urp.edu.pe"}",
                        $"**Pago por Derecho de Evaluación:** {"\n\n**Traslado Externo Nacional:** S/. 350.00\n\n**Costo Traslado Externo Internacional**: S/. 650.00"}"
                    };

                    string registrationInvestmentText = string.Join("\n\n", registrationInvestmentLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(registrationInvestmentText), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }
    }
}
