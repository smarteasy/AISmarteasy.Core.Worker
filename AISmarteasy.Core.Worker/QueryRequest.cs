using System.Diagnostics.CodeAnalysis;

namespace AISmarteasy.Core.Worker;

public struct QueryRequest(string query, LLMServiceSetting serviceSetting, CancellationToken cancellationToken = default)
{
    public string Query { get; set; } = query;
    public LLMServiceSetting ServiceSetting { get; set; } = serviceSetting;
    public CancellationToken CancellationToken { get; set; } = cancellationToken;
}
