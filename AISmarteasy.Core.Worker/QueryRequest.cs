namespace AISmarteasy.Core.Worker;

public struct QueryRequest(ChatHistory chatHistory, LLMServiceSetting serviceSetting, bool isWithStreaming = false,  
    CancellationToken cancellationToken = default)
{
    public ChatHistory ChatHistory { get; set; } = chatHistory;
    public LLMServiceSetting ServiceSetting { get; set; } = serviceSetting;
    public bool IsWithStreaming { get; set; } = isWithStreaming;
    public CancellationToken CancellationToken { get; set; } = cancellationToken;
}
