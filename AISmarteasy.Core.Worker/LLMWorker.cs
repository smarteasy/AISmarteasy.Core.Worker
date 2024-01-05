using AISmarteasy.Core.Function;
using AISmarteasy.Service.OpenAI;
using AISmarteasy.Skill;

namespace AISmarteasy.Core.Worker;

public abstract class LLMWorker
{

    protected LLMWorker(LLMWorkEnv workEnv)
    {
        WorkEnv = workEnv;
        if (workEnv.Vendor == LLMVendorTypeKind.OpenAI)
            AIServiceConnector = new OpenAIServiceConnector(workEnv.AIServiceType, workEnv.ServiceAPIKey, workEnv.Logger);
        var pluginStore = SemanticFunctionLoader.Load(workEnv.Logger);
        NativeFunctionLoader.Load(pluginStore, workEnv.Logger);

        LLMWorkEnv.PluginStore = pluginStore;
    }

    public LLMWorkEnv WorkEnv { get; set; }
    public AIServiceConnector? AIServiceConnector { get; set; }

    public abstract Task<ChatHistory> QueryAsync(QueryRequest request);
    public abstract Task<ChatHistory> GenerateAsync(GenerationRequest request);

    public abstract Task<ChatHistory> RunPipelineAsync(PipelineRunRequest request);

    public abstract Task<string> RunSpeechToTextAsync(SpeechToTextRunRequest request);

    public abstract Task RunTextToSpeechAsync(TextToSpeechRunRequest request);
    public abstract Task<Stream> RunTextToSpeechStreamAsync(TextToSpeechRunRequest request);
}