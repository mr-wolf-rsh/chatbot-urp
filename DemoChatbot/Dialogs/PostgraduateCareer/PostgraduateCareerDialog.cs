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
    public class PostgraduateCareerDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(PostgraduateCareerDialog);

        public const string SUBTITLE = "Escuela de Posgrado";

        protected readonly ILogger Logger;

        private const string DEGREE_SELECTED = "DegreeSelected";
        private const string DEGREE_OPTION_SELECTED = "DegreeOptionSelected";
        private const string DEGREE_INFO_SELECTED = "DegreeInfoSelected";

        private const string DOCTORATES = "Doctorados";
        private const string MASTERS = "Maestrías";
        private const string INFORMATION = "Información";
        private const string LINE_OF_INVESTIGATION = "Líneas de Invest.";
        private const string CURRICULAR_MESH = "Malla curricular";
        private const string CURRICULUM = "Plan de estudios";
        private const string SCHEDULE_INVESTMENT_REQUIREMENTS = "Hor., Inv. y Req.";
        private const string CONTACT_INFO = "Info. de Contacto";

        private readonly PrivateConversationState _privateConversationState;
        private readonly IStatePropertyAccessor<DialogChoice> _privateConversationAccessor;

        private readonly string[] Choices = new string[]
        {
            DOCTORATES,
            MASTERS,
            INFORMATION,
            LINE_OF_INVESTIGATION,
            CURRICULAR_MESH,
            CURRICULUM,
            SCHEDULE_INVESTMENT_REQUIREMENTS,
            CONTACT_INFO
        };

        public PostgraduateCareerDialog(
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
                DegreeStepAsync,
                DegreeOptionStepAsync,
                DegreeInfoStepAsync,
                DegreeInfoOptionStepAsync,
                DialogHelper.LastGoBackStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> DegreeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Options as IDictionary<string, string> ?? new Dictionary<string, string>();

            stepContext.Values[DialogHelper.OPTIONS_SELECTED] = optionsSelected;

            if (optionsSelected.ContainsKey(DEGREE_SELECTED))
            {
                return await stepContext.NextAsync(optionsSelected[DEGREE_SELECTED], cancellationToken);
            }

            var degreeAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = DOCTORATES,
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
                            Title = DOCTORATES,
                            Value = DOCTORATES
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = MASTERS,
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
                            Title = MASTERS,
                            Value = MASTERS
                        }
                    }
                }.ToAttachment()
            };

            var messageReply = MessageFactory.Text($"Estas son nuestras opciones en {SUBTITLE}. 🎓");

            var reply = MessageFactory.Carousel(degreeAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> DegreeOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                return await DialogHelper.ReplaceMainMenuDialogAsync(stepContext, cancellationToken);
            }

            if (!Choices.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            switch (resultSelected)
            {
                case DOCTORATES:
                    var doctorateChoice = new DialogChoice
                    {
                        DialogId = DIALOG_ID,
                        ChoiceList = new List<string>
                        {
                            "Adm. de Neg. Glob.",
                            "C. Pol. y Rel. Int."
                        }
                    };

                    await _privateConversationAccessor.SetAsync(stepContext.Context, doctorateChoice, cancellationToken);

                    break;
                case MASTERS:
                    var mastersChoice = new DialogChoice
                    {
                        DialogId = DIALOG_ID,
                        ChoiceList = new List<string>
                        {
                            "Docencia Superior",
                            "Ing. Inf. M. I. Sw.",
                            "Arq. Menc. G. Emp."
                        }
                    };

                    await _privateConversationAccessor.SetAsync(stepContext.Context, mastersChoice, cancellationToken);

                    break;
            }

            if (optionsSelected.ContainsKey(DEGREE_OPTION_SELECTED))
            {
                var degreeOptionSelected = optionsSelected[DEGREE_OPTION_SELECTED];

                return await stepContext.NextAsync(degreeOptionSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(DEGREE_SELECTED))
            {
                optionsSelected.Add(DEGREE_SELECTED, resultSelected);
            }

            switch (resultSelected)
            {
                case DOCTORATES:
                    var doctorateAttachments = new List<Attachment>
                    {
                        new HeroCard
                        {
                            Title = "Administración de Negocios Globales",
                            Subtitle = SUBTITLE,
                            Images = new List<CardImage>
                            {
                                new CardImage
                                {
                                    Url = "https://www.urp.edu.pe/img/thumbnails/wm/868/hm/651/we/976/he/" +
                                    "651/x/-54/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/696/n/20.jpg"
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = "Adm. de Neg. Glob.",
                                    Value = "Adm. de Neg. Glob."
                                },
                                new CardAction
                                {
                                    Type = ActionTypes.OpenUrl,
                                    Title = "Ir a sitio web",
                                    Value = "https://www.urp.edu.pe/posgrado/doctorados/administracion-de-negocios-globales"
                                }
                            }
                        }.ToAttachment(),
                        new HeroCard
                        {
                            Title = "Ciencia Política y Relaciones Internacionales",
                            Subtitle = SUBTITLE,
                            Images = new List<CardImage>
                            {
                                new CardImage
                                {
                                    Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/300/we/983/he/" +
                                    "300/x/-195/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/687/n/10.jpg"
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = "C. Pol. y Rel. Int.",
                                    Value = "C. Pol. y Rel. Int."
                                },
                                new CardAction
                                {
                                    Type = ActionTypes.OpenUrl,
                                    Title = "Ir a sitio web",
                                    Value = "https://www.urp.edu.pe/posgrado/doctorados/ciencia-politica-y-relaciones-internacionales"
                                }
                            }
                        }.ToAttachment()
                    };

                    var doctorateMessageReply = MessageFactory.Text($"🎓 Sobre {DOCTORATES}:");

                    var doctorateReply = MessageFactory.Carousel(doctorateAttachments);

                    await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, doctorateMessageReply, doctorateReply);

                    return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
                case MASTERS:
                    var mastersAttachments = new List<Attachment>
                    {
                        new HeroCard
                        {
                            Title = "Docencia Superior",
                            Subtitle = SUBTITLE,
                            Images = new List<CardImage>
                            {
                                new CardImage
                                {
                                    Url = "https://www.urp.edu.pe/img/thumbnails/wm/1000/hm/500/we/1000/he/" +
                                    "723/x/0/y/-124/s/0/q/60/zc/3/f/0/rgb/000000/src/742/n/23.jpg"
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = "Docencia Superior",
                                    Value = "Docencia Superior"
                                },
                                new CardAction
                                {
                                    Type = ActionTypes.OpenUrl,
                                    Title = "Ir a sitio web",
                                    Value = "https://www.urp.edu.pe/posgrado/maestrias/docencia-superior"
                                }
                            }
                        }.ToAttachment(),
                        new HeroCard
                        {
                            Title = "Ingeniería Informática con Mención en Ingeniería de Software",
                            Subtitle = SUBTITLE,
                            Images = new List<CardImage>
                            {
                                new CardImage
                                {
                                    Url = "https://www.urp.edu.pe/img/thumbnails/wm/800/hm/400/we/872/he/" +
                                    "400/x/-36/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/762/n/images-9.jpg"
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = "Ing. Inf. M. I. Sw.",
                                    Value = "Ing. Inf. M. I. Sw."
                                },
                                new CardAction
                                {
                                    Type = ActionTypes.OpenUrl,
                                    Title = "Ir a sitio web",
                                    Value = "https://www.urp.edu.pe/posgrado/maestrias/ingenieria-informatica-con-mencion-en-ingenieria-de-software"
                                }
                            }
                        }.ToAttachment(),
                        new HeroCard
                        {
                            Title = "Arquitectura con Mención en Gestión Empresarial",
                            Subtitle = SUBTITLE,
                            Images = new List<CardImage>
                            {
                                new CardImage
                                {
                                    Url = "https://www.urp.edu.pe/img/thumbnails/wm/1600/hm/500/we/1600/he/" +
                                    "900/x/0/y/-166/s/0/q/60/zc/3/f/0/rgb/000000/src/750/n/31.jpg"
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = "Arq. Menc. G. Emp.",
                                    Value = "Arq. Menc. G. Emp."
                                },
                                new CardAction
                                {
                                    Type = ActionTypes.OpenUrl,
                                    Title = "Ir a sitio web",
                                    Value = "https://www.urp.edu.pe/posgrado/maestrias/arquitectura-con-mencion-en-gestion-empresarial"
                                }
                            }
                        }.ToAttachment()
                    };

                    var mastersMessageReply = MessageFactory.Text($"🎓 Sobre {MASTERS}:");

                    var mastersReply = MessageFactory.Carousel(mastersAttachments);

                    await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, mastersMessageReply, mastersReply);

                    return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
                default:
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

        private async Task<DialogTurnResult> DegreeInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(DEGREE_SELECTED);

                return await stepContext.ReplaceDialogAsync(DIALOG_ID, optionsSelected, cancellationToken);
            }

            var previousDialogChoice = await _privateConversationAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            if (previousDialogChoice == null || !previousDialogChoice.ChoiceList.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            if (optionsSelected.ContainsKey(DEGREE_INFO_SELECTED))
            {
                var degreeOptionSelected = optionsSelected[DEGREE_INFO_SELECTED];

                return await stepContext.NextAsync(degreeOptionSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(DEGREE_OPTION_SELECTED))
            {
                optionsSelected.Add(DEGREE_OPTION_SELECTED, resultSelected);
            }

            var degreeOptionsAttachments = new List<Attachment>
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
                            Title = LINE_OF_INVESTIGATION,
                            Value = LINE_OF_INVESTIGATION
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = CURRICULAR_MESH,
                            Value = CURRICULAR_MESH
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = CURRICULUM,
                            Value = CURRICULUM
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = SCHEDULE_INVESTMENT_REQUIREMENTS,
                            Value = SCHEDULE_INVESTMENT_REQUIREMENTS
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

            var messageReply = MessageFactory.Text($"Tenemos las siguientes opciones para ti en {resultSelected}:");

            var reply = MessageFactory.Attachment(degreeOptionsAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> DegreeInfoOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(DEGREE_OPTION_SELECTED);

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
                    var info = "La Maestría en Ingeniería Informática con mención en Ingeniería de Software " +
                        "brinda formación académico profesional que conduce a la investigación e innovación " +
                        "en los diferentes campos de la Ingeniería de Software y de sus aplicaciones; " +
                        "impartiendo conocimientos teóricos y técnico evolutivos como base de futuras competencias de innovador.";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);

                    break;
                case LINE_OF_INVESTIGATION:
                    var linesOfResearch = "Entre las líneas de investigación se encuentran:";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(linesOfResearch), cancellationToken);

                    var linesOfResearchInfoLines = new string[]
                    {
                        "Área de Ciencias de la Computación y Tecnologías de Información",
                        "Ingeniería de Software"
                    };

                    var linesOfResearchInfoMapped = linesOfResearchInfoLines.Select(linesOfResearchInfo => $"- {linesOfResearchInfo}");

                    string linesOfResearchInfoText = string.Join("\n\n", linesOfResearchInfoMapped);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(linesOfResearchInfoText), cancellationToken);

                    break;
                case CURRICULAR_MESH:
                    var coursesUrl = "https://www.urp.edu.pe/img/thumbnails/wm/1430/hm/182/we/1430/he/" +
                            "182/x/0/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/28750/n/anexos-epg.jpg";

                    string coursesTitle = CURRICULAR_MESH;

                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.ContentUrl(coursesUrl, "image/jpeg",
                        coursesTitle, coursesTitle), cancellationToken);

                    break;
                case CURRICULUM:
                    var studiesPlanUrl = "https://www.urp.edu.pe/img/thumbnails/wm/555/hm/488/we/555/he/" +
                        "490/x/0/y/-1/s/0/q/60/zc/3/f/0/rgb/000000/src/2977/n/plan-ingenieria-informatica.jpg";

                    string studiesPlanTitle = CURRICULUM;

                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.ContentUrl(studiesPlanUrl, "image/jpeg",
                        studiesPlanTitle, studiesPlanTitle), cancellationToken);

                    break;
                case SCHEDULE_INVESTMENT_REQUIREMENTS:
                    var scheduleInfoTitle = "**Horario de clases**\n\n" +
                        "Las clases son dos (02) veces por semana los días Sábado y Domingo de 08.00 hrs. a 14.00 hrs.\n\n";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(scheduleInfoTitle), cancellationToken);

                    var scheduleInfoLines = new string[]
                    {
                        "Inscripción (pago por unica vez): S/.150.00",
                        "Matricula (al inicio de cada semestre): S/.200.00",
                        "Pensión mensual: S/.800.00 (05 Boletas por cada semestre académico)",
                        "Constancia de ingreso: S/. 30.00"
                    };

                    var scheduleInfoMapped = scheduleInfoLines.Select(scheduleInfo => $"- {scheduleInfo}");

                    string scheduleInfoText = "**Inversión**\n\n" + string.Join("\n\n", scheduleInfoMapped);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(scheduleInfoText), cancellationToken);

                    var requirementsAttachment = new HeroCard
                    {
                        Title = SCHEDULE_INVESTMENT_REQUIREMENTS,
                        Buttons = DialogHelper.DegreeSchoolButton("Ver requisitos de Admisión")
                    }.ToAttachment();

                    await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(requirementsAttachment), cancellationToken);

                    break;
                case CONTACT_INFO:
                    var contactInfoLines = new string[]
                    {
                        $"**Coordinador de la Maestría:** {"Mg. Olga Suarez Avelino"}",
                        $"**Celular:** {"978 488 574"}",
                        $"**Correo:** {"osuarez@urp.edu.pe"}"
                    };

                    string contactInfoText = string.Join("\n\n", contactInfoLines);

                    var contactInfoAttachment = new ThumbnailCard
                    {
                        Images = new List<CardImage>
                        {
                            new CardImage
                            {
                                Url = "https://www.urp.edu.pe/img/thumbnails/wm/1430/hm/182/we/1430/he/" +
                                "182/x/0/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/28750/n/anexos-epg.jpg"
                            }
                        }
                    }.ToAttachment();

                    await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(contactInfoAttachment, contactInfoText), cancellationToken);

                    var scheduleAttachment = new HeroCard
                    {
                        Title = CONTACT_INFO,
                        Buttons = DialogHelper.DegreeSchoolButton("Ver cronograma de Admisión")
                    }.ToAttachment();

                    await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(scheduleAttachment), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }
    }
}
