using System.Reflection.Metadata.Ecma335;

namespace AISmarteasy.Core.Worker;

public static class LLMServiceSettingBuilder
{
    private const int MIDDLE_MAX_TOKENS = 1024;

    public static LLMServiceSetting Build(LLMRequestLevelKind requestLevel)
    {
        var result = new LLMServiceSetting();

        if (requestLevel == LLMRequestLevelKind.Middle)
            result.MaxTokens = MIDDLE_MAX_TOKENS;

        return result;
    }
}
