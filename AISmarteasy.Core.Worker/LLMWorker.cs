using AISmarteasy.Service;
using AISmarteasy.Service.OpenAI;

namespace AISmarteasy.Core.Worker;

public abstract class LLMWorker
{
    protected LLMWorker(LLMWorkEnv workEnv)
    {
        WorkEnv = workEnv;
        if (workEnv.Vendor == LLMVendorTypeKind.OpenAI)
            AIServiceConnector = new OpenAIServiceConnector(AIServiceTypeKind.TextCompletion, workEnv.ServiceAPIKey);
    }

    public LLMWorkEnv WorkEnv { get; set; }

    public AIServiceConnector? AIServiceConnector { get; set; }

    public abstract Task<string> QueryAsync(QueryRequest request);
    public abstract Task<string> GenerateAsync(GenerationRequest request);

    protected virtual async Task<string> RunAsync(string prompt, LLMServiceSetting requestSetting, CancellationToken cancellationToken)
    {
        return await AIServiceConnector!.TextCompletionAsync(prompt, requestSetting, cancellationToken);
    }
}