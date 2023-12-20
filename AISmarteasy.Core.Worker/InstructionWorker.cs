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

    public override async Task<ChatHistory> GenerateAsync(GenerationRequest request)
    {
        var function = LLMWorkEnv.PluginStore!.FindFunction(request.PluginName, request.FunctionName);
        
        Verifier.NotNull(function);
        return await function.RunAsync(AIServiceConnector!, request.ServiceSetting);
    }

    //public Task RunFunctionAsync(FunctionRunConfig config)
    //{
    //    var function = FindFunction(config.PluginName, config.FunctionName);
    //    return RunFunctionAsync(function, config.Parameters);
    //}

    //public Task RunFunctionAsync(Function function, string prompt)
    //{
    //    var config = new FunctionRunConfig();
    //    config.UpdateInput(prompt);
    //    return RunFunctionAsync(function, config.Parameters);
    //}

    //public Task RunFunctionAsync(Function function, Dictionary<string, string>? parameters = default)
    //{
    //    if (parameters != null)
    //    {
    //        foreach (var parameter in parameters)
    //        {
    //            Context.Variables[parameter.Key] = parameter.Value;
    //        }
    //    }

    //    return function.RunAsync(function.RequestSettings);
    //}
}
