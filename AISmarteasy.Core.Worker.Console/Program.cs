using System.Globalization;
using AISmarteasy.Service;
using AISmarteasy.Service.OpenAI;
using Microsoft.Extensions.Logging.Abstractions;
using NAudio.Wave;

namespace AISmarteasy.Core.Worker.Console;

internal class Program
{
    private static readonly string OpenaiAPIKey = WorkerEnv.OPENAI_API_KEY;

    public static async Task Main()
    {
        //await Run_InstructionWorker_Query();
        //await Run_InstructionWorker_Generate_Summarize();
        //await Run_InstructionWorker_Query_Chat();
        //await PrintSemanticFunctionCategory();
        //await Run_InstructionWorker_Query_Chat_RAG();

        //await Run_InstructionWorker_Query_Streaming();
        //await Run_InstructionWorker_Query_Chat_RAG_Streaming();
        //await Run_InstructionWorker_NativeFunction_StaticTextSkill();
        //await Run_InstructionWorker_NativeFunction_TextSkill();
        //await Run_InstructionWorker_NativeFunction_TextSkill_Pipeline();
        await Run_InstructionWorker_NativeFunction_MathSkill();

        //await Run_InstructionWorker_AudioTranscription();
        //await Run_InstructionWorker_AudioTranscription_Ko();
        //await Run_InstructionWorker_AudioTranscription_Ko_Correct();

        //await Run_InstructionWorker_TextToSpeech_SaveFile();

        //var filepath = "./speech.mp3";
        //PlayTextToSpeechFile(filepath);

        //await Run_InstructionWorker_TextToSpeech_Stream();

        //var imageDescription = "A cute baby sea otter";
        //await Run_InstructionWorker_GenerateImage(imageDescription);

        //await Run_InstructionWorker_GenerateImage_UsingChat();

        //await SpeechToTextWithMicrophone();

        System.Console.ReadLine();
    }

    private static async Task Run_InstructionWorker_NativeFunction_MathSkill()
    {
        System.Console.WriteLine("======== NativeFunction_MathSkill ========");

        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);


        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new PipelineRunRequest(serviceSetting);
        request.AddPluginFunctionName("MathSkill", "TranslateToNCalc");
        request.AddPluginFunctionName("MathSkill", "Evaluate");

        System.Console.WriteLine("Problem: ");
        var problem = "250, 3240, 288의 최소공배수를 구해.";
        //var problem = "What is the square root of 625.";
        System.Console.WriteLine(problem);
        LLMWorkEnv.WorkerContext.Variables.UpdateInput(problem);
        var chatHistory = await worker.RunPipelineAsync(request);

