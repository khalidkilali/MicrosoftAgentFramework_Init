

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

public static class StructuredOutput
{
    public static async Task RunSample()
    {
        (string endpoint, string apiKey) = Utilities.SecretManager.GetAzureOpenAIApiKeyBasedCredentials();
        AIProjectClient client = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());


        string model = "gpt-4.1-mini";


        AIAgent agent = client.AsAIAgent(
            model: model,
            instructions: "You are a movie expert",
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
            string input = "List the top 3 best movies according to IMDB";
            AgentResponse<MovieResult> response = await agent.RunAsync<MovieResult> (input);

            MovieResult movieResult = response.Result;

            foreach  (Movie movie in movieResult.Movies)
            {
                Console.WriteLine($"- Title: {movie.Title} - " +
                              $"Director: {movie.Director} - " +
                              $"Year: {movie.YearOfRelease} - " +
                              $"Score: {movie.ImdbScore}");
            }
            Console.WriteLine();
            Output.Gray("response.Text = Raw JSON");
            Console.WriteLine(response.Text);

            Console.ReadKey();
            Output.Separator();

        }
    }

    private class MovieResult
    {
        public required List<Movie> Movies { get; set; }
    }

    private class Movie
    {
        public required string Title { get; set; }
        public required string Director { get; set; }
        public required int YearOfRelease { get; set; }
        public required decimal ImdbScore { get; set; }
    }
}