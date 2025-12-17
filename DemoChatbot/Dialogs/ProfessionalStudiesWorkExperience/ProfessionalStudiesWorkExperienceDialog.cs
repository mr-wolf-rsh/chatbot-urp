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
    public class ProfessionalStudiesWorkExperienceDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(ProfessionalStudiesWorkExperienceDialog);

        public const string SUBTITLE = "EPEL";

        protected readonly ILogger Logger;

        private const string EPEL_SELECTED = "EPELSelected";
        private const string EPEL_OPTION_SELECTED = "EPELOptionSelected";

        private const string INFORMATION = "Información";
        private const string BENEFITS = "Beneficios";
        private const string ACADEMIC_ACTIVITIES = "Activ. Académicas";
        private const string EXPERIENCES = "Testimonios";
        private const string EPEL_GRADUATE_ASSOCIATION = "Asoc. de Egr. EPEL";
        private const string CONTACT_INFO = "Info. de Contacto";

        private readonly PrivateConversationState _privateConversationState;
        private readonly IStatePropertyAccessor<DialogChoice> _privateConversationAccessor;

        private readonly string[] Choices = new string[]
        {
            INFORMATION,
            BENEFITS,
            ACADEMIC_ACTIVITIES,
            EXPERIENCES,
            EPEL_GRADUATE_ASSOCIATION,
            CONTACT_INFO
        };

        public ProfessionalStudiesWorkExperienceDialog(
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
                EPELStepAsync,
                EPELOptionStepAsync,
                EPELInfoStepAsync,
                DialogHelper.LastGoBackStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> EPELStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Options as IDictionary<string, string> ?? new Dictionary<string, string>();

            stepContext.Values[DialogHelper.OPTIONS_SELECTED] = optionsSelected;

            var dialogChoice = new DialogChoice
            {
                DialogId = DIALOG_ID,
                ChoiceList = new List<string>
                {
                    "Admin. y Gerencia",
                    "Contab. y Finanzas",
                    "Mark. Gl. Adm. Com."
                }
            };

            await _privateConversationAccessor.SetAsync(stepContext.Context, dialogChoice, cancellationToken);

            if (optionsSelected.ContainsKey(EPEL_SELECTED))
            {
                var epelSelected = optionsSelected[EPEL_SELECTED];

                return await stepContext.NextAsync(epelSelected, cancellationToken);
            }

            var epelAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = "Administración y Gerencia",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/400/we/723/he/" +
                            "400/x/-299/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/895/n/005-672x372.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Admin. y Gerencia",
                            Value = "Admin. y Gerencia"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/pregrado/facultad-de-ciencias-economicas-y-empresariales/epel/administracion"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Contab. y Finanzas",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/400/we/601/he/" +
                            "400/x/-101/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/900/n/ico-destinonegocio-finanzas-en-las-empresas-istock-getty-images-1030x685.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Contab. y Finanzas",
                            Value = "Contab. y Finanzas"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/pregrado/facultad-de-ciencias-economicas-y-empresariales/epel/contabilidad-y-finanzas"
                        }
                    }
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Marketing Global y Administración Comercial",
                    Subtitle = SUBTITLE,
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/400/we/600/he/" +
                            "400/x/-100/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/898/n/marketing-global.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = "Mark. Gl. Adm. Com.",
                            Value = "Mark. Gl. Adm. Com."
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ir a sitio web",
                            Value = "https://www.urp.edu.pe/pregrado/facultad-de-ciencias-economicas-y-empresariales/epel/marketing-global-y-administracion-comercial"
                        }
                    }
                }.ToAttachment()
            };

            var messageReply = MessageFactory.Text($"Estas son nuestras opciones en {SUBTITLE}. 📚💯");

            var reply = MessageFactory.Carousel(epelAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> EPELOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

            if (optionsSelected.ContainsKey(EPEL_OPTION_SELECTED))
            {
                var epelOptionSelected = optionsSelected[EPEL_OPTION_SELECTED];

                return await stepContext.NextAsync(epelOptionSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(EPEL_SELECTED))
            {
                optionsSelected.Add(EPEL_SELECTED, resultSelected);
            }

            var epelFlowsOptionAttachments = new List<Attachment>
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
                            Title = BENEFITS,
                            Value = BENEFITS
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = ACADEMIC_ACTIVITIES,
                            Value = ACADEMIC_ACTIVITIES
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = EXPERIENCES,
                            Value = EXPERIENCES
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = EPEL_GRADUATE_ASSOCIATION,
                            Value = EPEL_GRADUATE_ASSOCIATION
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

            var epelLinksAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = resultSelected,
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Sobre Nosotros",
                            Value = "https://www.urp.edu.pe/pregrado/facultad-de-ciencias-economicas-y-empresariales/epel/nosotros"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Ingreso EPEL",
                            Value = "https://www.urp.edu.pe/admision/modalidades-de-ingreso-epel"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Sobre la carrera",
                            Value = "https://www.urp.edu.pe/pregrado/facultad-de-ciencias-economicas-y-empresariales/epel/carrera-de-administracion-y-gerencia"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Noticias Académicas",
                            Value = "https://www.urp.edu.pe/pregrado/facultad-de-ciencias-economicas-y-empresariales/epel/noticias-academicas"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Brochure EPEL",
                            Value = "https://www.urp.edu.pe/pdf/id/32729/n/brochure-epel"
                        }
                    }
                }.ToAttachment()
            };

            var messageReply = MessageFactory.Text($"Tenemos las siguientes opciones para ti en {resultSelected}:");

            var flowsReply = MessageFactory.Attachment(epelFlowsOptionAttachments);

            var linksReply = MessageFactory.Carousel(epelLinksAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, flowsReply, linksReply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> EPELInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(EPEL_SELECTED);

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
                    var info = "La Carrera de **Administración y Gerencia** del **Programa por " +
                        "Experiencia Laboral Certificada EPEL** de la Universidad Ricardo Palma, " +
                        "forma profesionales altamente especializados en las áreas de planeamiento, " +
                        "administración de proyectos de inversión, potencial humano, finanzas y banca de inversión, " +
                        "gestión de empresas y negocios internacionales, con responsabilidad social y una sólida formación humanística.";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);

                    break;
                case BENEFITS:
                    //get from api
                    var benefitsLines = new string[]
                    {
                        "**Solidez Académica**",
                        "La carrera de Administración y Gerencia ha cumplido 51 años de servicio ininterrumpidos " +
                        "en la formación de profesionales altamente especializados y responsable del uso eficiente " +
                        "de los recursos en la empresa, sea ésta privada o pública, nacional o internacional",
                        "**Profesionales Competitivos**",
                        "Porque nuestros egresados ocupan puestos en empresas importantes a nivel nacional e internacional, " +
                        "es decir, seguimos sembrando y cosechando profesionales de talla mundial, " +
                        "sincronizando sus organizaciones a diferentes mercados",
                        "**Formación Especializada**",
                        "Ofrecemos una formación especializada en gerencia en marketing, gerencia en finanzas, " +
                        "gerencia en producción, gerencia en investigación y desarrollo, " +
                        "gerencia en recursos humanos, gestión empresarial, y negocios internacionales"
                    };

                    string benefitsText = string.Join("\n\n", benefitsLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(benefitsText), cancellationToken);

                    break;
                case ACADEMIC_ACTIVITIES:
                    string academicActivitiesTitle = ACADEMIC_ACTIVITIES;

                    var academicActivitiesAttachments = new List<Attachment>
                    {
                        new VideoCard
                        {
                            Title = "CONFERENCIA EPEL: ¿Cómo invertir en remates judiciales?",
                            Subtitle = academicActivitiesTitle,
                            Image = new ThumbnailUrl
                            {
                                Url = "https://i.ytimg.com/vi/s3CO5wILNXs/sddefault.jpg",
                            },
                            Media = new List<MediaUrl>
                            {
                                new MediaUrl()
                                {
                                    Url = "https://www.youtube.com/watch?v=s3CO5wILNXs",
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction()
                                {
                                    Title = "Ver más",
                                    Type = ActionTypes.OpenUrl,
                                    Value = "https://www.youtube.com/watch?v=s3CO5wILNXs",
                                },
                            },
                        }.ToAttachment(),
                        new VideoCard
                        {
                            Title = "CONFERENCIA EPEL: ¿Cómo gestionar proyectos sociales de vivienda?",
                            Subtitle = academicActivitiesTitle,
                            Image = new ThumbnailUrl
                            {
                                Url = "https://i.ytimg.com/vi/eCMa4HQ4hzc/sddefault.jpg",
                            },
                            Media = new List<MediaUrl>
                            {
                                new MediaUrl()
                                {
                                    Url = "https://www.youtube.com/watch?v=eCMa4HQ4hzc",
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction()
                                {
                                    Title = "Ver más",
                                    Type = ActionTypes.OpenUrl,
                                    Value = "https://www.youtube.com/watch?v=eCMa4HQ4hzc",
                                },
                            },
                        }.ToAttachment(),
                        new VideoCard
                        {
                            Title = "Conferencia: El Rol del Empresario en la Economía Moderna",
                            Subtitle = academicActivitiesTitle,
                            Image = new ThumbnailUrl
                            {
                                Url = "https://i.ytimg.com/vi_webp/XnBQkejht4A/sddefault.webp",
                            },
                            Media = new List<MediaUrl>
                            {
                                new MediaUrl()
                                {
                                    Url = "https://www.youtube.com/watch?v=XnBQkejht4A",
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction()
                                {
                                    Title = "Ver más",
                                    Type = ActionTypes.OpenUrl,
                                    Value = "https://www.youtube.com/watch?v=XnBQkejht4A",
                                },
                            },
                        }.ToAttachment(),
                        new VideoCard
                        {
                            Title = "Conferencia: Competencia Portuaria en América Latina y el Mundo",
                            Subtitle = academicActivitiesTitle,
                            Image = new ThumbnailUrl
                            {
                                Url = "https://i.ytimg.com/vi_webp/X9Qx7lP8C8Q/sddefault.webp",
                            },
                            Media = new List<MediaUrl>
                            {
                                new MediaUrl()
                                {
                                    Url = "https://www.youtube.com/watch?v=X9Qx7lP8C8Q",
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction()
                                {
                                    Title = "Ver más",
                                    Type = ActionTypes.OpenUrl,
                                    Value = "https://www.youtube.com/watch?v=X9Qx7lP8C8Q",
                                },
                            },
                        }.ToAttachment(),
                        new VideoCard
                        {
                            Title = "Conferencia: La Estructura Económica y Social y " +
                            "las limitaciones del Estado frente a la Pandemia",
                            Subtitle = academicActivitiesTitle,
                            Image = new ThumbnailUrl
                            {
                                Url = "https://i.ytimg.com/vi_webp/5gLf7GRsmB4/sddefault.webp",
                            },
                            Media = new List<MediaUrl>
                            {
                                new MediaUrl()
                                {
                                    Url = "https://www.youtube.com/watch?v=5gLf7GRsmB4",
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction()
                                {
                                    Title = "Ver más",
                                    Type = ActionTypes.OpenUrl,
                                    Value = "https://www.youtube.com/watch?v=5gLf7GRsmB4",
                                },
                            },
                        }.ToAttachment(),
                        new VideoCard
                        {
                            Title = "Conferencia: Aldea Digitales y sus Beneficios para las Pymes",
                            Subtitle = academicActivitiesTitle,
                            Image = new ThumbnailUrl
                            {
                                Url = "https://i.ytimg.com/vi/GXHBNqehq5c/sddefault.jpg",
                            },
                            Media = new List<MediaUrl>
                            {
                                new MediaUrl()
                                {
                                    Url = "https://www.youtube.com/watch?v=GXHBNqehq5c",
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction()
                                {
                                    Title = "Ver más",
                                    Type = ActionTypes.OpenUrl,
                                    Value = "https://www.youtube.com/watch?v=GXHBNqehq5c",
                                },
                            },
                        }.ToAttachment(),
                        new VideoCard
                        {
                            Title = "Conferencia: Finanzas Sostenibles. " +
                            "Herramientas para una Reactivación Económica Inclusiva",
                            Subtitle = academicActivitiesTitle,
                            Image = new ThumbnailUrl
                            {
                                Url = "https://i.ytimg.com/vi/XnICoCjz3kA/sddefault.jpg",
                            },
                            Media = new List<MediaUrl>
                            {
                                new MediaUrl()
                                {
                                    Url = "https://www.youtube.com/watch?v=XnICoCjz3kA",
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction()
                                {
                                    Title = "Ver más",
                                    Type = ActionTypes.OpenUrl,
                                    Value = "https://www.youtube.com/watch?v=XnICoCjz3kA",
                                },
                            },
                        }.ToAttachment()
                    };

                    await stepContext.Context.SendActivityAsync(MessageFactory.Carousel(academicActivitiesAttachments), cancellationToken);

                    break;
                case EXPERIENCES:
                    string experiencesTitle = EXPERIENCES;

                    var experiencesAttachments = new List<Attachment>
                    {
                        new VideoCard
                        {
                            Title = "TESTIMONIO DE EGRESADO DE LA CARRERA DE ADMINISTRACIÓN Y GERENCIA - EPEL URP",
                            Subtitle = experiencesTitle,
                            Image = new ThumbnailUrl
                            {
                                Url = "https://i.ytimg.com/vi_webp/aFmCH2WntQM/sddefault.webp"
                            },
                            Media = new List<MediaUrl>
                            {
                                new MediaUrl()
                                {
                                    Url = "https://www.youtube.com/watch?v=aFmCH2WntQM"
                                }
                            },
                            Buttons = new List<CardAction>
                            {
                                new CardAction()
                                {
                                    Title = "Ver más",
                                    Type = ActionTypes.OpenUrl,
                                    Value = "https://www.youtube.com/watch?v=aFmCH2WntQM"
                                }
                            },
                        }.ToAttachment()
                    };

                    await stepContext.Context.SendActivityAsync(MessageFactory.Carousel(experiencesAttachments), cancellationToken);

                    break;
                case EPEL_GRADUATE_ASSOCIATION:
                    var graduateAssociationVideoUrl = "https://www.youtube.com/watch?v=GLFKy6LV30U";

                    string graduateAssociationTitle = EPEL_GRADUATE_ASSOCIATION;

                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.ContentUrl(graduateAssociationVideoUrl, "video/mp4",
                        graduateAssociationTitle, graduateAssociationTitle), cancellationToken);

                    break;
                case CONTACT_INFO:
                    var contactInfoLines = new string[]
                    {
                        $"**Teléfono:** {"708-0000"}",
                        $"**Anexo:** {"0600 / 0601 / 0602 / 0603 / 0604 / 0605 / 0606 / 0607 / 0608"}",
                        $"**Correo:** {"inscripcionepel@urp.edu.pe"}",
                        $"**Celular:** {"996-032-105 / 953-108-904"}"
                    };

                    string contactInfoText = string.Join("\n\n", contactInfoLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(contactInfoText), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }
    }
}
