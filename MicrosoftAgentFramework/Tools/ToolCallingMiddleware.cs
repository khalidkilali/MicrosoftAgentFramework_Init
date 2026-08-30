
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MicrosoftAgentFramework.Utilities;
using ModelContextProtocol.Client;
using System.Security.AccessControl;
using System.Text;

namespace MicrosoftAgentFramework.Tools;

public static class ToolCallingMiddleware
{
    public static async Task RunSample()
    {
        (string endpoint, string apiKey) = Utilities.SecretManager.GetAzureOpenAIApiKeyBasedCredentials();
        AIProjectClient client = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());
        PersonTools personTools = new PersonTools();

        await using McpClient mcpClient = await McpClient.CreateAsync(new HttpClientTransport(new HttpClientTransportOptions()
        {
            Endpoint = new Uri("https://learn.microsoft.com/api/mcp"),
            TransportMode = HttpTransportMode.StreamableHttp           
        }));

        IList<McpClientTool> mcpTools = await mcpClient.ListToolsAsync();

        string model = "gpt-4.1-mini";


        AIAgent agent = client
            .AsAIAgent(
                model: model,
                instructions: "You are a Expert on c# version of Microsoft Agent Framework " +
                "(Use tools to find you knowledge)  " +
                "and assume Azure OpenAI with API Key is used",
                name: "HelloAgent",
                tools: mcpTools.Cast<AITool>().ToList()
                ).AsBuilder().Use(Middleware).Build();

        AgentSession session = await agent.CreateSessionAsync();
        Console.OutputEncoding = Encoding.UTF8;

        while (true) {
            Console.Write(">");
            string input = Console.ReadLine() ?? "";
            AgentResponse response = await agent.RunAsync(input, session);
            {
                Console.WriteLine(response);
            }
        }
        ;
        
    } 
    
    private static async ValueTask<object?> Middleware(AIAgent agent, FunctionInvocationContext context,
        Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next, CancellationToken concellationToken)

    {
        StringBuilder toolDetail = new();
        toolDetail.Append($"- Tool Call: '{context.Function.Name}'");
        if(context.Arguments.Count > 0) 
        {
            toolDetail.Append($"(args: {string.Join(", ", context.Arguments.Select(kvp => $"{kvp.Key}={kvp.Value}"))})");
        }
        Output.Yellow(toolDetail.ToString());

        return await next.Invoke(context, concellationToken);
    }
}

