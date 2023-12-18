using AISmarteasy.Service.OpenAI;

namespace AISmarteasy.Core.Worker;

public class PlanWorker : LLMWorker
{
    public PlanWorker(LLMWorkEnv workEnv) :
        base(workEnv)
    {
    }

    public override Task<string> QueryAsync(QueryRequest request)
    {
        throw new NotImplementedException();
    }

    public override Task<string> GenerateAsync(GenerationRequest request)
    {
        throw new NotImplementedException();
    }
}
