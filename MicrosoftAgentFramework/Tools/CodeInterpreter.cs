

using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MicrosoftAgentFramework.Utilities;
using OpenAI.Containers;
using OpenAI.Responses;
using System.ClientModel;
using System.Text;

namespace MicrosoftAgentFramework.Tools;

public static class CodeInterpreter
{
    public static async Task RunSample()
    {
        (string endpoint, string apiKey) = Utilities.SecretManager.GetAzureOpenAIApiKeyBasedCredentials();
        AIProjectClient client = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());


        string model = "gpt-4.1-mini";


        AIAgent agent = client.AsAIAgent(
            model: model,
            instructions: "You can make charts using the code interpreter tool.",
            name: "HelloAgent",
            tools:
            [
                new HostedCodeInterpreterTool()
            ]);

        AgentSession session = await agent.CreateSessionAsync();
        Console.OutputEncoding = Encoding.UTF8;

        while (true)
        {
            Console.WriteLine("> ...");
            string input = Console.ReadLine() ?? "";
            if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase)) break;
            AgentResponse resposne = await agent.RunAsync(input, session);
            {
                Console.WriteLine(resposne);
                foreach (ChatMessage message in resposne.Messages)
                {
                    foreach (AIContent content in message.Contents)
                    {
                        foreach (AIAnnotation annotation in content.Annotations ?? [])
                        {
#pragma warning disable OPENAI001
                            if (annotation is CitationAnnotation citation && citation.RawRepresentation is ContainerFileCitationMessageAnnotation containerFileCitation)
#pragma warning enable OPENAI001

                            {
                                await DownloadAndOpenFileAsync(client, containerFileCitation);
                            }

                        }

                    }
                }
            }

            Output.Separator();

        }
    }

    private static async Task DownloadAndOpenFileAsync(AIProjectClient client, ContainerFileCitationMessageAnnotation citation)
    {
        ContainerClient containerClient = client.ProjectOpenAIClient.GetContainerClient();
        ClientResult<BinaryData> fileContent = await containerClient.DownloadContainerFileAsync(citation.ContainerId, citation.FileId);
        string path = Path.Combine(Path.GetTempPath(), citation.Filename);
        await File.WriteAllBytesAsync(path, fileContent.Value.ToArray());
        await Task.Factory.StartNew(() =>
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        });

    }
}