using Azure.Core;

namespace AISmarteasy.Core.Worker;

public class PlanWorker : LLMWorker
{
    public PlanWorker(LLMWorkEnv workEnv) :
        base(workEnv)
    {
    }

    public override Task<ChatHistory> QueryAsync(QueryRequest request)
    {
        throw new NotImplementedException();
    }

    public override Task<ChatHistory> GenerateAsync(GenerationRequest request)
    {
        throw new NotImplementedException();
    }

    public override Task<ChatHistory> RunPipelineAsync(PipelineRunRequest request)
    {
        throw new NotImplementedException();
    }

    public override Task<string> RunSpeechToTextAsync(SpeechToTextRunRequest request)
    {
        throw new NotImplementedException();
    }

    public override Task RunTextToSpeechAsync(TextToSpeechRunRequest request)
    {
        throw new NotImplementedException();
    }

    public override Task<Stream> RunTextToSpeechStreamAsync(TextToSpeechRunRequest request)
    {
        throw new NotImplementedException();
    }
}
