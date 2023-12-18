using System.Diagnostics.CodeAnalysis;

namespace AISmarteasy.Core.Worker;

public struct GenerationRequest(string pluginName, string functionName, LLMServiceSetting serviceSetting, CancellationToken cancellationToken = default)
{
    public string PluginName { get; set; } = pluginName;
    public string FunctionName { get; set; } = functionName;
    public LLMServiceSetting ServiceSetting{ get; set; } = serviceSetting;
    public CancellationToken CancellationToken { get; set; } = cancellationToken;
}
