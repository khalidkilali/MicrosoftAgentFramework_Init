
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;


IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string endpoint = config["endpoint"]!;
string apiKey = config["apikey"]!;
string model = "gpt-4.1-mini";

//// Azure IA :  
//AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential()); /// for production could use ManagedIdentityCredential
//var responsesClient = client.GetResponsesClient();
var client = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());
AIAgent agent = client
    .AsAIAgent(
        model: model,
        instructions: "You are a friendly assistant. Keep your answers brief.",
        name: "HelloAgent"
        );



string message = "What is the capital of France?";

Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine("Input:");
Console.ResetColor();
Console.WriteLine(message);

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
Console.WriteLine();
AgentResponse response = await client.RunAsync(message);
long miliseconds = stopwatch.ElapsedMilliseconds;

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Output:");
Console.ResetColor();

Console.WriteLine(response);
Console.WriteLine();

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("Usage:");
Console.ResetColor();
if (response.Usage != null)
{
    Console.WriteLine($"- Input tokens: {response.Usage.InputTokenCount}");
    Console.WriteLine($"- Cached tokens: {response.Usage.CachedInputTokenCount ?? 0} ");
    Console.WriteLine($"- Output tokens: {response.Usage.OutputTokenCount}" +
        $"({response.Usage.ReasoningTokenCount ?? 0} being reasoning Tokens)");
}

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.DarkMagenta;
Console.WriteLine("Time spent:");
Console.ResetColor();
Console.WriteLine($"{miliseconds} milli-seconds");
