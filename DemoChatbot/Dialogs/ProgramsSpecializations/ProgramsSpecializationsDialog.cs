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
    public class ProgramsSpecializationsDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(ProgramsSpecializationsDialog);

        public const string SUBTITLE = "Programas y Especializaciones";

        protected readonly ILogger Logger;

        private const string PROGRAMS_SPECS_SELECTED = "ProgramsSpecsSelected";
        private const string PROGRAMS_SPECS_OPTION_SELECTED = "ProgramsSpecsOptionSelected";
        private const string PROGRAMS_SPECS_INFO_SELECTED = "ProgramsSpecsInfoSelected";

        private const string MODULES = "Módulos";
        private const string DURATION = "Duración";
        private const string INVESTMENT = "Inversión";

        private readonly PrivateConversationState _privateConversationState;
        private readonly IStatePropertyAccessor<DialogChoice> _privateConversationAccessor;

        private readonly string[] Choices = new string[]
        {
            MODULES,
            DURATION,
            INVESTMENT
        };

        public ProgramsSpecializationsDialog(
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
                ProgramsSpecsStepAsync,
                ProgramsSpecsOptionStepAsync,
                ProgramsSpecsInfoStepAsync,
                ProgramsSpecsInfoOptionStepAsync,
                DialogHelper.LastGoBackStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> ProgramsSpecsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Options as IDictionary<string, string> ?? new Dictionary<string, string>();

            stepContext.Values[DialogHelper.OPTIONS_SELECTED] = optionsSelected;

            var dialogChoice = new DialogChoice
            {
                DialogId = DIALOG_ID,
                ChoiceList = new List<string>
                {
                    "Marketing",
                    "Recursos Humanos",
                    "Idiomas",
                    "Arq. y Urbanismo",
                    "Ciencia de Datos",
                    "Educación Continua",
                    "Ingeniería",
                }
            };

            await _privateConversationAccessor.SetAsync(stepContext.Context, dialogChoice, cancellationToken);

            if (optionsSelected.ContainsKey(PROGRAMS_SPECS_SELECTED))
            {
                var progSpecsSelected = optionsSelected[PROGRAMS_SPECS_SELECTED];

                return await stepContext.NextAsync(progSpecsSelected, cancellationToken);
            }

            var progSpecsTypeAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = SUBTITLE,
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Marketing",
                            Value = "Marketing"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Recursos Humanos",
                            Value = "Recursos Humanos"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Idiomas",
                            Value = "Idiomas"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Arq. y Urbanismo",
                            Value = "Arq. y Urbanismo"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Ciencia de Datos",
                            Value = "Ciencia de Datos"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Educación Continua",
                            Value = "Educación Continua"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Ingeniería",
                            Value = "Ingeniería"
                        }
                    }
                }.ToAttachment(),
            };

            var messageReply = MessageFactory.Text($"Estas son nuestros {SUBTITLE} 📓🖋");

            var reply = MessageFactory.Carousel(progSpecsTypeAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> ProgramsSpecsOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

            var dialogChoice = new DialogChoice
            {
                DialogId = DIALOG_ID,
                ChoiceList = new List<string>
                {
                    "Inv. Cuantitativa",
                    "Coaching y Consult.",
                    "Recursos Humanos"
                }
            };

            await _privateConversationAccessor.SetAsync(stepContext.Context, dialogChoice, cancellationToken);

            if (optionsSelected.ContainsKey(PROGRAMS_SPECS_OPTION_SELECTED))
            {
                var progSpecsSelected = optionsSelected[PROGRAMS_SPECS_OPTION_SELECTED];

                return await stepContext.NextAsync(progSpecsSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(PROGRAMS_SPECS_SELECTED))
            {
                optionsSelected.Add(PROGRAMS_SPECS_SELECTED, resultSelected);
            }

            var progSpecsOptionAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = "Especialización Profesional en Investigación Cuantitativa",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/400/we/602/he/" +
                            "400/x/-101/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/701/n/doctorados.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Inv. Cuantitativa",
                            Value = "Inv. Cuantitativa"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/programa-de-especializacion-en-investigacion-cuantitativa"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Especialización en Coaching y Consultoría",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/400/we/832/he/" +
                            "400/x/-62/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/704/n/botn-maestrias.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Coaching y Consult.",
                            Value = "Coaching y Consult."
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/programas-y-cursos/programa-de-especializacion-en-coaching-y-consultoria"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Especialización en Recursos Humanos",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/400/we/832/he/" +
                            "400/x/-62/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/704/n/botn-maestrias.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Recursos Humanos",
                            Value = "Recursos Humanos"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/programas-y-cursos/programa-de-especializacion-en-recursos-humanos"
                        }
                    }
                }.ToAttachment()
            };

            var messageReply = MessageFactory.Text($"Tenemos estas opciones en {resultSelected}:");

            var reply = MessageFactory.Carousel(progSpecsOptionAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> ProgramsSpecsInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(PROGRAMS_SPECS_SELECTED);

                return await stepContext.ReplaceDialogAsync(DIALOG_ID, optionsSelected, cancellationToken);
            }

            var previousDialogChoice = await _privateConversationAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            if (previousDialogChoice == null || !previousDialogChoice.ChoiceList.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            if (optionsSelected.ContainsKey(PROGRAMS_SPECS_INFO_SELECTED))
            {
                var facultyOptionSelected = optionsSelected[PROGRAMS_SPECS_INFO_SELECTED];

                return await stepContext.NextAsync(facultyOptionSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(PROGRAMS_SPECS_OPTION_SELECTED))
            {
                optionsSelected.Add(PROGRAMS_SPECS_OPTION_SELECTED, resultSelected);
            }

            var progSpecsOptionsAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = resultSelected,
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = MODULES,
                            Value = MODULES
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = DURATION,
                            Value = DURATION
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = INVESTMENT,
                            Value = INVESTMENT
                        }
                    }
                }.ToAttachment()
            };

            var messageReply = MessageFactory.Text($"Tenemos las siguientes opciones para ti en {resultSelected}:");

            var reply = MessageFactory.Attachment(progSpecsOptionsAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> ProgramsSpecsInfoOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(PROGRAMS_SPECS_OPTION_SELECTED);

                return await stepContext.ReplaceDialogAsync(DIALOG_ID, optionsSelected, cancellationToken);
            }

            if (!Choices.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            switch (resultSelected)
            {
                case MODULES:
                    var modulesLines = new string[]
                    {
                        "Epistemología de la Investigación Científica",
                        "El proyecto de Investigación",
                        "Trabajo de Campo en la investigación",
                        "Estadística aplicada a la investigación científica",
                        "El SPSS aplicado a la investigación científica",
                        "Redacción al Informe Final de la Tesis"
                    };

                    string modulesMapping = string.Join(string.Empty, modulesLines.Select(m => $"* {m}"));

                    string modulesText = $"**MÓDULOS**\n\n{modulesMapping}";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(modulesText), cancellationToken);

                    break;
                case DURATION:
                    string duration = "Las clases del presente Programa de Especialización Profesional " +
                        "se llevarán a cabo de lunes a viernes, del 22 de enero al 02 de marzo de 2018, " +
                        "en el horario de 3:00 a 7:00 pm";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(duration), cancellationToken);

                    break;
                case INVESTMENT:
                    var investmentLines = new string[]
                    {
                        "Inscripción : S/ 100.00",
                        "Costo del Programa al contado : S/ 2,000.00",
                        "Costo del Programa en 3 partes : S/. 2,100.00"
                    };

                    string investmentMapping = string.Join(string.Empty, investmentLines.Select(m => $"* {m}"));

                    string investmentText = $"**COSTOS**\n\n{investmentMapping}";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(investmentText), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }
    }
}
