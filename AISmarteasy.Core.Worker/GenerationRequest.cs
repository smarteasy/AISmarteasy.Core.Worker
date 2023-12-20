namespace AISmarteasy.Core.Worker;

public struct GenerationRequest(string pluginName, string functionName, ChatHistory chatHistory,
    LLMServiceSetting serviceSetting, CancellationToken cancellationToken = default)
{
    public string PluginName { get; set; } = pluginName;
    public string FunctionName { get; set; } = functionName;
    public ChatHistory ChatHistory { get; set; } = chatHistory;
    public LLMServiceSetting ServiceSetting{ get; set; } = serviceSetting;
    public CancellationToken CancellationToken { get; set; } = cancellationToken;
}
