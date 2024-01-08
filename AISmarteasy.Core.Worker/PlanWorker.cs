namespace AISmarteasy.Core.Worker;

public class PlanWorker(LLMWorkEnv workEnv) : LLMWorker(workEnv)
{
    public override Task<ChatHistory> QueryAsync(QueryRequest request)
    {
        throw new NotImplementedException();
    }

    public override Task<ChatHistory> GenerateTextAsync(TextGenerationRequest request)
    {
        throw new NotImplementedException();
    }

    public override Task GenerateAudioAsync(AudioGenerationRequest request)
    {
        throw new NotImplementedException();
    }

    public override Task<Stream> GenerateAudioStreamAsync(AudioGenerationRequest request)
    {
        throw new NotImplementedException();
    }

    public override Task<string> GenerateImageAsync(ImageGenerationRequest request)
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
}
