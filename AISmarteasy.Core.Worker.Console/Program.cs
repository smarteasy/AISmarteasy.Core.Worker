using Microsoft.Extensions.Logging.Abstractions;

namespace AISmarteasy.Core.Worker.Console;

internal class Program
{
    private static readonly string OpenaiAPIKey = WorkerEnv.OPENAI_API_KEY;
    //    private static readonly string PineconeEnvironment = Env.PineconeEnvironment;
    //    private static readonly string PineconeAPIKey = Env.PineconeAPIKey;

    public static async Task Main()
    {
        //await Run_InstructionWorker_Query();
        //await Run_InstructionWorker_Generate_Summarize();
        //await Run_InstructionWorker_Query_Chat();

        await PrintSemanticFunctionCategory();

        System.Console.ReadLine();
    }

    private static Task PrintSemanticFunctionCategory()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, OpenaiAPIKey, LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

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

    private static async Task Run_InstructionWorker_Query_Chat()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, OpenaiAPIKey, LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var chatHistory = new ChatHistory();

        while (true)
        {
            chatHistory = await Query_Chat(worker, chatHistory);
        }
    }

    private static async Task<ChatHistory> Query_Chat(LLMWorker worker, ChatHistory chatHistory)
    {
        System.Console.Write("User > ");
        var userMessage = System.Console.ReadLine();
        chatHistory.AddUserMessage(userMessage ?? string.Empty);

        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new QueryRequest(chatHistory, serviceSetting);

        chatHistory = await worker.QueryAsync(request);
        System.Console.WriteLine("Assistant > " + chatHistory.LastContent);

        return chatHistory;
    }

    private static async void Run_InstructionWorker_Query()
    {
        var logger = NullLogger.Instance;
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, OpenaiAPIKey, LLMWorkTypeKind.Instruction, logger);
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
        var workEnv = new LLMWorkEnv(LLMVendorTypeKind.OpenAI, OpenaiAPIKey, LLMWorkTypeKind.Instruction, logger);
        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

        var chatHistory = new ChatHistory();
        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var request = new GenerationRequest("SummarizeSkill", "Summarize", chatHistory, serviceSetting);

        var input = GetSummarizeText();
        LLMWorkEnv.WorkerContext.Variables.Update(input);

        chatHistory = await worker.GenerateAsync(request);

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

    ////    private static async void Run_Example04_01_CombineLLMPromptsAndNativeCode()
    ////    {
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        var config = new PipelineRunConfig();
    ////        config.AddPluginFunctionName("GoogleSkill", "Search");
    ////        config.AddPluginFunctionName("SummarizeSkill", "Summarize");
    ////        config.UpdateInput("What's the tallest building in South America");

    ////        var result2 = await kernel!.RunPipelineAsync(config);
    ////        Console.WriteLine(result2.Text);
    ////    }


    ////    private static async void Run_Example01_NativeFunctions()
    ////    {
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        var config = new FunctionRunConfig("TextSkill", "Uppercase");
    ////        config.UpdateInput("ciao");
    ////        await kernel!.RunFunctionAsync(config);

    ////        Console.WriteLine(KernelProvider.Kernel!.ContextVariablesInput);
    ////    }

    ////    private static async void Run_Example02_Pipeline()
    ////    {
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        var config = new PipelineRunConfig();

    ////        config.AddPluginFunctionName("TextSkill", "TrimStart");
    ////        config.AddPluginFunctionName("TextSkill", "TrimEnd");
    ////        config.AddPluginFunctionName("TextSkill", "Uppercase");

    ////        config.UpdateInput("    i n f i n i t e     s p a c e     ");

    ////        var answer = await kernel!.RunPipelineAsync(config);
    ////        Console.WriteLine(answer.Text);
    ////    }

    ////    private static async void Run_Example03_Variables()
    ////    {
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        var variables = new ContextVariables("Today is: ");
    ////        variables.Set("day", DateTimeOffset.Now.ToString("dddd", CultureInfo.CurrentCulture));

    ////        kernel!.Context = new SKContext(variables);

    ////        var config = new PipelineRunConfig();
    ////        config.AddPluginFunctionName("TextSkill", "AppendDay");
    ////        config.AddPluginFunctionName("TextSkill", "Uppercase");

    ////        var answer = await kernel.RunPipelineAsync(config);
    ////        Console.WriteLine(answer.Text);
    ////    }

    ////    private static async void Run_Example04_01_CombineLLMPromptsAndNativeCode()
    ////    {
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        var config = new PipelineRunConfig();
    ////        config.AddPluginFunctionName("GoogleSkill", "Search");
    ////        config.AddPluginFunctionName("SummarizeSkill", "Summarize");
    ////        config.UpdateInput("What's the tallest building in South America");

    ////        var result2 = await kernel!.RunPipelineAsync(config);
    ////        Console.WriteLine(result2.Text);
    ////    }

    ////    private static async void Run_Example04_02_CombineLLMPromptsAndNativeCode()
    ////    {
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        var config = new PipelineRunConfig();
    ////        config.AddPluginFunctionName("GoogleSkill", "Search");
    ////        config.AddPluginFunctionName("SummarizeSkill", "Notegen");
    ////        config.UpdateInput("What's the tallest building in South America");

    ////        Console.WriteLine("======== Notegen ========");
    ////        var result1 = await kernel!.RunPipelineAsync(config);
    ////        Console.WriteLine(result1.Text);
    ////    }

    ////    private static async void Run_Example05_InlineFunctionDefinition()
    ////    {
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;
    ////        string promptTemplate = @"
    ////        Generate a creative reason or excuse for the given event.
    ////        Be creative and be funny. Let your imagination run wild.

    ////        Event: I am running late.
    ////        Excuse: I was being held ransom by giraffe gangsters.

    ////        Event: I haven't been to the gym for a year
    ////        Excuse: I've been too busy training my pet dragon.

    ////        Event: {{$input}}
    ////        ";

    ////        string configText = @"
    ////        {
    ////            ""schema"": 1,
    ////            ""type"": ""completion"",
    ////            ""description"": ""Generate a creative reason or excuse for the given event."",
    ////            ""completion"": {
    ////                ""max_tokens"": 1024,
    ////                ""temperature"": 0.4,
    ////                ""top_p"": 1
    ////            }
    ////        }
    ////        ";
    ////        var config = PromptTemplateConfig.FromJson(configText);
    ////        var template = new PromptTemplate(promptTemplate, config);
    ////        var functionConfig = new SemanticFunctionConfig("EventSkill", "GenerateReasonOrExcuse", config, template);
    ////        var inlineFunction = kernel!.RegisterSemanticFunction(functionConfig);

    ////        await kernel.RunFunctionAsync(inlineFunction, "I missed the F1 final race");
    ////        Console.WriteLine(KernelProvider.Kernel!.ContextVariablesInput);

    ////        await kernel.RunFunctionAsync(inlineFunction, "sorry I forgot your birthday");
    ////        Console.WriteLine(KernelProvider.Kernel.ContextVariablesInput);
    ////    }

    ////    private static async void Run_Example06_TemplateLanguage()
    ////    {
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        string promptTemplate = @"
    ////Today is: {{TimeSkill.Date}}
    ////Current time is: {{TimeSkill.Time}}

    ////Answer to the following questions using JSON syntax, including the data used.
    ////Is it morning, afternoon, evening, or night (morning/afternoon/evening/night)?
    ////Is it weekend time (weekend/not weekend)?
    ////";

    ////        string configText = @"
    ////        {
    ////            ""schema"": 1,
    ////            ""type"": ""completion"",
    ////            ""description"": ""Generate day and time information."",
    ////            ""completion"": {
    ////                ""max_tokens"": 1024,
    ////                ""temperature"": 0.4,
    ////                ""top_p"": 1
    ////            }
    ////        }
    ////        ";
    ////        var config = PromptTemplateConfig.FromJson(configText);
    ////        var template = new PromptTemplate(promptTemplate, config);
    ////        var functionConfig =
    ////            new SemanticFunctionConfig("TimeSkill", "GenerateDayTimeInformation", config, template);
    ////        var function = kernel?.RegisterSemanticFunction(functionConfig);

    ////        await kernel!.RunFunctionAsync(function!, string.Empty);
    ////        Console.WriteLine(KernelProvider.Kernel?.ContextVariablesInput);
    ////    }

    ////    private static Task RunExample10_DescribeAllPluginsAndFunctions()
    ////    {
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        foreach (var plugin in kernel!.Plugins.Values)
    ////        {
    ////            foreach (var function in plugin.Functions)
    ////            {
    ////                PrintFunction(function.View);
    ////            }
    ////        }

    ////        return Task.CompletedTask;
    ////    }

    ////    private static void PrintFunction(FunctionView func)
    ////    {
    ////        Console.WriteLine($"   {func.Name}: {func.Description}");

    ////        if (func.Parameters.Count > 0)
    ////        {
    ////            Console.WriteLine("      Params:");
    ////            foreach (var p in func.Parameters)
    ////            {
    ////                Console.WriteLine($"      - {p.Name}: {p.Description}");
    ////                Console.WriteLine($"        default: '{p.DefaultValue}'");
    ////            }
    ////        }

    ////        Console.WriteLine();
    ////    }

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



    ////    private static async void Run_Example13_03_ConversationSummaryPlugin()
    ////    {
    ////        Console.WriteLine("======== SamplePlugins - Conversation Summary Plugin - Topics ========");

    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;
    ////        var config = new FunctionRunConfig("ConversationSummarySkill", "GenerateTopics");
    ////        config.UpdateInput(ProviderChatTranscript.EXAMPLE13);
    ////        await kernel!.RunFunctionAsync(config);

    ////        Console.WriteLine("Generated Topics::");
    ////        Console.WriteLine(kernel.Result);
    ////    }

    ////    private static async void Run_Example14_01_SemanticMemory()
    ////    {
    ////        Console.WriteLine("Adding some GitHub file URLs and their descriptions to the semantic memory.");

    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, OpenaiAPIKey, ImageGenerationTypeKind.None,
    ////                MemoryTypeKind.PineCone, PineconeAPIKey, PineconeEnvironment));
    ////        var kernel = KernelProvider.Kernel;
    ////        var githubFiles = SampleData();
    ////        await kernel!.SaveMemoryAsync(githubFiles);
    ////    }

    ////    private static Dictionary<string, string> SampleData()
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




    ////    private static async void Run_Example18_01_DallE()
    ////    {
    ////        Console.WriteLine("======== OpenAI Dall-E 2 Image Generation ========");

    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, OpenaiAPIKey, ImageGenerationTypeKind.DallE));
    ////        var kernel = KernelProvider.Kernel;
    ////        var imageDescription = "A cute baby sea otter";
    ////        var image = await kernel!.RunImageGenerationAsync(imageDescription, 256, 256);

    ////        Console.WriteLine(imageDescription);
    ////        Console.WriteLine("Image URL: " + image);
    ////    }

    ////    private static async void Run_Example18_02_DallE()
    ////    {
    ////        Console.WriteLine("======== Chat with images ========");

    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey, ImageGenerationTypeKind.DallE));
    ////        var kernel = KernelProvider.Kernel;

    ////        var systemMessage = @"You're chatting with a user. Instead of replying directly to the user
    //// provide the description of an image that expresses what you want to say.
    //// The user won't see your message, they will see only the image. The system
    //// generates an image using your description, so it's important you describe the image with details.";

    ////        var chatHistory = await kernel!.RunChatCompletionAsync(systemMessage);

    ////        var msg = "Hi, I'm from Tokyo, where are you from?";
    ////        chatHistory.AddUserMessage(msg);
    ////        Console.WriteLine("User: " + msg);

    ////        chatHistory = await kernel.RunChatCompletionAsync(chatHistory);
    ////        await MessageOutputAsync(chatHistory);

    ////        var botMessageBase = chatHistory.Messages.Last();
    ////        var image = await kernel.RunImageGenerationAsync(botMessageBase.Content, 1024, 1024);
    ////        Console.WriteLine("Bot: " + image);
    ////        Console.WriteLine("Img description: " + botMessageBase.Content);

    ////        msg = "Oh, wow. Not sure where that is, could you provide more details?";
    ////        chatHistory.AddUserMessage(msg);
    ////        Console.WriteLine("User: " + msg);

    ////        chatHistory = await kernel.RunChatCompletionAsync(chatHistory);
    ////        await MessageOutputAsync(chatHistory);
    ////    }

    ////    private static async void Run_Example32_StreamingCompletion()
    ////    {
    ////        Console.WriteLine("======== Azure OpenAI - Text Completion - Raw Streaming ========");

    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;
    ////        var prompt = "Write one paragraph why AI is awesome";

    ////        Console.WriteLine("Prompt: " + prompt);
    ////        await foreach (var answer in kernel!.RunTextStreamingCompletionAsync(prompt))
    ////        {
    ////            await foreach (var stream in answer.GetCompletionStreamingAsync())
    ////            {
    ////                Console.Write(stream);
    ////            }
    ////        }

    ////        Console.WriteLine();
    ////    }

    ////    private static async void Run_Example33_StreamingChat()
    ////    {
    ////        Console.WriteLine("======== Open AI - ChatGPT Streaming ========");

    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;
    ////        Console.WriteLine("Chat content:");
    ////        Console.WriteLine("------------------------");

    ////        var systemMessage = "You are a librarian, expert about books";
    ////        var chatHistory = await kernel!.RunChatCompletionAsync(systemMessage);
    ////        await MessageOutputAsync(chatHistory);

    ////        chatHistory.AddUserMessage("Hi, I'm looking for book suggestions");
    ////        await MessageOutputAsync(chatHistory);

    ////        Console.Write($"{AuthorRole.Assistant}: ");

    ////        await foreach (var answer in kernel.RunChatStreamingAsync(chatHistory))
    ////        {
    ////            await foreach (var stream in answer.GetStreamingChatMessageAsync())
    ////            {
    ////                Console.Write(stream.Content);
    ////            }
    ////        }
    ////    }

    ////    private static async void Run_LanguageCalculatorPlugin()
    ////    {
    ////        KernelBuilder.Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, OpenaiAPIKey));
    ////        var kernel = KernelProvider.Kernel;

    ////        var config = new PipelineRunConfig();

    ////        config.AddPluginFunctionName("LanguageMathProblemSkill", "TranslateMathProblem");
    ////        config.AddPluginFunctionName("LanguageCalculatorSkill", "Calculate");

    ////        config.UpdateInput("what is the square root of 625?");

    ////        var answer = await kernel!.RunPipelineAsync(config);
    ////        Console.WriteLine(answer.Text);
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

    //    //public static async Task RunExample7()
    //    //{
    //    //    var kernel = new KernelBuilder()
    //    //        .Build(new AIServiceConfig(AIServiceTypeKind.ChatCompletion, API_KEY));

    //    //    var loader = new NativePluginLoader();
    //    //    loader.Load();

    //    //    var questions = "What's the exchange rate EUR:USD?";
    //    //    Console.WriteLine(questions);

    //    //    var function = kernel.FindFunction("RAGSkill", "Search");
    //    //    var parameters = new Dictionary<string, string> { ["input"] = questions };
    //    //    var answer = await kernel.RunFunction(function, parameters);

    //    //    if (answer.Text.Contains("google.search", StringComparison.OrdinalIgnoreCase))
    //    //    {
    //    //        var searchFunction = kernel.FindFunction("GoogleSkill", "Search");
    //    //        var searchParameters = new Dictionary<string, string> { { "input", questions } };

    //    //        var searchAnswer = await kernel.RunFunction(searchFunction, searchParameters);
    //    //        string searchResult = searchAnswer.Text;

    //    //        Console.WriteLine("---- Fetching information from Google");
    //    //        Console.WriteLine(searchResult);

    //    //        parameters = new Dictionary<string, string>
    //    //        {
    //    //            ["input"] = questions,
    //    //            ["externalInformation"] = searchResult
    //    //        };

    //    //        answer = await kernel.RunFunction(function, parameters);
    //    //        Console.WriteLine($"Answer: {answer.Text}");
    //    //    }
    //    //    else
    //    //    {
    //    //        Console.WriteLine("AI had all the information, no need to query Google.");
    //    //        Console.WriteLine($"Answer: {answer.Text}");
    //    //    }
    //    //}

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



    //    //        public static async Task RunSemanticFunction()
    //    //        {
    //    //            var kernel = new KernelBuilder()
    //    //                .Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, API_KEY));

    //    //            var function = kernel.FindFunction("Fun", "Joke");
    //    //            var parameters = new Dictionary<string, string> { { "input", "time travel to dinosaur age" } };

    //    //            var answer = await kernel.RunFunction(function, parameters);
    //    //            Console.WriteLine(answer.Text);
    //    //        }

    //    //        public static async Task RunNativeFunction()
    //    //        {
    //    //            var kernel = new KernelBuilder()
    //    //                .Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, API_KEY));

    //    //            var loader = new NativePluginLoader();
    //    //            loader.Load();

    //    //            var function = kernel.FindFunction("MathSkill", "Sqrt");
    //    //            var parameters = new Dictionary<string, string> { { "input", "12" } };

    //    //            var answer = await kernel.RunFunction(function, parameters);
    //    //            Console.WriteLine(answer.Text);

    //    //            parameters = new Dictionary<string, string>
    //    //            {
    //    //                { "input", "12.34" },
    //    //                { "number", "56.78" }
    //    //            };

    //    //            function = kernel.FindFunction("MathSkill", "Multiply");
    //    //            answer = await kernel.RunFunction(function, parameters);
    //    //            Console.WriteLine(answer.Text);
    //    //        }

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



    //    //        public static async Task RunOrchestratorFunction()
    //    //        {
    //    //            var kernel = new KernelBuilder()
    //    //                .Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, API_KEY)); 

    //    //            var loader = new NativePluginLoader();
    //    //            loader.Load();

    //    //            var function = kernel.FindFunction("OrchestratorSkill", "RouteRequest");
    //    //            var parameters = new Dictionary<string, string> { { "input", "What is the square root of 634?" } };

    //    //            var answer = await kernel.RunFunction(function, parameters);
    //    //            Console.WriteLine(answer.Text);
    //    //        }

    //    //        public static async Task RunPipeline()
    //    //        {
    //    //            var kernel = new KernelBuilder()
    //    //                    .Build(new AIServiceConfig(AIServiceTypeKind.TextCompletion, API_KEY));


    //    //            var jokeFunction = kernel.FindFunction("TempSkill", "Joke");
    //    //            var poemFunction = kernel.FindFunction("TempSkill", "Poem");
    //    //            var menuFunction = kernel.FindFunction("TempSkill", "Menu");

    //    //            kernel.Context.Variables["input"] = "Charlie Brown";


    //    //            var answer = await kernel.RunPipeline(jokeFunction, poemFunction, menuFunction);
    //    //            Console.WriteLine(answer.Text);
}




