using System.Runtime.CompilerServices;
using AISmarteasy.Service;
using AISmarteasy.Service.OpenAI;
using AISmarteasy.Service.Pinecone;

namespace AISmarteasy.Core.Worker;

public class MemoryWorker(LLMWorkEnv workEnv, IMemory memory) 
    : LLMWorker(workEnv)
{
    IMemory? Memory { get; } = memory;


    public IMemoryStore? MemoryStore { get; set; }
    
    public ITextEmbeddingConnector? TextEmbeddingConnector { get; set; }

    private readonly IFileSystemConnector _fileSystemConnector = new LocalFileSystemConnector();

    public async Task<bool> SaveAsync(MemoryUpsertRequest request, CancellationToken cancellationToken = default)
    {
        Verifier.NotNull(TextEmbeddingConnector);
        Verifier.NotNull(MemoryStore);

        var collectionName = request.CollectionName;
        if (!await MemoryStore.DoesCollectionExistAsync(collectionName, cancellationToken).ConfigureAwait(false))
        {
            await MemoryStore.CreateCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
        }

        foreach (var data in request.Datas)
        {
            var embeddingRequest = new EmbeddingRequest(data);
            var embedding = await TextEmbeddingConnector.GenerateEmbeddingsAsync(embeddingRequest, cancellationToken)
                .ConfigureAwait(false);
            MemoryRecord record = MemoryRecord.LocalRecord(data.Id, data.Text, description: data.Description, embedding, data.AdditionalMetadata);
            await MemoryStore.UpsertAsync(collectionName, record, cancellationToken).ConfigureAwait(false);
        }

        return true;
    }

    public bool ImportAsync(DocumentRequest request, CancellationToken cancellationToken = default)
    {
        //var queryContent = request.ChatHistory.LastContent;
        //LLMWorkEnv.WorkerContext.Variables.UpdateInput(queryContent);


        var stream = Memory!.ImportAsync(request, cancellationToken: cancellationToken);
        
        //_documentConnector.ReadText(stream);

        return true;
    }


    //public Task<string> ImportDocumentAsync()
    //{
    //    var document = new Document(documentId, tags: tags).AddFile(filePath);
    //    DocumentUploadRequest uploadRequest = new(document, index, steps);
    //    return this.ImportDocumentAsync(uploadRequest, cancellationToken);
    //}


    //public async Task<bool> SaveDocumentAsync(DocumentRequest request, CancellationToken cancellationToken = default)
    //{
    //    var queryContent = request.ChatHistory.LastContent;
    //    LLMWorkEnv.WorkerContext.Variables.UpdateInput(queryContent);


    //    var stream = await _fileSystemConnector.GetFileContentStreamAsync(request.DocumentPath, cancellationToken);
    //    _documentConnector.ReadText(stream);

    //    return true;
    //}

    public async Task<MemoryQueryResult?> GetAsync(MemoryQueryRequest request, CancellationToken cancellationToken = default)
    {
        Verifier.NotNull(MemoryStore);

        MemoryRecord? record = await MemoryStore.GetAsync(request.CollectionName, request.Id, cancellationToken).ConfigureAwait(false);

        if (record == null) { return null; }

        return MemoryQueryResult.FromMemoryRecord(record, 1);
    }

    public async IAsyncEnumerable<MemoryQueryResult> QueryAsync(MemoryQueryRequest request, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Verifier.NotNull(request.MemoryQuery);

        Verifier.NotNull(TextEmbeddingConnector);
        Verifier.NotNull(MemoryStore);

        var embeddingRequest = new EmbeddingRequest(new MemorySourceData("query", request.Query));
        var embedding = await TextEmbeddingConnector.GenerateEmbeddingsAsync(embeddingRequest, cancellationToken).ConfigureAwait(false);

        IAsyncEnumerable<(MemoryRecord, double)> results = MemoryStore.GetNearestMatchesAsync(request.CollectionName, request.CollectionNamespace, embedding, request.TopK, 
            request.MinRelevanceScore, request.IsIncludeMetadata, cancellationToken);

        await foreach ((MemoryRecord, double) result in results)
        {
            var memoryQuery = MemoryQuery.Create().WithId(result.Item1.Key);
            MemoryRecord? record = await MemoryStore.GetAsync(request.CollectionName, memoryQuery.Id!, cancellationToken).ConfigureAwait(false);
            yield return MemoryQueryResult.FromMemoryRecord(record!, result.Item2);
        }
    }
}



//public async Task<string> SaveReferenceAsync(string collection, string text, string externalId, string externalSourceName,
//    string? description = null, string? additionalMetadata = null, CancellationToken cancellationToken = default)
//{
//    var embedding = await this._embeddingGenerator.GenerateEmbeddingAsync(text, cancellationToken).ConfigureAwait(false);
//    var data = MemoryRecord.ReferenceRecord(externalId: externalId, sourceName: externalSourceName, description: description,
//        additionalMetadata: additionalMetadata, embedding: embedding);

//    if (!(await this._storage.DoesCollectionExistAsync(collection, cancellationToken).ConfigureAwait(false)))
//    {
//        await this._storage.CreateCollectionAsync(collection, cancellationToken).ConfigureAwait(false);
//    }

//    return await this._storage.UpsertAsync(collection, data, cancellationToken).ConfigureAwait(false);
//}



//public async Task RemoveAsync(string collection, string key, CancellationToken cancellationToken = default)
//{
//    await this._storage.RemoveAsync(collection, key, cancellationToken).ConfigureAwait(false);
//}



//public async Task<IList<string>> GetCollectionsAsync(CancellationToken cancellationToken = default)
//{
//    return await storage.GetCollectionsAsync(cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
//}

