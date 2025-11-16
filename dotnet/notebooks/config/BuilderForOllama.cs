using Microsoft.SemanticKernel;
using Kernel = Microsoft.SemanticKernel.Kernel;
using System.Net.Http;

public static class BuilderForOllama
{
    public static Kernel BuildKernel(string modelPath)
    {
        // Inject your logger 
        // see Microsoft.Extensions.Logging.ILogger @ https://learn.microsoft.com/dotnet/core/extensions/logging


        //Create Kernel builder
        var builder = Kernel.CreateBuilder();
        // Configure AI service credentials used by the kernel
        var (useAzureOpenAI, model, azureEndpoint, apiKey, orgId) = Settings.LoadFromFile();
        Console.WriteLine($"Using model: {model}, {apiKey}, {orgId}");
        //if (useAzureOpenAI)
        //builder.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
        //else
        // Ollama용 HttpClient 설정
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:11434/v1/")
        };
        builder.AddOpenAIChatCompletion(modelId: model, apiKey, orgId, httpClient: httpClient);

        var kernel = builder.Build();
        return kernel;
    }
}