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
    public class StudentWelfareDialog : ComponentDialog
    {
        public const string DIALOG_ID = nameof(StudentWelfareDialog);

        public const string SUBTITLE = "Bienestar Universitario";

        protected readonly ILogger Logger;

        private const string STUDENT_WELFARE_SELECTED = "StudentWelfareSelected";
        private const string STUDENT_WELFARE_OPTION_SELECTED = "StudentWelfareOptionSelected";

        private const string INFORMATION = "Información";
        private const string PAYMENTS = "Pensiones";
        private const string SCHOLARSHIPS = "Becas";
        private const string MEDICAL_DEPARTMENT = "Departamento Médico";
        private const string PROGRAMS = "Programas";
        private const string CEPSI = "CEPSI";
        private const string INTERNSHIPS = "Práct. Pre-prof.";
        private const string CONTACT_INFO = "Info. de Contacto";
        private const string MEDICAL_DEPARTMENT_INFO = "Info de Dep. Médico";
        private const string MEDICAL_DEPARTMENT_INSURANCE = "Seguro Médico";

        private readonly string[] Choices = new string[]
        {
            INFORMATION,
            PAYMENTS,
            SCHOLARSHIPS,
            MEDICAL_DEPARTMENT,
            PROGRAMS,
            CEPSI,
            INTERNSHIPS,
            CONTACT_INFO,
            MEDICAL_DEPARTMENT_INFO,
            MEDICAL_DEPARTMENT_INSURANCE
        };

        public StudentWelfareDialog(
            UserState userState,
            ILogger<MainDialog> logger,
            IBotTelemetryClient telemetryClient,
            string dialogId = DIALOG_ID) : base(dialogId)
        {
            Logger = logger;
            TelemetryClient = telemetryClient;

            var waterfallSteps = new WaterfallStep[]
            {
                StudentWelfareStepAsync,
                StudentWelfareOptionStepAsync,
                StudentWelfareInfoStepAsync,
                DialogHelper.LastGoBackStepAsync
            };

            AddDialog(new WaterfallDialog(dialogId, waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = dialogId;

            Logger.LogInformation(dialogId);
        }

        private async Task<DialogTurnResult> StudentWelfareStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Options as IDictionary<string, string> ?? new Dictionary<string, string>();

            stepContext.Values[DialogHelper.OPTIONS_SELECTED] = optionsSelected;

            if (optionsSelected.ContainsKey(STUDENT_WELFARE_SELECTED))
            {
                var studentWelfareSelected = optionsSelected[STUDENT_WELFARE_SELECTED];

                return await stepContext.NextAsync(studentWelfareSelected, cancellationToken);
            }

            var studentWelfareAttachments = new List<Attachment>
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
                            Title = PAYMENTS,
                            Value = PAYMENTS
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = SCHOLARSHIPS,
                            Value = SCHOLARSHIPS
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = MEDICAL_DEPARTMENT,
                            Value = MEDICAL_DEPARTMENT
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = PROGRAMS,
                            Value = PROGRAMS
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = CEPSI,
                            Value = CEPSI
                        },
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = INTERNSHIPS,
                            Value = INTERNSHIPS
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

            var reply = MessageFactory.Attachment(studentWelfareAttachments);

            await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, messageReply, reply);

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> StudentWelfareOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

            if (optionsSelected.ContainsKey(STUDENT_WELFARE_OPTION_SELECTED))
            {
                var univChapOptionSelected = optionsSelected[STUDENT_WELFARE_OPTION_SELECTED];

                return await stepContext.NextAsync(univChapOptionSelected, cancellationToken);
            }

            if (!optionsSelected.ContainsKey(STUDENT_WELFARE_SELECTED))
            {
                optionsSelected.Add(STUDENT_WELFARE_SELECTED, resultSelected);
            }

            switch (resultSelected)
            {
                case INFORMATION:
                    //get from api
                    var infoLines = new string[]
                    {
                        "La **Oficina de Bienestar Universitario** como instancia orgánica de nuestra Casa de Estudios tiene " +
                        "como objeto velar por el bienestar integral de los estudiantes para su mejor realización académica.",
                        "Funciona en el área de Mezanine del Pabellón Central de la Universidad. " +
                        "Conformado por el Departamento de Bienestar Estudiantil integrado por asistentas sociales " +
                        "para la atención de los estudiantes de Pre-grado y Postgrado, por el Departamento Medico " +
                        "y el Centro de Orientación Psicopedagógica."
                    };

                    string infoText = string.Join("\n\n", infoLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(infoText), cancellationToken);

                    break;
                case PAYMENTS:
                    var paymentsLines = new string[]
                    {
                        "**COSTOS DE PENSIONES URP**",
                        "Pensiones de estudios para los alumnos ingresantes a partir del semestre 2018-II" +
                        "El pago por carrera es de 5 armadas por semestre",
                        "*Estos precios son referenciales"
                    };

                    string paymentsText = string.Join("\n\n", paymentsLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(paymentsText), cancellationToken);

                    var stringBuilder = new System.Text.StringBuilder();

                    var headers = new string[]
                    {
                        "Carrera",
                        "Escala",
                        "Beca de estudios por orfandad"
                    };

                    string headersRow = string.Join(" | ", headers);

                    stringBuilder.AppendLine($"| {headersRow} |");

                    var separators = new string[headers.Length];
                    System.Array.Fill(separators, ":---:");

                    string separatorsRow = string.Join("|", separators);

                    stringBuilder.AppendLine($"| {separatorsRow} |");

                    var data = new string[][]
                    {
                        new string[]
                        {
                            "Medicina Humana",
                            "A18",
                            "2600"
                        },
                        new string[]
                        {
                            "Arquitectura",
                            "A28",
                            "1550"
                        },
                        new string[]
                        {
                            "Ingeniería Civil",
                            "A38",
                            "1450"
                        },
                        new string[]
                        {
                            "Ingeniería Electrónica",
                            "A38",
                            "1450"
                        },
                        new string[]
                        {
                            "Ingeniería Industrial",
                            "A38",
                            "1450"
                        },
                        new string[]
                        {
                            "Ingeniería Informática",
                            "A38",
                            "1450"
                        },
                        new string[]
                        {
                            "Ingeniería Mecatrónica",
                            "A38",
                            "1450"
                        }
                    };

                    foreach (var dataRow in data)
                    {
                        string row = string.Join(" | ", dataRow);

                        stringBuilder.AppendLine($"| {row} |");
                    }

                    string table = stringBuilder.ToString();

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(table), cancellationToken);

                    break;
                case SCHOLARSHIPS:
                    var scholarshipsLines = new string[]
                    {
                        "Beca de estudios por alto rendimiento académico y situación socioeconómica",
                        "Beca automática por primeros puestos",
                        "Beca de estudios por orfandad",
                        "Beca de estudios a deportistas de alta calificación",
                        "Beca por convenio con las Fuerzas Armadas y la Policía Nacional del Perú"
                    };

                    var scholarshipsMapped = scholarshipsLines.Select(scholarship => $"- {scholarship}");

                    string scholarshipsText = "**BECAS**\n\n" +
                        string.Join("\n\n", scholarshipsMapped);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(scholarshipsText), cancellationToken);

                    break;
                case MEDICAL_DEPARTMENT:
                    var medicalDepartmentAttachments = new List<Attachment>
                    {
                        new HeroCard
                        {
                            Title = MEDICAL_DEPARTMENT,
                            Buttons = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = MEDICAL_DEPARTMENT_INFO,
                                    Value = MEDICAL_DEPARTMENT_INFO
                                },
                                new CardAction
                                {
                                    Type = ActionTypes.ImBack,
                                    Title = MEDICAL_DEPARTMENT_INSURANCE,
                                    Value = MEDICAL_DEPARTMENT_INSURANCE
                                }
                            }
                        }.ToAttachment()
                    };

                    var medicalDepartmentMessageReply = MessageFactory.Text($"Estas son las opciones en {MEDICAL_DEPARTMENT}. 🏥");

                    var medicalDepartmentReply = MessageFactory.Attachment(medicalDepartmentAttachments);

                    await DialogHelper.SendActivitiesWithDelayAsync(stepContext, cancellationToken, medicalDepartmentMessageReply, medicalDepartmentReply);

                    return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
                case PROGRAMS:
                    var programsLines = new string[]
                    {
                        "1° Programa de Apoyo Social en Calidad de Vida: Alimentación-Voluntariado, Comedor y Cafeterías.",
                        "2° Programa de Atención Médica Primaria",
                        "3° Programa de Atención Psicológica",
                        "4° Programa de Atención Psicopedagógica",
                        "5° Programa de Seguro Estudiantil",
                        "6° Programa de Asistencia Social: Becas, Movilización Social, Convenios",
                        "7°-8°-9° Programas de Deportes, Actividades Culturales, y Esparcimiento"
                    };

                    var programsMapped = programsLines.Select(program => $"- {program}");

                    string scheduleInfoText = "**PROGRAMAS DE LA OFICINA DE BIENESTAR UNIVERSITARIO**\n\n" +
                        string.Join("\n\n", programsMapped);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(scheduleInfoText), cancellationToken);

                    break;
                case CEPSI:
                    var cepsiLines = new string[]
                    {
                        "**SERVICIOS DE APOYO PROFESIONAL**",
                        "Actuar-Planificar -Verificar y Hacer Integral - " +
                        "Formando seres humanos en una Cultura de Paz Modalidad presencial y Virtual actualmente",
                        "El CEPSI es un Centro de Orientación Psicopedagógica, " +
                        "con un equipo de profesionales en la salud dentro de los cuales podemos encontrar a psicólogos clínicos, " +
                        "psicopedagogos, psiquiatra, y facilitadores terapéuticos e internos en el área de Psicología clínica y " +
                        "Psicopedagógica, los cuales brindan:"
                    };

                    string cepsiText = string.Join("\n\n", cepsiLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(cepsiText), cancellationToken);

                    var cepsiBullets = new string[]
                    {
                        "Atención diaria al trabajo planificado y coordinado con la Alta Dirección",
                        "Consultas privadas",
                        "Psicoterapia Analítica y terapia cognitivo conductual TCC",
                        "Talleres vivenciales de afrontamiento al estrés",
                        "Evaluación psicológica",
                        "Orientación Vocacional",
                        "Asesorías a Empresas públicas y privadas según convenios institucionales",
                        "Elaboración de proyectos, programas y planes de gestión y desarrollo",
                        "Investigación Descriptiva y Aplicada en Neuropsicología, Psicología Genética en laboratorio (ADN -ARNmt)",
                        "Prevención e intervención en salud",
                        "Campañas en salud y capacitación en eventos y certámenes Internacionales",
                        "Programas de promoción social que buscarán formar líderes jóvenes",
                        "Realizar actividades de salud comunitaria en trabajo de campo con municipalidades, " +
                        "proyecto de Universidad sin paredes, con el uso de medios didácticos recreativos: " +
                        "teatro, títeres como contenidos de salud y nutrición, bajo la metodología de jugar aprendiendo, y validando manual de FAO",
                        "Participando en Investigación sobre control del consumo de drogas CADI-OEA",
                        "Cumplimiento de convenios marcos y específicos interinstitucionales Nacionales e Internacionales"
                    };

                    var cepsiBulletsMapped = cepsiBullets.Select(cepsi => $"- {cepsi}");

                    string cepsiBulletsText = string.Join("\n\n", cepsiBulletsMapped);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(cepsiBulletsText), cancellationToken);

                    break;
                case INTERNSHIPS:
                    var internshipsLines = new string[]
                    {
                        "**PRÁCTICAS PRE-PROFESIONALES**",
                        "Son actividades que posibilitan la adquisición de habilidades y destrezas complementarias a " +
                        "las obtenidas en la formación profesional de los estudios universitarios. " +
                        "Y es realizada por los estudiantes que han aprobado el séptimo semestre o un equivalente a 140 créditos, " +
                        "durante el período que solicite la dependencia académico-administrativo, considerándose un plazo máximo de 5 meses y medio. " +
                        "Se establece una prioridad en su otorgamiento mediante concurso previo."
                    };

                    string internshipsText = string.Join("\n\n", internshipsLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(internshipsText), cancellationToken);

                    break;
                case CONTACT_INFO:
                    var contactInfoLines = new string[]
                    {
                        "**ATENCIÓN Y CONSULTAS**",
                        "La Oficina de Bienestar Universitario informa a los estudiantes, " +
                        "se viene atendiendo de manera virtual y telefónica con la finalidad de seguir brindando " +
                        "una mejor y personalizada atención llamando a la Centrarl URP 01-708-0000 marcando los anexos:"
                    };

                    string contactInfoText = string.Join("\n\n", contactInfoLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(contactInfoText), cancellationToken);

                    var contactInfoBullets = new string[]
                    {
                        "**Secretaria Dirección:** 0197",
                        "**Secretaria Asistentas Sociales:** 0198",
                        "**Asistente Dirección:** 0181",
                        "**Centro de Orientación Psicopedagógica:** 7256",
                        "**Practicas Pre-profesionales:** 0189",
                        "**Departamento Médico:** 8016 / 8147 / 8188 / 8422 / 8292",
                        "**Seguro Estudiantil:** 8293",
                        "**Correo:** obu@urp.edu.pe"
                    };

                    var contactInfoBulletsMapped = contactInfoBullets.Select(contInfo => $"- {contInfo}");

                    string contactInfoBulletsText = string.Join("\n\n", contactInfoBulletsMapped);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(contactInfoBulletsText), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> StudentWelfareInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var optionsSelected = stepContext.Values[DialogHelper.OPTIONS_SELECTED] as IDictionary<string, string>;

            string resultSelected = (stepContext.Result as string).Trim();

            if (DialogHelper.IsGoBack(resultSelected))
            {
                optionsSelected.Remove(STUDENT_WELFARE_SELECTED);

                return await stepContext.ReplaceDialogAsync(DIALOG_ID, optionsSelected, cancellationToken);
            }

            if (!Choices.Contains(resultSelected))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            switch (resultSelected)
            {
                case MEDICAL_DEPARTMENT_INFO:
                    var medicalDepartmentInfoLines = new string[]
                    {
                        "**DEPARTAMENTO MÉDICO**",
                        "Las instalaciones del Departamento Médico se encuentran en el Campus Universitario. " +
                        "Ofrece los siguientes servicios gratuitos:"
                    };

                    string medicalDepartmentInfoText = string.Join("\n\n", medicalDepartmentInfoLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(medicalDepartmentInfoText), cancellationToken);

                    var medicalDepartmentInfoBullets = new string[]
                    {
                        "Consultas de medicina general, ginecología y obstetricia.",
                        "Tratamiento de Fisioterapia",
                        "Servicio Paramédico: curaciones, controles vitales e inyectables.",
                        "Servicio Odontológico: Profilaxis, curaciones, restauraciones de piezas dentales con luz halógena.",
                        "Servicio de ambulancia.",
                        "La atención a la Comunidad Universitaria, en general es de " +
                        "lunes a sábado de 08.00 a 22:00 horas y domingos de 08.00 a 14.00 horas."
                    };

                    var medicalDepartmentInfoBulletsMapped = medicalDepartmentInfoBullets.Select(medDepInfo => $"- {medDepInfo}");

                    string medicalDepartmentInfoBulletsText = string.Join("\n\n", medicalDepartmentInfoLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(medicalDepartmentInfoBulletsText), cancellationToken);

                    break;
                case MEDICAL_DEPARTMENT_INSURANCE:
                    var medicalDepartmentInsuranceLines = new string[]
                    {
                        "**SEGURO ESTUDIANTIL**",
                        "La afiliación del Seguro Estudiantil es opcional mediante la Compañía Rímac Seguros. " +
                        "El alumno como condición de matrícula deberá obligatoriamente elegir y llenar uno de los tres formatos, " +
                        "los cuales serán descargados de la Página Web de nuestra Casa de Estudios, " +
                        "conjuntamente con el Plan de Seguro Médico Accidental y Enfermedades Críticas.",
                        "A Los alumnos asegurados les llegara su Póliza y Guía de Instrucciones para su atención " +
                        "a sus correos institucionales enviada por la Oficina del Bróker Allemant & Asociados ubicada en el 2do piso del Departamento Médico.",
                        "Horario de atención: lunes a viernes de 08.00 a 17.00 horas."
                    };

                    string medicalDepartmentInsuranceText = string.Join("\n\n", medicalDepartmentInsuranceLines);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(medicalDepartmentInsuranceText), cancellationToken);

                    break;
                default:
                    break;
            }

            return await DialogHelper.GoBackPromptAsync(stepContext, cancellationToken);
        }
    }
}
