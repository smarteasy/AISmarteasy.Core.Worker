using System.Text;

namespace AISmarteasy.Core.Worker;

public class InstructionWorker(LLMWorkEnv workEnv, MemoryWorker memoryWorker) : LLMWorker(workEnv)
{
    public MemoryWorker MemoryWorker { get; set; } = memoryWorker;

    public async Task<ChatHistory> QueryAsync(QueryRequest request)
    {
        if (request.IsWithImage)
        { 
            Verifier.NotNull(request.ChatHistory);

            var chatHistoryWithImage = await LLMServiceConnector.RunAsync(request.ChatHistory, request.ServiceSetting);
            Verifier.NotNull(chatHistoryWithImage);
            Verifier.NotNull(chatHistoryWithImage.LastContent);
            Verifier.NotNull(chatHistoryWithImage.LastContent.Content);

            LLMWorkEnv.WorkerContext.Variables.UpdateContext(chatHistoryWithImage.LastContent.Content);
        }

        var queryContent = request.QueryContent;
        Verifier.NotNull(queryContent);

        LLMWorkEnv.WorkerContext.Variables.UpdateInput(queryContent);

        //var memoryQuery = MemoryQuery.Create().WithQuery(queryContent);
        //Verifier.NotNull(MemoryWorker);
        //Verifier.NotNull(memoryQuery);

        //var memoryAnswer = new StringBuilder();
        //var memoryRequest = MemoryQueryRequest.Create(request.MemoryCollectionName, request.MemoryCollectionNamespace, memoryQuery, request.MemoryTopK, request.MemoryRelevanceScore);
        //await foreach (var answer in MemoryWorker.QueryAsync(memoryRequest))
        //{
        //    memoryAnswer.Append(answer.Metadata.Text);
        //}
        //LLMWorkEnv.WorkerContext.Variables.UpdateContext(memoryAnswer.ToString());

        var queryFunction = LLMWorkEnv.PluginStore!.FindFunction("QuerySkill", "Default");
        Verifier.NotNull(queryFunction);
        var chatHistory = await queryFunction.RunAsync(LLMServiceConnector, request.ServiceSetting);

        if (chatHistory.LastContent.Content!.Contains("google.search", StringComparison.OrdinalIgnoreCase))
        {
            chatHistory = await QueryWithGoogleSearch(request, chatHistory);
        }

        return chatHistory;
    }

    private async Task<ChatHistory> QueryWithGoogleSearch(QueryRequest request, ChatHistory chatHistory)
    {
        var searchFunction = LLMWorkEnv.PluginStore!.FindFunction("GoogleSkill", "Search");
        var searchResult = await searchFunction!.RunAsync(LLMServiceConnector, request.ServiceSetting);
        LLMWorkEnv.WorkerContext.Variables.UpdateContext(searchResult.LastContent.Content!);

        var function = LLMWorkEnv.PluginStore.FindFunction("QuerySkill", "WithSearchEngine");
        Verifier.NotNull(function);

        if (request.IsWithStreaming)
        {
            var answer = string.Empty;
            await foreach (var answerStreaming in function.RunStreamingAsync(LLMServiceConnector, request.ServiceSetting))
            {
                answer += answerStreaming.Content;
            }

            chatHistory.AddAssistantMessage(answer);
        }
        else
        {
            chatHistory = await function.RunAsync(LLMServiceConnector, request.ServiceSetting);
        }

        return chatHistory;
    }

    //public override Task GenerateAudioAsync(AudioGenerationRequest request)
    //{
    //    return ServiceConnector!.GenerateAudioAsync(request);
    //}

    //public override Task<Stream> GenerateAudioStreamAsync(AudioGenerationRequest request)
    //{
    //    return ServiceConnector!.GenerateAudioStreamAsync(request);
    //}

    //public override Task<string> GenerateImageAsync(ImageGenerationRequest request)
    //{
    //    return ServiceConnector!.GenerateImageAsync(request);
    //}


    //public override async Task<ChatHistory> RunPipelineAsync(PipelineRunRequest request)
    //{
    //    var chatHistory = new ChatHistory();

    //    foreach (var pluginFunctionName in request.PluginFunctionNames)
    //    {
    //        var function = LLMWorkEnv.PluginStore!.FindFunction(pluginFunctionName.PluginName, pluginFunctionName.FunctionName);

    //        Verifier.NotNull(function);
    //        chatHistory = await function.RunAsync(ServiceConnector!, request.ServiceSetting);
    //        LLMWorkEnv.WorkerContext.Variables.UpdateInput(chatHistory.PipelineLastContent);
    //    }

    //    request.Parameters.Clear();

    //    return chatHistory;
    //}

    //public override async Task<string> RunSpeechToTextAsync(SpeechToTextRunRequest request)
    //{
    //    return await ServiceConnector!.RunSpeechToTextAsync(request);
    //}
}
