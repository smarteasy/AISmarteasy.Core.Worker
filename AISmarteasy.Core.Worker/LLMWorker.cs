using AISmarteasy.Core.Function;
using AISmarteasy.Service.OpenAI;
using AISmarteasy.Skill;

namespace AISmarteasy.Core.Worker;

public abstract class LLMWorker : AIWorker
{
    protected ITextCompletionConnector LLMServiceConnector => (ITextCompletionConnector)ServiceConnector!;

    public LLMWorkEnv LLMWorkEnv  => (LLMWorkEnv)WorkEnv;

    protected LLMWorker(LLMWorkEnv workEnv) : base(workEnv)
    {
        if (workEnv.ServiceVendor == AIServiceVendorKind.OpenAI)
            ServiceConnector = new OpenAITextCompletionConnector(workEnv.AIServiceType, workEnv.AIServiceAPIKey);

        var pluginStore = SemanticFunctionLoader.Load();
        NativeFunctionLoader.Load(pluginStore);

        LLMWorkEnv.PluginStore = pluginStore;
    }



    //public abstract Task<ChatHistory> GenerateTextAsync(TextGenerationRequest request);
    //public abstract Task GenerateAudioAsync(AudioGenerationRequest request);
    //public abstract Task<Stream> GenerateAudioStreamAsync(AudioGenerationRequest request);
    //public abstract Task<string> GenerateImageAsync(ImageGenerationRequest request);


    //public abstract Task<string> RunSpeechToTextAsync(SpeechToTextRunRequest request);
    //public abstract Task<ChatHistory> RunPipelineAsync(PipelineRunRequest request);

    //public abstract Task<bool> SaveAsync(MemoryEmbeddingRequest request);
}