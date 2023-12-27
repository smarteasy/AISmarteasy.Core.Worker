using AISmarteasy.Core.Function;
using AISmarteasy.Service.OpenAI;

namespace AISmarteasy.Core.Worker;

public abstract class LLMWorker
{

    protected LLMWorker(LLMWorkEnv workEnv)
    {
        WorkEnv = workEnv;
        if (workEnv.Vendor == LLMVendorTypeKind.OpenAI)
            AIServiceConnector = new OpenAIServiceConnector(AIServiceTypeKind.TextCompletion, workEnv.ServiceAPIKey, workEnv.Logger);
        LLMWorkEnv.PluginStore = SemanticFunctionLoader.Load(workEnv.Logger);
    }

    public LLMWorkEnv WorkEnv { get; set; }
    public AIServiceConnector? AIServiceConnector { get; set; }

    public abstract Task<ChatHistory> QueryAsync(QueryRequest request);
    public abstract Task<ChatHistory> GenerateAsync(GenerationRequest request);

    protected virtual async Task<ChatHistory> RunAsync(ChatHistory chatHistory, LLMServiceSetting requestSetting, CancellationToken cancellationToken)
    {
        return await AIServiceConnector!.TextCompletionAsync(chatHistory, requestSetting, cancellationToken);
    }
}