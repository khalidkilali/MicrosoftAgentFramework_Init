
//using Azure.AI.Projects;
//using Azure.Identity;
//using Microsoft.Agents.AI;
//using Microsoft.Extensions.AI;
//using System.Text;

//namespace MicrosoftAgentFramework.Tools;

//public static class CreatingTools
//{
//    public static async Task RunSample()
//    {
//        (string endpoint, string apiKey) = Utilities.SecretManager.GetAzureOpenAIApiKeyBasedCredentials();
//        AIProjectClient client = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());
//        PersonTools personTools = new PersonTools();

//        string model = "gpt-4.1-mini";


//        AIAgent agent = client.
//            .AsAIAgent(
//                model: model,
//                instructions: "You are a friendly assistant. Keep your answers brief. always inclide today's dtae at the top of your answers",
//                name: "HelloAgent",
//                tools: [
//                    AIFunctionFactory.Create(personTools.GetPerson, "Get_Persons", "Get All persons you Know"),
//                    AIFunctionFactory.Create(personTools.GetPerson, "Get_Person", "Get a specific person by name"),
//                    AIFunctionFactory.Create(PersonTools.ChangeConsoleColor, "Change_console_color", "Change the color of the console")
                    
//                    ]
//                );

//        AgentSession session = await agent.CreateSessionAsync();
//        Console.OutputEncoding = Encoding.UTF8;

//        while (true) {
//            Console.Write(">");
//            string input = Console.ReadLine() ?? "";
//            AgentResponse response = await agent.RunAsync(input, session);
//            {
//                Console.WriteLine(response);
//            }
//        }
//        ;
        
//    }    
    
//    public static DateTime GetCurrentDateTime()
//    {
//        return DateTime.Now;
//    }

//    public static TimeZoneInfo GetTimeZoneInfo()
//    {
//        return TimeZoneInfo.Local;
//    }
//}

