using AISmarteasy.Service.OpenAI;

namespace AISmarteasy.Core.Worker;

public class InstructionWorker : LLMWorker
{
    public InstructionWorker(LLMWorkEnv workEnv)
        : base(workEnv)
    {
    }

    public override async Task<string> QueryAsync(QueryRequest request)
    {
        return await RunAsync(request.Query, request.ServiceSetting, request.CancellationToken);
    }

    public override async Task<string> GenerateAsync(GenerationRequest request)
    {
        var prompt = "";
        return await RunAsync(prompt, request.ServiceSetting, request.CancellationToken);
    }
}
