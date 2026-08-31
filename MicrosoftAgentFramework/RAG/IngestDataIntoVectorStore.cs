using Azure.AI.Projects;
using Azure.Identity;
using CommunityToolkit.VectorData.SqliteVec;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using MicrosoftAgentFramework.Utilities;
using OpenAI.Embeddings;

namespace MicrosoftAgentFramework.RAG;

public static class IngestDataIntoVectorStore
{
    public static async Task RunSample()
    {
        //Sample Data simulating an Internal Employee Handbook / Knowledge base
        List<KnowledgeBaseEntry> knowledgeBase =
        [
            new("What is the WI-FI Password at the Office?", "The Password is 'Guest42'"),
            new("Is Christmas Eve a full or half day off", "It is a full day off"),
            new("How do I register vacation?", "Go to the internal portal and under Vacation Registration (top right), enter your request. Your manager will be notified and will approve/reject the request"),
            new("What do I need to do if I'm sick?", "Inform you manager, and if you have any meetings remember to tell the affected colleagues/customers"),
            new("Where is the employee handbook?", "It is located [here](https://www.yourcompany.com/hr/handbook.pdf)"),
            new("Who is in charge of support?", "John Doe is in charge of support. His email is john@yourcompany.com"),
            new("I can't log in to my office account", "Take hold of Susan. She can reset your password"),
            new("When using the CRM System if get error 'index out of bounds'", "That is a known issue. Log out and back in to get it working again. The CRM team have been informed and status of ticket can be seen here: https://www.crm.com/tickets/12354"),
            new("What is the policy on buying books and online courses?", "Any training material under 20$ you can just buy.. anything higher need an approval from Richard"),
            new("Is there a bounty for find candidates for an open job position?", "Yes. 1000$ if we hire them... Have them send the application to jobs@yourcompany.com")
        ];

        //(string endpoint, string apiKey) = Utilities.SecretManager.GetAzureOpenAIApiKeyBasedCredentials();

        string endpoint = "https://devops-generator.services.ai.azure.com";

        AIProjectClient client = new(new Uri(endpoint), new DefaultAzureCredential());

        var openAIClient = client.ProjectOpenAIClient;

        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = openAIClient
             .GetEmbeddingClient("text-embedding-3-small")
             .AsIEmbeddingGenerator();


        var embeddingClient =
    openAIClient.GetEmbeddingClient("text-embedding-3-small");

        var result =
            await embeddingClient.GenerateEmbeddingAsync("Bonjour");

        Console.WriteLine(result.Value.ToFloats().Length);
        Console.WriteLine(endpoint);
        Console.WriteLine(openAIClient.GetType());
        Console.WriteLine("Deployment: text-embedding-3-small");
        await embeddingClient.GenerateEmbeddingAsync("Bonjour");


        string conenctionString = $"Data Source={Path.Combine(Path.GetTempPath(), "vector-store.db")}";

        VectorStore vectorStore = new SqliteVectorStore(conenctionString, new SqliteVectorStoreOptions
        {
            EmbeddingGenerator = embeddingGenerator
        });



        #region Alternative Connectors

        //VectorStore vectorStoreFromAzureAiSearch = new AzureAISearchVectorStore(
        //    new SearchIndexClient(new Uri("azureAiSearchEndpoint"),
        //        new AzureKeyCredential("azureAiSearchKey")
        //    ));

        //VectorStore vectorStoreFromSqlServer2025 = new SqlServerVectorStore(
        //    "connectionString");

        //VectorStore vectorStoreFromCosmosDb = new CosmosNoSqlVectorStore(
        //    "connectionString",
        //    "databaseName",
        //    new CosmosClientOptions
        //    {
        //        UseSystemTextJsonSerializerWithOptions = JsonSerializerOptions.Default,
        //    });

        #endregion

        VectorStoreCollection<Guid, KnowledgeBaseVectorRecord> vectorStoreCollection =
            vectorStore.GetCollection<Guid, KnowledgeBaseVectorRecord>("knowledgeBase");

        await vectorStoreCollection.EnsureCollectionExistsAsync();
        Console.Write("Import Data? (Y/N): ");
        ConsoleKeyInfo key = Console.ReadKey();
        if (key.Key == ConsoleKey.Y)
        {
            // Delete old table

            await vectorStoreCollection.EnsureCollectionDeletedAsync();
            Console.Clear();

            int counter = 0;
            foreach (KnowledgeBaseEntry item in knowledgeBase)
            {
                counter++;
                Console.Write($"\rEmbedding Data: {counter}/{knowledgeBase.Count}");
                try
                {
                    await vectorStoreCollection.UpsertAsync(new KnowledgeBaseVectorRecord
                    {
                        Id = Guid.NewGuid(),
                        Question = item.Question,
                        Answer = item.Answer
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            Console.WriteLine();
            Console.WriteLine("\rEmbedding complete...");
        }
        Console.WriteLine();

        Output.Title("Listing all data in the vector-store");
        await foreach (KnowledgeBaseVectorRecord existingRecord in vectorStoreCollection.GetAsync(record => record.Id != Guid.Empty, int.MaxValue))
        {
            Console.WriteLine($"Q: {existingRecord.Question} - A: {existingRecord.Answer} - Vector: {existingRecord.Vector}");
        }

    }
    public record KnowledgeBaseEntry(string Question, string Answer);

    public class KnowledgeBaseVectorRecord
    {
        [VectorStoreKey]
        public required Guid Id { get; set; }

        [VectorStoreData]
        public required string Question { get; set; }

        [VectorStoreData]
        public required string Answer { get; set; }

        [VectorStoreVector(1536)]
        public string Vector => $"Q: {Question} - A: {Answer}";
    }
}