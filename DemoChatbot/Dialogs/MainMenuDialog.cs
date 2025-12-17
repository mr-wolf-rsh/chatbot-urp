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
    public class MainMenuDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(MainMenuDialog);

        public const string SUBTITLE = "Universidad Ricardo Palma";

        protected readonly ILogger Logger;

        private const string ADMISSION = "Admisión";
        private const string UNDERGRADUATE_CAREER = "Pregrado";
        private const string POSTGRADUATE_CAREER = "Posgrado";
        private const string PROFESSIONAL_STUDIES_WORK_EXPERIENCE = "EPEL";
        private const string SECOND_SPECIALTY = "Seg. Especialidad";
        private const string PROGRAMS_SPECIALIZATIONS = "Progr. y Espec.";
        private const string STUDENT_SERVICES = "Serv. al Estudiante";

        private readonly UserState _userState;


        private readonly Dictionary<string, string> Choices = new Dictionary<string, string>
        {
            [ADMISSION] = AdmissionDialog.SUBTITLE,
            [UNDERGRADUATE_CAREER] = UndergraduateCareerDialog.SUBTITLE,
            [POSTGRADUATE_CAREER] = PostgraduateCareerDialog.SUBTITLE,
            [PROFESSIONAL_STUDIES_WORK_EXPERIENCE] = ProfessionalStudiesWorkExperienceDialog.SUBTITLE,
            [SECOND_SPECIALTY] = SecondSpecialtyDialog.SUBTITLE,
            [PROGRAMS_SPECIALIZATIONS] = ProgramsSpecializationsDialog.SUBTITLE,
            [STUDENT_SERVICES] = StudentServicesDialog.SUBTITLE
        };

        private readonly Dictionary<string, string> DialogsIds = new Dictionary<string, string>
        {
            [ADMISSION] = AdmissionDialog.DIALOG_ID,
            [UNDERGRADUATE_CAREER] = UndergraduateCareerDialog.DIALOG_ID,
            [POSTGRADUATE_CAREER] = PostgraduateCareerDialog.DIALOG_ID,
            [PROFESSIONAL_STUDIES_WORK_EXPERIENCE] = ProfessionalStudiesWorkExperienceDialog.DIALOG_ID,
            [SECOND_SPECIALTY] = SecondSpecialtyDialog.DIALOG_ID,
            [PROGRAMS_SPECIALIZATIONS] = ProgramsSpecializationsDialog.DIALOG_ID,
            [STUDENT_SERVICES] = StudentServicesDialog.DIALOG_ID
        };

        public MainMenuDialog(
            UserState userState,
            PrivateConversationState privateConversationState,
            ILogger<MainDialog> logger,
            IBotTelemetryClient telemetryClient,
            string dialogId = DIALOG_ID) : base(dialogId)
        {
            Logger = logger;
            TelemetryClient = telemetryClient;

            _userState = userState;

            var waterfallSteps = new WaterfallStep[]
            {
                MenuStepAsync,
                MenuOptionStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            // Custom Dialogs
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

        private async Task<DialogTurnResult> MenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values.Remove(DialogHelper.OPTIONS_SELECTED);

            var delay = new Activity
            {
                Type = ActivityTypes.Delay,
                Value = 500
            };

            await stepContext.Context.SendActivityAsync(delay, cancellationToken);

            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text("¡No olvides que estamos licenciados por SUNEDU y " +
                "contamos con las acreditaciones de ABET, IAC, ICACIT y más! 🙌🎉"), cancellationToken);

            await stepContext.Context.SendActivityAsync(delay, cancellationToken);

            var menuAttachments = new List<Attachment>
            {
                new HeroCard
                {
                    Title = Choices[ADMISSION],
                    Subtitle = SUBTITLE,
                    Text = "¡Entérate sobre el proceso de admisión, inscripciónes, modalidades, CEPURP y más! 📚💯",
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/848/hm/478/we/848/he/" +
                            "527/x/0/y/-25/s/0/q/60/zc/3/f/0/rgb/000000/src/23856/n/admision.png"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = ADMISSION,
                            Value = ADMISSION
                        }
                    },
                }.ToAttachment(),
                new HeroCard
                {
                    Title = Choices[UNDERGRADUATE_CAREER],
                    Subtitle = SUBTITLE,
                    Text = "¡Realiza consultas sobre facultades, carreras, acreditaciones, convenios y más! 📖📓",
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/300/we/451/he/" +
                            "300/x/-26/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/512/n/fac-ingniera.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = UNDERGRADUATE_CAREER,
                            Value = UNDERGRADUATE_CAREER
                        }
                    },
                }.ToAttachment(),
                new HeroCard
                {
                    Title = Choices[POSTGRADUATE_CAREER],
                    Subtitle = SUBTITLE,
                    Text = "¡Realiza consultas sobre doctorados y maestrías, líneas de investigación, entre otros! ‍🎓",
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/400/we/600/he/" +
                            "400/x/-100/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/703/n/informacin-acade.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = POSTGRADUATE_CAREER,
                            Value = POSTGRADUATE_CAREER
                        }
                    },
                }.ToAttachment(),
                new HeroCard
                {
                    Title = Choices[PROFESSIONAL_STUDIES_WORK_EXPERIENCE],
                    Subtitle = SUBTITLE,
                    Text = "¡Entérate acerca de la modalidad de Estudios Profesionales por Experiencia Laboral (EPEL)! 💻📒",
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://scontent.flim16-2.fna.fbcdn.net/v/t1.6435-9/79082161_2521193954594760_6465034552057266176_n.png?" +
                            "_nc_cat=102&ccb=1-5&_nc_sid=973b4a&_nc_ohc=ug3rZYpe1LQAX-Rzo7S&_nc_ht=" +
                            "scontent.flim16-2.fna&oh=4e96e5ff4167222e77d244dcb4187951&oe=61BAC669"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = PROFESSIONAL_STUDIES_WORK_EXPERIENCE,
                            Value = PROFESSIONAL_STUDIES_WORK_EXPERIENCE
                        }
                    },
                }.ToAttachment(),
                new HeroCard
                {
                    Title = Choices[SECOND_SPECIALTY],
                    Subtitle = SUBTITLE,
                    Text = "¡Realiza consultas sobre una segunda especialidad, cronogramas, inversiones y más! 📚📙",
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://scontent.flim16-1.fna.fbcdn.net/v/t1.6435-9/78994894_2521047814609374_8139975448495915008_n.png?" +
                            "_nc_cat=110&ccb=1-5&_nc_sid=730e14&_nc_ohc=hWGJNYMRwbIAX-fFGyH&_nc_ht=" +
                            "scontent.flim16-1.fna&oh=59134ebabe9f37cff376015839c80ea7&oe=61BCC2CE"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = SECOND_SPECIALTY,
                            Value = SECOND_SPECIALTY
                        }
                    },
                }.ToAttachment(),
                new HeroCard
                {
                    Title = Choices[PROGRAMS_SPECIALIZATIONS],
                    Subtitle = SUBTITLE,
                    Text = "¡Realiza consultas sobre los módulos, la duración o informes relacionados! 📙🖋",
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/400/we/601/he/" +
                            "400/x/-101/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/900/n/" +
                            "ico-destinonegocio-finanzas-en-las-empresas-istock-getty-images-1030x685.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = PROGRAMS_SPECIALIZATIONS,
                            Value = PROGRAMS_SPECIALIZATIONS
                        }
                    },
                }.ToAttachment(),
                new HeroCard
                {
                    Title = Choices[STUDENT_SERVICES],
                    Subtitle = SUBTITLE,
                    Text = "¡Realiza consultas sobre becas, bienestar universitario, orientación psicológica y más! 🏫",
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/urpimg/obu/page/cepsi-02.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = STUDENT_SERVICES,
                            Value = STUDENT_SERVICES
                        }
                    },
                }.ToAttachment(),
                new HeroCard
                {
                    Title = "Trámites Académicos y Administrativos",
                    Subtitle = SUBTITLE,
                    Text = "¡Encuentra la información que necesites sobre trámites y procedimientos! 🗃 🖨 ",
                    Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "https://www.urp.edu.pe/img/thumbnails/wm/400/hm/400/we/555/he/" +
                            "400/x/-78/y/0/s/0/q/60/zc/3/f/0/rgb/000000/src/702/n/escuela-posgrado.jpg"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Trámites Académicos y Administrativos",
                            Value = "https://www.urp.edu.pe/pdf/id/29601/n/" +
                            "guia-de-procedimientos-de-tramites-academicos-y-administrativos"
                        }
                    }
                }.ToAttachment()
            };

            var userAccessor = _userState.CreateProperty<UserInfo>(nameof(UserInfo));
            var userInfo = await userAccessor.GetAsync(stepContext.Context, cancellationToken: cancellationToken);

            string name = userInfo?.FirstName ?? "Hola";

            var reply = MessageFactory.Carousel(menuAttachments, $"{name}, tenemos disponibles las siguientes consultas. 😁");

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = (Activity)reply
                },
                cancellationToken
            );
        }

        private async Task<DialogTurnResult> MenuOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string resultSelected = (stepContext.Result as string).Trim();

            if (!Choices.ContainsKey(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            if (DialogsIds.ContainsKey(resultSelected))
            {
                return await stepContext.BeginDialogAsync(DialogsIds[resultSelected], cancellationToken: cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Lo siento, esa no es una opción válida. 😥"), cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
