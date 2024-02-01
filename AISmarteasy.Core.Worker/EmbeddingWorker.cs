using AISmarteasy.Service.OpenAI;

namespace AISmarteasy.Core.Worker;

public class EmbeddingWorker: LLMWorker
{
    protected ITextEmbeddingConnector EmbeddingServiceConnector => (ITextEmbeddingConnector)ServiceConnector!;

    public EmbeddingWorker(LLMWorkEnv workEnv)
    : base(workEnv)
    {
        if (WorkEnv.ServiceVendor == AIServiceVendorKind.OpenAI)
            ServiceConnector = new OpenAIEmbeddingConnector(workEnv.AIServiceType, workEnv.AIServiceAPIKey);
    }

    public async Task<ReadOnlyMemory<float>> GenerateEmbeddingsAsync(EmbeddingRequest request, CancellationToken cancellationToken = default)
    {
        var generateEmbeddingRequest = new EmbeddingRequest(request.Data);
        return await EmbeddingServiceConnector.GenerateEmbeddingsAsync(generateEmbeddingRequest, cancellationToken).ConfigureAwait(false);
    }
}
