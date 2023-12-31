﻿namespace AISmarteasy.Core.Worker;

public class InstructionWorker(LLMWorkEnv workEnv) : LLMWorker(workEnv)
{
    public override async Task<ChatHistory> QueryAsync(QueryRequest request)
    {
        var queryFunction = LLMWorkEnv.PluginStore!.FindFunction("QuerySkill", "Default");
        Verifier.NotNull(queryFunction);

        var question = request.ChatHistory.LastContent;
        LLMWorkEnv.WorkerContext.Variables.UpdateInput(question);
        var chatHistory = await queryFunction.RunAsync(AIServiceConnector!, request.ServiceSetting);

        if (chatHistory.LastContent.Contains("google.search", StringComparison.OrdinalIgnoreCase))
        {
            var searchFunction = LLMWorkEnv.PluginStore.FindFunction("GoogleSkill", "Search");
            await searchFunction!.RunAsync(AIServiceConnector!, request.ServiceSetting);

            LLMWorkEnv.WorkerContext.Variables.UpdateContext(LLMWorkEnv.WorkerContext.Result);
        }

        var function = LLMWorkEnv.PluginStore.FindFunction("QuerySkill", "WithSearch");
        Verifier.NotNull(function);

        if (request.IsWithStreaming)
        {
            var answer = string.Empty;
            await foreach (var answerStreaming in function.RunStreamingAsync(AIServiceConnector!, request.ServiceSetting))
            {
                answer += answerStreaming.Content;
            }

            chatHistory.AddAssistantMessage(answer);
        }
        else
        {
            chatHistory = await function.RunAsync(AIServiceConnector!, request.ServiceSetting);
        }

        return chatHistory;
    }

    public override async Task<ChatHistory> GenerateTextAsync(TextGenerationRequest request)
    {
        var function = LLMWorkEnv.PluginStore!.FindFunction(request.PluginName, request.FunctionName);
        Verifier.NotNull(function);

        var userMessage = request.ChatHistory.LastContent;
        LLMWorkEnv.WorkerContext.Variables.UpdateInput(userMessage);

        return await function.RunAsync(AIServiceConnector!, request.ServiceSetting);
    }

    public override Task GenerateAudioAsync(AudioGenerationRequest request)
    {
        return AIServiceConnector!.GenerateAudioAsync(request);
    }

    public override Task<Stream> GenerateAudioStreamAsync(AudioGenerationRequest request)
    {
        return AIServiceConnector!.GenerateAudioStreamAsync(request);
    }

    public override Task<string> GenerateImageAsync(ImageGenerationRequest request)
    {
        return AIServiceConnector!.GenerateImageAsync(request);
    }


    public override async Task<ChatHistory> RunPipelineAsync(PipelineRunRequest request)
    {
        var chatHistory = new ChatHistory();

        foreach (var pluginFunctionName in request.PluginFunctionNames)
        {
            var function = LLMWorkEnv.PluginStore!.FindFunction(pluginFunctionName.PluginName, pluginFunctionName.FunctionName);

            Verifier.NotNull(function);
            chatHistory = await function.RunAsync(AIServiceConnector!, request.ServiceSetting);
            LLMWorkEnv.WorkerContext.Variables.UpdateInput(chatHistory.PipelineLastContent);
        }

        request.Parameters.Clear();

        return chatHistory;
    }

    public override async Task<string> RunSpeechToTextAsync(SpeechToTextRunRequest request)
    {
        return await AIServiceConnector!.RunSpeechToTextAsync(request);
    }
}
