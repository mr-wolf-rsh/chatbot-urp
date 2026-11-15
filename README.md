# URP Chatbot

Conversational chatbot built as my undergraduate thesis at Universidad Ricardo Palma (URP, Lima). It guides users through a menu of dialogs to answer questions about admissions, undergraduate and postgraduate programs, student services, and contact information.

**Thesis document (Spanish):** [Repositorio URP](https://repositorio.urp.edu.pe/server/api/core/bitstreams/35b71c7b-c119-4ca7-bd80-6a723e4ac4a5/content)

## Stack

- .NET Core 3.1 and Bot Framework v4 (C#)
- LUIS for natural-language intent recognition
- Azure Cosmos DB for conversation state
- Azure Resource Manager deployment template included

## Project layout

- `DemoChatbot/Dialogs/` — one dialog per topic (`Admission`, `UndergraduateCareer`,
  `PostgraduateCareer`, `ProgramsSpecializations`, `ProfessionalStudiesWorkExperience`,
  `StudentServices`, `ContactInfo`) plus `MainDialog` / `MainMenuDialog` for menu routing.
- `DemoChatbot/CognitiveModels/` — LUIS application model and recognizer.
- `DemoChatbot/DeploymentTemplates/` — ARM template for Azure deployment.

## Running locally

Requires .NET Core SDK 3.1.

```bash
cd DemoChatbot
dotnet run
```

Provide `MicrosoftAppId`, `MicrosoftAppPassword`, LUIS and Cosmos DB settings through
environment variables — `appsettings.json` ships placeholders only.
