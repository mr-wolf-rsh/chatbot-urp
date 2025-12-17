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
    public class CentralAdmissionOfficeDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(CentralAdmissionOfficeDialog);

        public const string SUBTITLE = "Oficina Central de Admisión";

        protected readonly ILogger Logger;

        private const string CENTRAL_ADMISSION_OFFICE_SELECTED = "CentralAdmissionOfficeSelected";
        private const string CENTRAL_ADMISSION_OFFICE_OPTION_SELECTED = "CentralAdmissionOfficeOptionSelected";

        private const string INFORMATION = "Información";
        private const string REGISTRATIONS = "Inscripciones";
        private const string ACADEMIC_TERM = "Calendario Académ.";
        private const string EDUCATIONAL_INSTITUTIONS = "Inst. Educativas";
        private const string CAREERS = "Carreras disp.";
        private const string CONTACT_INFO = "Info. de Contacto";
        private const string EDUCATIONAL_INSTITUTIONS_INFO = "Info. Inst. Educ.";
        private const string EDUCATIONAL_INSTITUTIONS_IMAGES = "Imág. de Inst. Educ.";
        private const string EDUCATIONAL_INSTITUTIONS_CONTACT_INFO = "Contac. Inst. Educ.";

        private readonly string[] Choices = new string[]
        {
            INFORMATION,
            REGISTRATIONS,
            ACADEMIC_TERM,
            EDUCATIONAL_INSTITUTIONS,
            CAREERS,
            CONTACT_INFO,
            EDUCATIONAL_INSTITUTIONS_INFO,
            EDUCATIONAL_INSTITUTIONS_IMAGES,
            EDUCATIONAL_INSTITUTIONS_CONTACT_INFO
        };

        public CentralAdmissionOfficeDialog(
            UserState userState,
            ILogger<MainDialog> logger,
            IBotTelemetryClient telemetryClient,
            string dialogId = DIALOG_ID) : base(dialogId)
        {
            Logger = logger;
            TelemetryClient = telemetryClient;

            var waterfallSteps = new WaterfallStep[]
            {
                CentralAdmissionOfficeStepAsync,
                CentralAdmissionOfficeOptionStepAsync,
                CentralAdmissionOfficeInfoStepAsync,
                DialogHelper.LastGoBackStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> CentralAdmissionOfficeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Options as IDictionary<string, string> ?? new Dictionary<string, string>();

            stepContext.Values[DialogHelper.OPTIONS_SELECTED] = optionsSelected;

            if (optionsSelected.ContainsKey(CENTRAL_ADMISSION_OFFICE_SELECTED))
            {
                var ocaSelected = optionsSelected[CENTRAL_ADMISSION_OFFICE_SELECTED];

                return await stepContext.NextAsync(ocaSelected, cancellationToken);
            }

            var ocaAttachments = new List<Attachment>
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
                            Type = ActionTypes.OpenUrl,
                            Title = "Prospecto de Admisión",
                            Value = "https://www.urp.edu.pe/pdf/id/29614/n/prospecto-urp-2021-listo-7-9-20-4-1-comprimido.pdf"
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = REGISTRATIONS,
                            Value = REGISTRATIONS
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = ACADEMIC_TERM,
                            Value = ACADEMIC_TERM
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = EDUCATIONAL_INSTITUTIONS,
                            Value = EDUCATIONAL_INSTITUTIONS
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = CAREERS,
                            Value = CAREERS
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

            var messageReply = MessageFactory.Text($"Estas son las opciones en {SUBTITLE}. 📚💯");

            var reply = MessageFactory.Attachment(ocaAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> CentralAdmissionOfficeOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

            if (optionsSelected.ContainsKey(CENTRAL_ADMISSION_OFFICE_OPTION_SELECTED))
            {
                var ocaOptionSelected = optionsSelected[CENTRAL_ADMISSION_OFFICE_OPTION_SELECTED];

                return await stepContext.NextAsync(ocaOptionSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(CENTRAL_ADMISSION_OFFICE_SELECTED))
            {
                optionsSelected.Add(CENTRAL_ADMISSION_OFFICE_SELECTED, resultSelected);
            }

            switch (resultSelected)
            {
                case INFORMATION:
                    //get from api
                    var info = "La **Oficina Central de Admisión**, integrada por el Centro Preuniversitario (**CEPURP**), " +
                        "tiene como función la implementación y ejecución de los procesos de admisión a nivel de pregrado y posgrado, " +
                        "así como de la difusión y promoción de las carreras profesionales que ofrece la Universidad Ricardo Palma.";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);

                    break;
                case REGISTRATIONS:
                    string registrationsTitle = REGISTRATIONS;

                    var registrationsVideoUrl = "https://www.youtube.com/watch?v=ghRgXh4-rxE";

                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.ContentUrl(registrationsVideoUrl, "video/mp4",
                        registrationsTitle, registrationsTitle), cancellationToken);

                    var registrationsImageUrl = "https://consulta.urp.edu.pe/Admision/imagenes/proceso.jpg";

                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.ContentUrl(registrationsImageUrl, "image/jpeg",
                        registrationsTitle, registrationsTitle), cancellationToken);

                    var registrationsAttachment = new HeroCard
                    {
                        Title = REGISTRATIONS,
                        Buttons = new List<CardAction>
                        {
                            new CardAction
                            {
                                Type = ActionTypes.OpenUrl,
                                Title = "Inscríbete aquí",
                                Value = "https://www.urp.edu.pe/admision/inscribete-aqui"
                            }
                        }
                    }.ToAttachment();

                    await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(registrationsAttachment), cancellationToken);

                    break;
                case ACADEMIC_TERM:
                    var academicTermImageUrl = "https://www.urp.edu.pe/img/thumbnails/wm/868/hm/1228/we/868/he/" +
                        "1228/x/0/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/29610/n/semestre-academico-2021-1.png";

                    string academicTermTitle = ACADEMIC_TERM;

                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.ContentUrl(academicTermImageUrl, "image/jpeg",
                        academicTermTitle, academicTermTitle), cancellationToken);

                    break;
                case EDUCATIONAL_INSTITUTIONS:
                    var educInstAttachments = new List<Attachment>
                    {
                        new HeroCard
                        {
                            Title = EDUCATIONAL_INSTITUTIONS,
                            Buttons = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = EDUCATIONAL_INSTITUTIONS_INFO,
                                    Value = EDUCATIONAL_INSTITUTIONS_INFO
                                },
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = EDUCATIONAL_INSTITUTIONS_IMAGES,
                                    Value = EDUCATIONAL_INSTITUTIONS_IMAGES
                                },
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = EDUCATIONAL_INSTITUTIONS_CONTACT_INFO,
                                    Value = EDUCATIONAL_INSTITUTIONS_CONTACT_INFO
                                }
                            }
                        }.ToAttachment()
                    };

                    var educInstMessageReply = MessageFactory.Text($"Nuestras opciones en {resultSelected}. 📚");

                    var educInstReply = MessageFactory.Attachment(educInstAttachments);

                    await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, educInstMessageReply, educInstReply);

                    return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
                case CAREERS:
                    var careersImageUrl = "https://www.urp.edu.pe/img/thumbnails/wm/501/hm/783/we/501/he/" +
                        "785/x/0/y/-1/s/1/q/100/zc/3/f/0/rgb/000000/src/24130/n/whatsapp-image-2020-07-18-at-11.19.29-am.jpeg";

                    string careersTitle = $"**Estas son nuestras carreras disponibles:**";

                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.ContentUrl(careersImageUrl, "image/jpeg",
                        careersTitle, careersTitle), cancellationToken);

                    break;
                case CONTACT_INFO:
                    var contactInfoLines = new string[]
                    {
                        $"**Teléfono:** {"708-0000"}",
                        $"**Anexo:** {"0600 / 0601 / 0602 / 0603 / 0604"}",
                        $"**Correo:** {"postula@urp.edu.pe"}"
                    };

                    string contactInfoText = string.Join("\n\n", contactInfoLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(contactInfoText), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> CentralAdmissionOfficeInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(CENTRAL_ADMISSION_OFFICE_SELECTED);

                return await stepContext.ReplaceDialogAsync(DIALOG_ID, optionsSelected, cancellationToken);
            }

            if (!Choices.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            switch (resultSelected)
            {
                case EDUCATIONAL_INSTITUTIONS_INFO:
                    var eduInstInfoLines = new string[]
                    {
                        "**PARA INSTITUCIONES EDUCATIVAS Y EMPRESAS**",
                        "Para nuestra participación en una FERIA VOCACIONAL, CHARLA INFORMATIVA o realizar una VISITA GUIADA a la URP " +
                        "deberá de enviar una carta dirigida al Director de la Oficina Central de Admisión (OCA) " +
                        "a la siguiente dirección electrónica: admision@urp.edu.pe",
                        "**VISITA GUIADA**",
                        "Indique la fecha tentativa que desea realizar la visita (por lo menos con 2 semanas de anticipación), " +
                        "indicando el número de alumnos que participarían, así como el nombre de la persona y los números de teléfonos de contacto " +
                        "para realizar las coordinaciones pertinentes. Cabe mencionar que las visitas se realizan de lunes a viernes de 09:00 a 13:00 hrs.",
                        "**FERIAS VOCACIONALES Y CHARLAS INFORMATIVAS**",
                        "Solicite la presencia de un representante de la Universidad, informando la fecha de la actividad " +
                        "(por lo menos con 2 semanas de anticipación), el número de participantes y las características de la misma, " +
                        "así como el nombre y el teléfono de contacto de la persona con la que debemos de realizar las coordinaciones."
                    };

                    string eduInstInfoText = string.Join("\n\n", eduInstInfoLines);

                    var eduInstInfoAttachment = new HeroCard
                    {
                        Title = EDUCATIONAL_INSTITUTIONS_INFO,
                        Images = new List<CardImage>
                        {
                            new CardImage
                            {
                                Url = "https://www.urp.edu.pe/img/thumbnails/wm/600/hm/450/we/800/he/" +
                                "450/x/-100/y/0/s/0/q/60/zc/3/f/0/rgb/000/src/20103/n/"
                            }
                        },
                        Buttons = new List<CardAction>
                        {
                            new CardAction
                            {
                                Type = ActionTypes.OpenUrl,
                                Title = "Ir a sitio web",
                                Value = "https://www.urp.edu.pe/admision/instituciones-educativas"
                            }
                        }
                    }.ToAttachment();

                    await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(eduInstInfoAttachment, eduInstInfoText), cancellationToken);

                    break;
                case EDUCATIONAL_INSTITUTIONS_IMAGES:
                    var eduInstImagesAttachments = new List<Attachment>
                    {
                        new HeroCard
                        {
                            Title = "Galería de imágenes",
                            Images = new List<CardImage>
                            {
                                new CardImage
                                {
                                    Url = "https://www.urp.edu.pe/img/thumbnails/wm/600/hm/450/we/600/he/" +
                                    "450/x/0/y/0/s/0/q/60/zc/3/f/0/rgb/000/src/20102/n/"
                                },
                                new CardImage
                                {
                                    Url = "https://www.urp.edu.pe/img/thumbnails/wm/600/hm/450/we/600/he/" +
                                    "800/x/0/y/-96/s/0/q/60/zc/3/f/0/rgb/000000/src/20098/n/"
                                },
                                new CardImage
                                {
                                    Url = "https://www.urp.edu.pe/img/thumbnails/wm/600/hm/450/we/800/he/" +
                                    "450/x/-100/y/0/s/0/q/60/zc/3/f/0/rgb/000/src/20091/n/"
                                },
                                new CardImage
                                {
                                    Url = "https://www.urp.edu.pe/img/thumbnails/wm/600/hm/450/we/800/he/" +
                                    "450/x/-100/y/0/s/0/q/60/zc/3/f/0/rgb/000/src/20088/n/"
                                }
                            }
                        }.ToAttachment()
                    };

                    await stepContext.Context.SendActivityAsync(MessageFactory.Carousel(eduInstImagesAttachments), cancellationToken);

                    break;
                case EDUCATIONAL_INSTITUTIONS_CONTACT_INFO:
                    var eduInstContactInfoLines = new string[]
                    {
                        $"**Teléfono:** {"708-0000"}",
                        $"**Anexo:** {"0034 / 0600 / 0601 / 0602 / 0603"}",
                        $"**Correo:** {"admision@urp.edu.pe"}"
                    };

                    string contactInfoText = string.Join("\n\n", eduInstContactInfoLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(contactInfoText), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }
    }
}
