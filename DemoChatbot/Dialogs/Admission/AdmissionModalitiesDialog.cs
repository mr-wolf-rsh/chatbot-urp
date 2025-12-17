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
    public class AdmissionModalitiesDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(AdmissionModalitiesDialog);

        public const string SUBTITLE = "Modalidades de Ingreso";

        protected readonly ILogger Logger;

        private const string ADMISSION_MODALITIES_SELECTED = "AdmissionModalitiesSelected";
        private const string ADMISSION_MODALITIES_OPTION_SELECTED = "AdmissionModalitiesOptionSelected";

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

        public AdmissionModalitiesDialog(
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
                ModalityStepAsync,
                ModalityOptionStepAsync,
                ModalityInfoStepAsync,
                DialogHelper.LastGoBackStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> ModalityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Options as IDictionary<string, string> ?? new Dictionary<string, string>();

            stepContext.Values[DialogHelper.OPTIONS_SELECTED] = optionsSelected;

            var dialogChoice = new DialogChoice
            {
                DialogId = DIALOG_ID,
                ChoiceList = new List<string>
                {
                    "Primeros Puestos",
                    "Bach. Escolar Int.",
                    "Deport. Destacados",
                    "Dip. ini. estudios"
                }
            };

            await _privateConversationAccessor.SetAsync(stepContext.Context, dialogChoice, cancellationToken);

            if (optionsSelected.ContainsKey(ADMISSION_MODALITIES_SELECTED))
            {
                var modalitySelected = optionsSelected[ADMISSION_MODALITIES_SELECTED];

                return await stepContext.NextAsync(modalitySelected, cancellationToken);
            }

            var modalityAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = "Primeros Puestos",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/600/hm/450/we/675/he/" +
                            "450/x/-75/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/27116/n/565210-pl5qye-92.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Primeros Puestos",
                            Value = "Primeros Puestos"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/admision/primeros-puestos"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Bachillerato Escolar Internacional",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/600/hm/450/we/675/he/" +
                            "450/x/-38/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/27117/n/18010.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Bach. Escolar Int.",
                            Value = "Bach. Escolar Int."
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/admision/bachillerato-escolar-internacional"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Deportistas Destacados",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/868/hm/651/we/1384/he/" +
                            "1384/x/-261/y/-376/s/0/q/60/zc/3/f/0/rgb/000000/src/17857/n/60627629-2142042095843283-4811429055985352704-n.png"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Deport. Destacados",
                            Value = "Deport. Destacados"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/admision/deportistas-destacados"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Diplomáticos para iniciar estudios",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/868/hm/651/we/977/he/" +
                            "651/x/-55/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/15790/n/student-3500990-1920.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Dip. ini. estudios",
                            Value = "Dip. ini. estudios"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/admision/diplomaticos-para-iniciar-estudios"
                        }
                    }
                }.ToAttachment()
            };

            var messageReply = MessageFactory.Text($"Estas son las opciones en {SUBTITLE}. 📚💯");

            var reply = MessageFactory.Carousel(modalityAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> ModalityOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

            if (optionsSelected.ContainsKey(ADMISSION_MODALITIES_OPTION_SELECTED))
            {
                var modalityOptionSelected = optionsSelected[ADMISSION_MODALITIES_OPTION_SELECTED];

                return await stepContext.NextAsync(modalityOptionSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(ADMISSION_MODALITIES_SELECTED))
            {
                optionsSelected.Add(ADMISSION_MODALITIES_SELECTED, resultSelected);
            }

            var modalityOptionAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = resultSelected,
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

            var reply = MessageFactory.Attachment(modalityOptionAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> ModalityInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(ADMISSION_MODALITIES_SELECTED);

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
                    var info = "Dirigida a egresados de secundaria del Perú, que haya ocupado el primero o segundo puesto " +
                        "en el orden de mérito de su promoción, considerando los cinco años de educación secundaria. " +
                        "Este beneficio corresponde a los procesos de admisión del año inmediato posterior a la culminación de la educación secundaria.";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);

                    break;
                case REQUIREMENTS:
                    //get from api
                    var requirementsLines = new string[]
                    {
                        "Partida de nacimiento",
                        "Documento nacional de identidad (DNI)",
                        "Certificados originales de los cinco años de estudios secundaria (notas aprobadas), " +
                        "refrendado por el director del sector educativo correspondiente",
                        "Constancia original expedida por el director del centro educativo de procedencia y " +
                        "por el director del sector educativo correspondiente, " +
                        "que acredite haber ocupado el primer o segundo puesto en el cuadro de méritos de su promoción",
                        "Cuadro de puntaje de los cinco años de educación secundaria de los 10 (diez) primeros alumnos, " +
                        "refrendado por el director del centro educativo y por el director del sector educativo correspondiente"
                    };

                    var requirementsMapped = requirementsLines.Select(requirement => $"- {requirement}");

                    string requirementsText = string.Join("\n\n", requirementsMapped);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(requirementsText), cancellationToken);

                    break;
                case REGISTRATION_INVESTMENT:
                    var registrationInvestmentLines = new string[]
                    {
                        $"**Prospecto y revisión de documentos:** {"S/. 50.00"}",
                        $"**Inscripción y recepción de documentos:** {"Del 14 de enero al 14 de febrero de 2022"}",
                        $"**Evaluación:** {"Viernes 25 de febrero de 2022"}",
                        $"**Envía tus documentos a:** {"inscripcionexonerados@urp.edu.pe"}",
                        $"**Pago por Derecho de Evaluación:** {"S/. 300.00"}"
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
