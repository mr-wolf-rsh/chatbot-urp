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
    public class SchoolDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(SchoolDialog);

        protected readonly ILogger Logger;

        private const string SCHOOL_SELECTED = "SchoolSelected";
        private const string SCHOOL_OPTION_SELECTED = "SchoolOptionSelected";

        private const string INFORMATION = "Información";
        private const string LABORATORIES = "Laboratorios";

        private readonly PrivateConversationState _privateConversationState;
        private readonly IStatePropertyAccessor<DialogChoice> _privateConversationAccessor;

        private readonly string[] Choices = new string[]
        {
            INFORMATION,
            LABORATORIES
        };

        public SchoolDialog(
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
                SchoolStepAsync,
                SchoolOptionStepAsync,
                SchoolInfoStepAsync,
                DialogHelper.LastGoBackStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> SchoolStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Options as IDictionary<string, string>;

            stepContext.Values[DialogHelper.OPTIONS_SELECTED] = optionsSelected;

            var dialogChoice = new DialogChoice
            {
                DialogId = DIALOG_ID,
                ChoiceList = new List<string>
                {
                    "Ing. Civil",
                    "Ing. Electrónica",
                    "Ing. Industrial",
                    "Ing. Informática",
                    "Ing. Mecatrónica"
                }
            };

            await _privateConversationAccessor.SetAsync(stepContext.Context, dialogChoice, cancellationToken);

            if (optionsSelected.ContainsKey(SCHOOL_SELECTED))
            {
                var schoolSelected = optionsSelected[SCHOOL_SELECTED];

                return await stepContext.NextAsync(schoolSelected, cancellationToken);
            }

            var schoolAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = "Ingeniería Civil",
                    Subtitle = UndergraduateCareerDialog.SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/300/we/659/he/" +
                            "439/x/-47/y/-139/s/0/q/60/zc/3/f/0/rgb/000000/src/7276/n/2.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Ing. Civil",
                            Value = "Ing. Civil"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/pregrado/facultad-de-ingenieria/escuelas/ingenieria-civil"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Ingeniería Electrónica",
                    Subtitle = UndergraduateCareerDialog.SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/300/we/547/he/" +
                            "363/x/-147/y/-63/s/0/q/60/zc/3/f/0/rgb/000000/src/1828/n/dsc0090.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Ing. Electrónica",
                            Value = "Ing. Electrónica"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/pregrado/facultad-de-ingenieria/escuelas/ingenieria-electronica"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Ingeniería Industrial",
                    Subtitle = UndergraduateCareerDialog.SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/300/we/451/he/" +
                            "300/x/-26/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/636/n/programas-y-cursosok.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Ing. Industrial",
                            Value = "Ing. Industrial"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/pregrado/facultad-de-ingenieria/escuelas/ingenieria-industrial"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Ingeniería Informática",
                    Subtitle = UndergraduateCareerDialog.SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/300/we/450/he/" +
                            "300/x/-25/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/672/n/dqpx0799.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Ing. Informática",
                            Value = "Ing. Informática"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/pregrado/facultad-de-ingenieria/escuelas/ingenieria-informatica"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Ingeniería Mecatrónica",
                    Subtitle = UndergraduateCareerDialog.SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/300/we/451/he/" +
                            "300/x/-26/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/671/n/dsc0108ok.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Ing. Mecatrónica",
                            Value = "Ing. Mecatrónica"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/pregrado/facultad-de-ingenieria/escuelas/ingenieria-mecatronica"
                        }
                    }
                }.ToAttachment()
            };

            var facultySelected = optionsSelected[UndergraduateCareerDialog.FACULTY_SELECTED];

            var messageReply = MessageFactory.Text($"Estas son las escuelas de {facultySelected}:");

            var reply = MessageFactory.Carousel(schoolAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> SchoolOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                return await stepContext.ReplaceDialogAsync(
                    UndergraduateCareerDialog.DIALOG_ID,
                    optionsSelected,
                    cancellationToken);
            }

            var previousDialogChoice = await _privateConversationAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            if (previousDialogChoice == null || !previousDialogChoice.ChoiceList.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            if (optionsSelected.ContainsKey(SCHOOL_OPTION_SELECTED))
            {
                var schoolOptionSelected = optionsSelected[SCHOOL_OPTION_SELECTED];

                return await stepContext.NextAsync(schoolOptionSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(SCHOOL_SELECTED))
            {
                optionsSelected.Add(SCHOOL_SELECTED, resultSelected);
            }

            // use api
            var schoolOptionAttachments = new List<Attachment>
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
                            Title = LABORATORIES,
                            Value = LABORATORIES
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Plan de estudios",
                            Value = "https://www.urp.edu.pe/pdf/id/2923/n/plan-de-estudios-2015-ii"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Malla curricular",
                            Value = "https://www.urp.edu.pe/pdf/id/2925/n/malla-curricular-2015-ii"
                        }
                    }
                }.ToAttachment()
            };

            var messageReply = MessageFactory.Text($"Estas son las opciones en {resultSelected}. 📖📚");

            var reply = MessageFactory.Attachment(schoolOptionAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> SchoolInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(SCHOOL_SELECTED);

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
                    var info = "La Escuela Profesional de Ingeniería Informática es " +
                        "una unidad académica de la Facultad de Ingeniería de la Universidad Ricardo Palma, " +
                        "dedicada a la formación de profesionales de ingeniería Informática, " +
                        "en el ámbito de sus especialidades, líderes, emprendedores, innovadores y " +
                        "competitivos globalmente, con base: científica, técnica, humanista y compromiso " +
                        "con la responsabilidad social y ambiental con proyección al estudio, investigación y " +
                        "propuesta de soluciones a los problemas técnicos-económicos y sociales del país.";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);

                    break;
                case LABORATORIES:
                    //get from api
                    var laboratories = new HeroCard
                    {
                        Title = LABORATORIES,
                        Buttons = new List<CardAction>
                        {
                            new CardAction
                            {
                                Type = ActionTypes.OpenUrl,
                                Title = "Lab. de Física",
                                Value = "https://www.urp.edu.pe/pregrado/facultad-de-ingenieria/laboratorios/laboratorio-de-fisica"
                            },
                            new CardAction
                            {
                                Type = ActionTypes.OpenUrl,
                                Title = "Lab. de Informática",
                                Value = "https://www.urp.edu.pe/pregrado/facultad-de-ingenieria/laboratorios/laboratorio-de-informatica"
                            },
                            new CardAction
                            {
                                Type = ActionTypes.OpenUrl,
                                Title = "Lab. de Química",
                                Value = "https://www.urp.edu.pe/pregrado/facultad-de-ingenieria/laboratorios/laboratorio-de-quimica"
                            }
                        }
                    }.ToAttachment();

                    await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(laboratories), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }
    }
}