        System.Console.WriteLine("Answer: ");
        System.Console.WriteLine(chatHistory.PipelineLastContent);
    }

    private static async Task SpeechToTextWithMicrophone()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var recorder = new SpeechRecorder("./temp.mp3", worker.AIServiceConnector!);
        recorder.StartRecording();

        System.Console.ReadKey();

        await recorder.StopRecording();
    }

    public static async Task<string> Run_InstructionWorker_GenerateImage(string imageDescription)
    {
        System.Console.WriteLine("======== Generating Image ========");

        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.ImageGeneration, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new ImageGenerationRequest(imageDescription, 1024, 1024);

        var image = await worker.GenerateImageAsync(request);

        System.Console.WriteLine(imageDescription);
        System.Console.WriteLine("Image URL: " + image);

        System.Console.WriteLine("이미지 생성 완료.");

        return image;
    }

    public static async Task Run_InstructionWorker_GenerateImage_UsingChat()
    {
        System.Console.WriteLine("======== Generating Image UsingChat ========");

        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var chatHistory = new ChatHistory();

        var userMessage =
            "An ink sketch style illustration of a small hedgehog holding a piece of watermelon with its tiny paws, taking little bites with its eyes closed in delight.";


        chatHistory.AddUserMessage(userMessage);
        System.Console.WriteLine("User: " + userMessage);

        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new TextGenerationRequest("ImageSkill", "GenerateDescription", chatHistory, serviceSetting);

        System.Console.WriteLine("Bot: ");
        chatHistory = await worker.GenerateTextAsync(request);
        await Run_InstructionWorker_GenerateImage(chatHistory.LastContent);
    }

    public static async Task Run_InstructionWorker_TextToSpeech_Stream()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextToSpeechSpeed, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var text = "안녕하세요. 반갑습니다. 저는 뉴테크프라임 대표 컨설턴트 김현남입니다. " + GetTtsText();

        LLMWorkEnv.WorkerContext.Variables.UpdateInput(text);
        System.Console.WriteLine(text);
        var request = new AudioGenerationRequest(OpenAIConfigProvider.ProvideTtsVoice(TtsVoiceKind.Alloy));

        await using var stream = await worker.GenerateAudioStreamAsync(request);
        PlayTextToSpeechStream(stream);

        System.Console.WriteLine("TTS 완료.");
    }

    private static void PlayTextToSpeechFile(string filepath)
    {
        using var mf = new MediaFoundationReader(filepath);
        using var wo = new WaveOutEvent();
        wo.Init(mf);
        wo.Play();
        while (wo.PlaybackState == PlaybackState.Playing)
        {
            Thread.Sleep(1000);
        }
    }

    private static void PlayTextToSpeechStream(Stream stream)
    {
        using var mf = new StreamMediaFoundationReader(stream);
        using var wo = new WaveOutEvent();
        wo.Init(mf);
        wo.Play();
        while (wo.PlaybackState == PlaybackState.Playing)
        {
            Thread.Sleep(1000);
        }
    }

    public static async Task Run_InstructionWorker_TextToSpeech_SaveFile()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextToSpeechQuality, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var filepath = "./speech.mp3";
        var text = "안녕하세요. 반갑습니다. 저는 뉴테크프라임 대표 컨설턴트 김현남입니다. " + GetTtsText();
        LLMWorkEnv.WorkerContext.Variables.UpdateInput(text);
        System.Console.WriteLine(text);
        var request = new AudioGenerationRequest(filepath, OpenAIConfigProvider.ProvideTtsVoice(TtsVoiceKind.Onyx));
        await worker.GenerateAudioAsync(request);

        System.Console.WriteLine("TTS 완료.");
    }

    public static async Task Run_InstructionWorker_AudioTranscription_Ko()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var filepath = "./kmk.mp3";

        var trimmedAudioFiles = AudioTranscriptionHelper.TrimSilence(filepath);
        var request = new SpeechToTextRunRequest(SpeechSourceTypeKind.Files, "ko", new byte[] { }, trimmedAudioFiles);

        var audioTranscription = await worker.RunSpeechToTextAsync(request);
        System.Console.WriteLine(audioTranscription);
    }

    public static async Task Run_InstructionWorker_AudioTranscription_Ko_Correct()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var filepath = "./kmk.mp3";
        var trimmedAudioFiles = AudioTranscriptionHelper.TrimSilence(filepath);
        var transcriptionRunRequest =
            new SpeechToTextRunRequest(SpeechSourceTypeKind.Files, "ko", new byte[] { }, trimmedAudioFiles);

        var audioTranscription = await worker.RunSpeechToTextAsync(transcriptionRunRequest);
        System.Console.WriteLine(audioTranscription);


        workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new PipelineRunRequest(serviceSetting);
        request.AddPluginFunctionName("AudioSkill", "CorrectKoreanTranscription");
        LLMWorkEnv.WorkerContext.Variables["CorrectlySpelledWords"] = "생성AI";
        LLMWorkEnv.WorkerContext.Variables.UpdateInput(audioTranscription);
        var chatHistory = await worker.RunPipelineAsync(request);
        System.Console.WriteLine("Correct AudioTranscription:");
        System.Console.WriteLine(chatHistory.PipelineLastContent);
    }

    public static async Task Run_InstructionWorker_AudioTranscription_Correct()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var earningsCallUrl = "https://cdn.openai.com/API/examples/data/EarningsCall.wav";
        var earningsCallFilepath = "./EarningsCall.wav";

        await AudioTranscriptionHelper.DownloadAudioFile(earningsCallUrl, earningsCallFilepath);
        var trimmedAudioFiles = AudioTranscriptionHelper.TrimSilence(earningsCallFilepath);

        var transcriptionRunRequest =
            new SpeechToTextRunRequest(SpeechSourceTypeKind.Files, "en", new byte[] { }, trimmedAudioFiles);
        var audioTranscription = await worker.RunSpeechToTextAsync(transcriptionRunRequest);
        System.Console.WriteLine("AudioTranscription:" + audioTranscription);

        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new PipelineRunRequest(serviceSetting);
        request.AddPluginFunctionName("AudioSkill", "CorrectTranscription");
        LLMWorkEnv.WorkerContext.Variables["CompanyName"] = "NewTechPrime";
        LLMWorkEnv.WorkerContext.Variables["CorrectlySpelledWords"] = "UML";
        LLMWorkEnv.WorkerContext.Variables.UpdateInput(audioTranscription);
        var chatHistory = await worker.RunPipelineAsync(request);
        System.Console.WriteLine("Correct AudioTranscription:" + chatHistory.PipelineLastContent);
    }

    public static async Task Run_InstructionWorker_AudioTranscription()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var earningsCallUrl = "https://cdn.openai.com/API/examples/data/EarningsCall.wav";
        var earningsCallFilepath = "./EarningsCall.wav";

        await AudioTranscriptionHelper.DownloadAudioFile(earningsCallUrl, earningsCallFilepath);
        var trimmedAudioFiles = AudioTranscriptionHelper.TrimSilence(earningsCallFilepath);

        var request = new SpeechToTextRunRequest(SpeechSourceTypeKind.Files, "en", new byte[] { }, trimmedAudioFiles);
        var audioTranscription = await worker.RunSpeechToTextAsync(request);
        System.Console.WriteLine(audioTranscription);
    }

    private static async Task Run_InstructionWorker_NativeFunction_StaticTextSkill()
    {
        System.Console.WriteLine("======== NativeFunction_StaticTextSkill ========");

        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);


        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new PipelineRunRequest(serviceSetting);
        request.AddPluginFunctionName("StaticTextSkill", "AppendDay");
        LLMWorkEnv.WorkerContext.Variables.UpdateInput("Today is: ");
        LLMWorkEnv.WorkerContext.Variables.Set("day", DateTimeOffset.Now.ToString("dddd", CultureInfo.CurrentCulture));

        System.Console.WriteLine("Answer: ");

        var chatHistory = await worker.RunPipelineAsync(request);
        System.Console.WriteLine(chatHistory.PipelineLastContent);
    }

    public static async Task Run_InstructionWorker_NativeFunction_TextSkill()
    {
        System.Console.WriteLine("======== NativeFunction_TextSkill ========");

        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new PipelineRunRequest(serviceSetting);
        request.AddPluginFunctionName("TextSkill", "Uppercase");
        LLMWorkEnv.WorkerContext.Variables.UpdateInput("ciao!");

        System.Console.WriteLine("Answer: ");

        var chatHistory = await worker.RunPipelineAsync(request);
        System.Console.Write(chatHistory.PipelineLastContent);
    }

    public static async Task Run_InstructionWorker_NativeFunction_TextSkill_Pipeline()
    {
        System.Console.WriteLine("======== NativeFunction_TextSkill ========");

        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new PipelineRunRequest(serviceSetting);
        request.AddPluginFunctionName("TextSkill", "TrimStart");
        request.AddPluginFunctionName("TextSkill", "TrimEnd");
        request.AddPluginFunctionName("TextSkill", "Uppercase");

        LLMWorkEnv.WorkerContext.Variables.UpdateInput("    infinite space     ");
        System.Console.WriteLine("Answer: ");

        var chatHistory = await worker.RunPipelineAsync(request);
        System.Console.Write(chatHistory.PipelineLastContent);
    }

    public static async Task Run_InstructionWorker_Query_Streaming()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var chatHistory = new ChatHistory();
        var userMessage = "ChatGPT?";

        System.Console.WriteLine("Query: " + userMessage);

        chatHistory.AddUserMessage(userMessage);
        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new QueryRequest(chatHistory, serviceSetting, true);

        System.Console.WriteLine("Answer: ");

        chatHistory = await worker.QueryAsync(request);
        System.Console.Write(chatHistory.LastContent);
    }

    public static async Task Run_InstructionWorker_Query_Chat_RAG_Streaming()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var chatHistory = new ChatHistory();
        var userMessage = "What's Ferrari stock price?";

        System.Console.WriteLine("Query: " + userMessage);

        chatHistory.AddUserMessage(userMessage);
        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new QueryRequest(chatHistory, serviceSetting, true);

        chatHistory = await worker.QueryAsync(request);

        System.Console.WriteLine("=========");
        System.Console.WriteLine("Retrieval: " + LLMWorkEnv.WorkerContext.Variables.Context);
        System.Console.WriteLine("=========");
        System.Console.WriteLine("Answer: " + chatHistory.LastContent);
    }

    public static async Task Run_InstructionWorker_Query_Chat_RAG()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var chatHistory = new ChatHistory();
        var userMessage = "What's Ferrari stock price?";

        System.Console.WriteLine("Query: " + userMessage);

        chatHistory.AddUserMessage(userMessage);
        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new QueryRequest(chatHistory, serviceSetting);

        chatHistory = await worker.QueryAsync(request);

        System.Console.WriteLine("=========");
        System.Console.WriteLine("Retrieval: " + LLMWorkEnv.WorkerContext.Variables.Context);
        System.Console.WriteLine("=========");
        System.Console.WriteLine("Answer: " + chatHistory.LastContent);
    }

    private static Task PrintSemanticFunctionCategory()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var categories = LLMWorkEnv.PluginStore!.SemanticFunctionCategories;
        foreach (var category in categories)
        {
            System.Console.WriteLine(category.Name);

            foreach (var subCategory in category.SubCategories)
            {
                System.Console.WriteLine("- " + subCategory.Name + ":" + subCategory.Content);
            }
        }

        return Task.CompletedTask;
    }

    private static async Task<ChatHistory> Run_InstructionWorker_Query_Chat()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var chatHistory = new ChatHistory();

        while (true)
        {
            chatHistory = await Query_Chat(worker, chatHistory);
        }

        return chatHistory;
    }

    private static async Task<ChatHistory> Query_Chat(LLMWorker worker, ChatHistory chatHistory)
    {
        System.Console.Write("User > ");
        var userMessage = System.Console.ReadLine();
        Verifier.NotNullOrWhitespace(userMessage);

        chatHistory.AddUserMessage(userMessage);

        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new QueryRequest(chatHistory, serviceSetting);

        chatHistory = await worker.QueryAsync(request);
        System.Console.WriteLine("Assistant > " + chatHistory.LastContent);

        return chatHistory;
    }

    private static async void Run_InstructionWorker_Query()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var systemMessage = "You are a librarian, expert about books";

        var chatHistory = new ChatHistory();
        chatHistory.AddAssistantMessage(systemMessage);

        var userMessage = "Hi, I'm looking for book suggestions";
        chatHistory.AddUserMessage(userMessage);
        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new QueryRequest(chatHistory, serviceSetting);

        chatHistory = await worker.QueryAsync(request);

        System.Console.WriteLine(chatHistory.LastContent);
    }

    private static async void Run_InstructionWorker_Generate_Summarize()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
            LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var chatHistory = new ChatHistory();
        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new TextGenerationRequest("SummarizeSkill", "Summarize", chatHistory, serviceSetting);

        var input = GetSummarizeText();
        LLMWorkEnv.WorkerContext.Variables.UpdateInput(input);

        chatHistory = await worker.GenerateTextAsync(request);

        System.Console.WriteLine(chatHistory.LastContent);
    }

    private static string GetSummarizeText()
    {
        string result =
            @"
John: Hello, how are you?
Jane: I'm fine, thanks. How are you?
John: I'm doing well, writing some example code.
Jane: That's great! I'm writing some example code too.
John: What are you writing?
Jane: I'm writing a chatbot.
John: That's cool. I'm writing a chatbot too.
Jane: What language are you writing it in?
John: I'm writing it in C#.
Jane: I'm writing it in Python.
John: That's cool. I need to learn Python.
Jane: I need to learn C#.
John: Can I try out your chatbot?
Jane: Sure, here's the link.
John: Thanks!
Jane: You're welcome.
Jane: Look at this poem my chatbot wrote:
Jane: Roses are red
Jane: Violets are blue
Jane: I'm writing a chatbot
Jane: What about you?
John: That's cool. Let me see if mine will write a poem, too.
John: Here's a poem my chatbot wrote:
John: The singularity of the universe is a mystery.
John: The universe is a mystery.
John: The universe is a mystery.
John: The universe is a mystery.
John: Looks like I need to improve mine, oh well.
Jane: You might want to try using a different model.
Jane: I'm using the GPT-3 model.
John: I'm using the GPT-2 model. That makes sense.
John: Here is a new poem after updating the model.
John: The universe is a mystery.
John: The universe is a mystery.
John: The universe is a mystery.
John: Yikes, it's really stuck isn't it. Would you help me debug my code?
Jane: Sure, what's the problem?
John: I'm not sure. I think it's a bug in the code.
Jane: I'll take a look.
Jane: I think I found the problem.
Jane: It looks like you're not passing the right parameters to the model.
John: Thanks for the help!
Jane: I'm now writing a bot to summarize conversations. I want to make sure it works when the conversation is long.
John: So you need to keep talking with me to generate a long conversation?
Jane: Yes, that's right.
John: Ok, I'll keep talking. What should we talk about?
Jane: I don't know, what do you want to talk about?
John: I don't know, it's nice how CoPilot is doing most of the talking for us. But it definitely gets stuck sometimes.
Jane: I agree, it's nice that CoPilot is doing most of the talking for us.
Jane: But it definitely gets stuck sometimes.
John: Do you know how long it needs to be?
Jane: I think the max length is 1024 tokens. Which is approximately 1024*4= 4096 characters.
John: That's a lot of characters.
Jane: Yes, it is.
John: I'm not sure how much longer I can keep talking.
Jane: I think we're almost there. Let me check.
Jane: I have some bad news, we're only half way there.
John: Oh no, I'm not sure I can keep going. I'm getting tired.
Jane: I'm getting tired too.
John: Maybe there is a large piece of text we can use to generate a long conversation.
Jane: That's a good idea. Let me see if I can find one. Maybe Lorem Ipsum?
John: Yeah, that's a good idea.
Jane: I found a Lorem Ipsum generator.
Jane: Here's a 4096 character Lorem Ipsum text:
Jane: Lorem ipsum dolor sit amet, con
Jane: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc sit amet aliquam
Jane: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc sit amet aliquam
Jane: Darn, it's just repeating stuf now.
John: I think we're done.
Jane: We're not though! We need like 1500 more characters.
John: Oh Cananda, our home and native land.
Jane: True patriot love in all thy sons command.
John: With glowing hearts we see thee rise.
Jane: The True North strong and free.
John: From far and wide, O Canada, we stand on guard for thee.
Jane: God keep our land glorious and free.
John: O Canada, we stand on guard for thee.
Jane: O Canada, we stand on guard for thee.
Jane: That was fun, thank you. Let me check now.
Jane: I think we need about 600 more characters.
John: Oh say can you see?
Jane: By the dawn's early light.
John: What so proudly we hailed.
Jane: At the twilight's last gleaming.
John: Whose broad stripes and bright stars.
Jane: Through the perilous fight.
John: O'er the ramparts we watched.
Jane: Were so gallantly streaming.
John: And the rockets' red glare.
Jane: The bombs bursting in air.
John: Gave proof through the night.
Jane: That our flag was still there.
John: Oh say does that star-spangled banner yet wave.
Jane: O'er the land of the free.
John: And the home of the brave.
Jane: Are you a Seattle Kraken Fan?
John: Yes, I am. I love going to the games.
Jane: I'm a Seattle Kraken Fan too. Who is your favorite player?
John: I like watching all the players, but I think my favorite is Matty Beniers.
Jane: Yeah, he's a great player. I like watching him too. I also like watching Jaden Schwartz.
John: Adam Larsson is another good one. The big cat!
Jane: WE MADE IT! It's long enough. Thank you!
John: You're welcome. I'm glad we could help. Goodbye!
Jane: Goodbye!
";
        return result;
    }

    private static string GetTtsText()
    {
        string result =
            @"
안녕하십니까, 시민 여러분. 이 무대는 박물관입니다. 정확히 말씀드리자면 구석기 시대 유물 전시실이지요. 박물관의 전속 실내 장식가는 보시는 바 이렇게 꾸며 놓았습니다. 
기둥은 구름으로 만들었고, 벽은 공기, 문은 바람으로, 천정은 햇빛, 모두 그 옛날의 재료를 썼다고 합니다. 
여기, 같은 재료로 만든 의자가 무대 한 가운데 놓여 있고, 출입구는 좌우 양측에, 진열장은 벽면을 차지했습니다. 
돌로 만든 그릇, 도끼, 장신구 등 옛 석기 시대 물건들이 이 진열장 속에 가지런히 놓여져 있지요. 
        ";
        return result;
    }


    //    //        public static async Task TimeSkillNow()
    //    //        {
    //    //            var kernel = new KernelBuilder()
    //    //                .Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, API_KEY));

    //    //            var loader = new NativePluginLoader();
    //    //            loader.Load();

    //    //            var function = kernel.FindFunction("TimeSkill", "Now");

    //    //            var parameters = new Dictionary<string, string> { };
    //    //            var answer = await kernel.RunFunction(function, parameters);

    //    //            Console.WriteLine(answer.Text);
    //    //        }

    ////    private static async void Run_Example14_01_SemanticMemory()
    ////    {
    ////        Console.WriteLine("Adding some GitHub file URLs and their descriptions to the semantic memory.");

    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, OpenaiAPIKey, ImageGenerationTypeKind.None,
    ////                MemoryTypeKind.PineCone, PineconeAPIKey, PineconeEnvironment));
    ////        var kernel = KernelProvider.Kernel;
    ////        var githubFiles = SampleData();
    ////        await kernel!.SaveMemoryAsync(githubFiles);
    ////    }
    ///
    ///     ////    private static Dictionary<string, string> SampleData()
    ////    {
    ////        return new Dictionary<string, string>
    ////        {
    ////            ["https://github.com/microsoft/semantic-kernel/blob/main/README.md"]
    ////                = "README: Installation, getting started, and how to contribute",
    ////            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks/02-running-prompts-from-file.ipynb"]
    ////                = "Jupyter notebook describing how to pass prompts from a file to a semantic plugin or function",
    ////            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks//00-getting-started.ipynb"]
    ////                = "Jupyter notebook describing how to get started with the Semantic Kernel",
    ////            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/plugins/ChatPlugin/ChatGPT"]
    ////                = "Sample demonstrating how to create a chat plugin interfacing with ChatGPT",
    ////            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/SemanticKernel/Memory/VolatileMemoryStore.cs"]
    ////                = "C# class that defines a volatile embedding store",
    ////            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/dotnet/KernelHttpServer/README.md"]
    ////                = "README: How to set up a Semantic Kernel Service API using Azure Function Runtime v4",
    ////            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/chat-summary-webapp-react/README.md"]
    ////                = "README: README associated with a sample chat summary react-based webapp",
    ////        };
    ////    }

    ////    private static async void Run_Example14_02_SemanticMemory()
    ////    {
    ////        var query = "Can I build a chat with SK?";

    ////        Console.WriteLine("\nQuery: " + query + "\n");

    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
    ////            ImageGenerationTypeKind.None,
    ////            MemoryTypeKind.PineCone, PineconeAPIKey, PineconeEnvironment));
    ////        var kernel = KernelProvider.Kernel;
    ////        var memoryResults = await kernel!.SearchMemoryAsync(query);


    ////        var i = 0;
    ////        await foreach (MemoryQueryResult memoryResult in memoryResults)
    ////        {
    ////            Console.WriteLine($"Result {++i}:");
    ////            Console.WriteLine("  URL:     : " + memoryResult.Metadata.Id);
    ////            Console.WriteLine("  Title    : " + memoryResult.Metadata.Text);
    ////            Console.WriteLine("  Relevance: " + memoryResult.Relevance);
    ////            Console.WriteLine();
    ////        }
    ////    }


    //    //public static async Task RunPdf()
    //    //{
    //    //    var kernel = new KernelBuilder()
    //    //        .Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, API_KEY,
    //    //            MemoryTypeKind.PineCone, PINECONE_API_KEY, PINECONE_ENVIRONMENT));

    //    //    var directory = Directory.GetCurrentDirectory();
    //    //    var paragraphs = await kernel.SaveEmbeddingsFromDirectoryPdfFiles(directory);

    //    //    foreach (var paragraph in paragraphs)
    //    //    {
    //    //        Console.WriteLine(paragraph);
    //    //    }
    //    //}



    ////    private static async void Run_Example12_SequentialPlanner()
    ////    {
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        var goal = "Write a poem about John Doe, then translate it into Korean.";

    ////        var plan = await kernel!.RunPlanAsync(goal, WorkerTypeKind.Sequential);
    ////        Console.WriteLine("Plan:\n");
    ////        Console.WriteLine(plan.Content);
    ////        Console.WriteLine("Answer:\n");
    ////        Console.WriteLine(plan.Answer);
    ////    }


    ////    private static async void Run_Example28_ActionPlanner()
    ////    {
    ////        Console.WriteLine("======== Action Planner ========");
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        var goal = "Write a joke about Cleopatra in the style of Hulk Hogan.";

    ////        var plan = await kernel!.RunPlanAsync(goal, WorkerTypeKind.Action);
    ////        Console.WriteLine("Plan:\n");
    ////        Console.WriteLine(plan!.Content);
    ////        Console.WriteLine("Answer:\n");
    ////        Console.WriteLine(plan.Answer);
    ////    }

    ////    private static async void Run_StepwisePlanner()
    ////    {
    ////        Console.WriteLine("======== Stepwise Planner ========");
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        var goal = "What is the weather in Seattle?";

    ////        kernel!.AddIncludedFunctionView(new[] { "GoogleSkill" });
    ////        var plan = await kernel!.RunPlanAsync(goal, WorkerTypeKind.Stepwise);
    ////        Console.WriteLine("Plan:\n");
    ////        Console.WriteLine(plan!.Content);
    ////        Console.WriteLine("Answer:\n");
    ////        Console.WriteLine(plan.Answer);
    ////    }



    //    //        public static async Task RunPineconeIndexRelated()
    //    //        {
    //    //            var pinecone = new PineconeClient(PINECONE_ENVIRONMENT, PINECONE_API_KEY);

    //    //            var creatingIndexName = "smarteasy";

    //    //            await foreach(var index in pinecone.ListIndexesAsync())
    //    //            {
    //    //                var existedIndexName = index;
    //    //                if (existedIndexName == creatingIndexName)
    //    //                {
    //    //                    creatingIndexName = "test";
    //    //                }
    //    //                var describeIndex = await pinecone.DescribeIndexAsync(existedIndexName!);
    //    //                Console.WriteLine(describeIndex?.Configuration);

    //    //                await pinecone.DeleteIndexAsync(existedIndexName!);
    //    //            }

    //    //            var indexDefinition = new IndexDefinition(creatingIndexName);
    //    //            await pinecone.CreateIndexAsync(indexDefinition);
    //    //            Console.WriteLine($"{creatingIndexName} is created." );
    //    //        }

    //    //        public static async Task GeneratePineconeEmbeddings()
    //    //        {
    //    //            var kernel = new KernelBuilder()
    //    //                .Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, API_KEY, 
    //    //                    MemoryTypeKind.PineCone, PINECONE_API_KEY, PINECONE_ENVIRONMENT));

    //    //            string[] data = { "A", "B" };

    //    //            var embeddings = await kernel.AIService.GenerateEmbeddingsAsync(data);

    //    //            Console.WriteLine("GenerateEmbeddings");

    //    //            var vectors = new List<PineconeDocument>();
    //    //            foreach (var embedding in embeddings)
    //    //            {
    //    //                var vector = new PineconeDocument(embedding);
    //    //                vectors.Add(vector);
    //    //            }

    //    //            var indexName = "smarteasy";
    //    //            var pinecone = new PineconeClient(PINECONE_ENVIRONMENT, PINECONE_API_KEY);

    //    //            await pinecone.UpsertAsync(indexName, vectors);

    //    //            var describeIndexStats = await pinecone.DescribeIndexStatsAsync(indexName);
    //    //            Console.WriteLine(JsonSerializer.Serialize(describeIndexStats));
    //    //        }
}




