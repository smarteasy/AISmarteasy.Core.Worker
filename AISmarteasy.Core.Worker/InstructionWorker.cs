using AISmarteasy.Service.OpenAI;

namespace AISmarteasy.Core.Worker;

public class InstructionWorker : LLMWorker
{
    public InstructionWorker(LLMWorkEnv workEnv)
        : base(workEnv)
    {
    }

    public override async Task<ChatHistory> QueryAsync(QueryRequest request)
    {
        return await RunAsync(request.ChatHistory, request.ServiceSetting, request.CancellationToken);
    }

    public override Task<ChatHistory> GenerateAsync(GenerationRequest request)
    {
        throw new NotImplementedException();
    }
}
