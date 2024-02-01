namespace AISmarteasy.Core.Worker;

public abstract class AIWorker(AIWorkEnv workEnv)
{
    public AIWorkEnv WorkEnv { get; } = workEnv;
    public IAIServiceConnector? ServiceConnector { get; set; }
}