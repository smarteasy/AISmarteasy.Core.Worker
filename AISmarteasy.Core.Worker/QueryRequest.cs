using System.Diagnostics.CodeAnalysis;

namespace AISmarteasy.Core.Worker;

public struct QueryRequest(ChatHistory chatHistory, LLMServiceSetting serviceSetting, CancellationToken cancellationToken = default)
{
    public ChatHistory ChatHistory { get; set; } = chatHistory;
    public LLMServiceSetting ServiceSetting { get; set; } = serviceSetting;
    public CancellationToken CancellationToken { get; set; } = cancellationToken;
}
