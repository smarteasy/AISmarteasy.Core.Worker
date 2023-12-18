using System.Reflection.Metadata.Ecma335;

namespace AISmarteasy.Core.Worker;

public static class  LLMWorkerBuilder
{
    private const int MIDDLE_MAX_TOKENS = 1024;
    public static LLMWorker BuildInstructionWorker(LLMWorkEnv workEnv)
    {
        LLMWorker result = workEnv.WorkType == LLMWorkTypeKind.Instruction ? new InstructionWorker(workEnv) : new PlanWorker(workEnv);
        return result;
    }
}
