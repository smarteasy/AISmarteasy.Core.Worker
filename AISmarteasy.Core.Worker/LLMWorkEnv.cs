namespace AISmarteasy.Core.Worker;

public class LLMWorkEnv(LLMVendorTypeKind vendor, string serviceAPIKey, LLMWorkTypeKind workType)
{
    public LLMVendorTypeKind Vendor { get; set; } = vendor;
    public string ServiceAPIKey { get; set; } = serviceAPIKey;
    public LLMWorkTypeKind WorkType { get; set; } = workType;
}
