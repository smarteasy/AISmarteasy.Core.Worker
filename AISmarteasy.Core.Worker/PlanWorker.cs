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
}
