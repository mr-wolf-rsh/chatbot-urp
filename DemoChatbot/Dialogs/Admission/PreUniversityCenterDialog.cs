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
    public class PreUniversityCenterDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(PreUniversityCenterDialog);

        public const string SUBTITLE = "Centro Preuniversitario";

        protected readonly ILogger Logger;

        private const string INFORMATION = "Información";
        private const string PROFILE = "Perfil (Objetivo)";
        private const string BENEFITS = "Beneficios";
        private const string CONTACT_INFO = "Info. de Contacto";

        private readonly string[] Choices = new string[]
        {
            INFORMATION,
            PROFILE,
            BENEFITS,
            CONTACT_INFO
        };

        public PreUniversityCenterDialog(
            UserState userState,
            ILogger<MainDialog> logger,
            IBotTelemetryClient telemetryClient,
            string dialogId = DIALOG_ID) : base(dialogId)
        {
            Logger = logger;
            TelemetryClient = telemetryClient;

            var waterfallSteps = new WaterfallStep[]
            {
                PreUniversityCenterStepAsync,
                PreUniversityCenterOptionStepAsync,
                DialogHelper.LastGoBackStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> PreUniversityCenterStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var preUniversityCenterAttachments = new List<Attachment>
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
                            Title = PROFILE,
                            Value = PROFILE
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = BENEFITS,
                            Value = BENEFITS
                        },
                        new CardAction
                        {
                            Type = ActionTypes.OpenUrl,
                            Title = "Aula Virtual",
                            Value = "https://ricardo-palma.mrooms.net"
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

            var messageReply = MessageFactory.Text($"Estas son las opciones en {SUBTITLE}. 💯");

            var reply = MessageFactory.Attachment(preUniversityCenterAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> PreUniversityCenterOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                return await stepContext.ReplaceDialogAsync(AdmissionDialog.DIALOG_ID, cancellationToken: cancellationToken);
            }

            if (!Choices.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            switch (resultSelected)
            {
                case INFORMATION:
                    //get from api
                    var info = "En el **CEPURP** consideramos de suma importancia el apoyo que los padres de familia " +
                        "deben dar a sus hijos durante su formación preuniversitaria debido a las características especiales " +
                        "de su desarrollo personal que están viviendo, es por ello que lo hacemos participar en reuniones y talleres con el director, " +
                        "los profesores y las tutoras. Establecemos una comunicación efectiva entre el alumno, el Centro Preuniversitario y la familia.";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);

                    break;
                case PROFILE:
                    string profile = "El trabajo que se realiza hace posible que el alumno " +
                        "que culmina un ciclo de formación en el CEPURP, tenga el siguiente perfil:";

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(profile), cancellationToken);

                    var profileInfoLines = new string[]
                    {
                        "Vocación profesional definida",
                        "Conocimiento de las exigencias, cursos y campo profesional de la carrera profesional a la que ingresa",
                        "Formación adecuada en cursos básicos de la carrera elegida",
                        "Motivado para iniciar sus estudios universitarios",
                        "Identificado con la URP"
                    };

                    var profileInfoMapped = profileInfoLines.Select(profileInfo => $"- {profileInfo}");

                    string profileInfoText = string.Join("\n\n", profileInfoMapped);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(profileInfoText), cancellationToken);

                    break;
                case BENEFITS:
                    var benefitsInfoLines = new string[]
                    {
                        "Ambientes especiales para obtener la mejor preparación Preuniversitaria",
                        "Plana docente seleccionada",
                        "Asesoramiento personal especializado",
                        "Conferencias específicas por carrera",
                        "Elección en orden de prioridad de dos (02) opciones de carreras que ofrece la Universidad",
                        "Programa de complementación académica: Apoyo a la optimización de los procesos de estudio y aprendizaje",
                        "Apoyo psicológico permanente",
                        "Comunicación permanente con los padres de familia o apoderados",
                        "Olimpiadas académicas y deportivas"
                    };

                    var benefitsInfoMapped = benefitsInfoLines.Select(benefitsInfo => $"- {benefitsInfo}");

                    string benefitsInfoText = string.Join("\n\n", benefitsInfoMapped);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(benefitsInfoText), cancellationToken);

                    break;
                case CONTACT_INFO:
                    var contactInfoLines = new string[]
                    {
                        $"**Dirección:** {"Av. Armendáriz 349 - Miraflores"}",
                        $"**Teléfono:** {"445-9890 / 241-2797"}",
                        $"**Correo:** {"cepurp@urp.edu.pe"}"
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
