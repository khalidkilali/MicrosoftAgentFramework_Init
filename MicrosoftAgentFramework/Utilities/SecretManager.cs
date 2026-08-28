using Microsoft.Extensions.Configuration;

namespace MicrosoftAgentFramework.Utilities;

public class SecretManager
{
    public static (string endpoint, string apiKey) GetAzureOpenAIApiKeyBasedCredentials()
    {
        IConfigurationRoot configuration = GetConfiguration();
        string endpoint = configuration["endpoint"] ?? ThrowMissingSecretException("endpoint");
        string apiKey = configuration["apikey"] ?? ThrowMissingSecretException("apikey");
        return (endpoint, apiKey);
    }

    public static string GetAzureOpenAIRoleBaseAccessControl()
    {
        IConfigurationRoot configuration = GetConfiguration();
        string endpoint = configuration["AzureEndpoint"] ?? ThrowMissingSecretException("AzureEndpoint");
        return endpoint;
    }

    public static string GetOpenAIApiKey()
    {
        IConfigurationRoot configuration = GetConfiguration();
        string apiKey = configuration["OpenAIApiKey"] ?? ThrowMissingSecretException("OpenAIApiKey");
        return apiKey;
    }

    private static string ThrowMissingSecretException(string variable)
    {
        throw new Exception($"Secret '{variable}' is missing; Add it to UserSecrets or Environment Variables");
    }

    private static IConfigurationRoot GetConfiguration()
    {
        return new ConfigurationBuilder().AddUserSecrets<SecretManager>().Build();
    }
}
