using AISmarteasy.Service;
using Microsoft.Extensions.Logging.Abstractions;

namespace AISmarteasy.Core.Worker.Console;

internal class Program
{
    private static readonly string OpenaiAPIKey = WorkerEnv.OPENAI_API_KEY;
    //private static readonly string MemoryEnvironment = WorkerEnv.MEMORY_ENVIRONMENT;
    //private static readonly string MemoryAPIKey = WorkerEnv.MEMORY_API_KEY;
    private static readonly string MemoryCollectionName = "smarteasy";

    public static async Task Main()
    {
        //await Run_InstructionWorker_Query();
        //await Run_EmbeddingWorker_Embedding();
        //await Run_VectorDatabaseWorker_Pinecone_Save();
        //await Run_VectorDatabaseWorker_Pinecone_Retrieve_WithId();
        //await Run_VectorDatabaseWorker_Pinecone_Retrieve_WithQuery();
        //await Run_VectorDatabaseWorker_Volatile_Save_Retrieve();
        //await Run_VectorDatabaseWorker_Volatile_Retrieve_WithQuery();


        //await Run_ImportDocument();

        await Run_InstructionWorker_Query_WithImage();

        //await Run_InstructionWorker_Query_Ex();
        //await Run_InstructionWorker_Generate_Summarize();
        //await Run_InstructionWorker_Query_Chat();
        //await PrintSemanticFunctionCategory();
        //await Run_InstructionWorker_Query_Chat_RAG();

        //await Run_InstructionWorker_Query_PineconeEmbedding();


        //await Run_InstructionWorker_Query_Streaming();
        //await Run_InstructionWorker_Query_Chat_RAG_Streaming();

        //await Run_InstructionWorker_NativeFunction_StaticTextSkill();
        //await Run_InstructionWorker_NativeFunction_TextSkill();
        //await Run_InstructionWorker_NativeFunction_TextSkill_Pipeline();
        //await Run_InstructionWorker_NativeFunction_MathSkill();

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

    private static async Task Run_InstructionWorker_Query_WithImage()
    {
        var logger = NullLogger.Instance;
        
        var workEnv = new LLMWorkEnv(AIWorkTypeKind.Instruction, AIServiceTypeKind.Vision, 
            AIServiceVendorKind.OpenAI, OpenaiAPIKey);

        var worker = (InstructionWorker)AIWorkerBuilder.Build(workEnv);

        const string ImageUri = "https://blog.kakaocdn.net/dn/n1V12/btsmBg04xXX/D3WEKyPLWgIVs5McD9KkX1/img.jpg";
        const string UserMessage = "이미지에 표시된 텍스트는 JSON으로 작성해줘. 아래한글이라는 단어가 있으면 아래한글이라고 꼭 해주고.";

        var chatHistory = new ChatHistory("You are a friendly assistant.");

        chatHistory.AddUserMessage(new ChatMessageContentItemCollection
        {
            new TextContent(UserMessage),
            new ImageContent(new Uri(ImageUri))
        });

        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
        var queryContent = "작성 일자는?";
        var request = new QueryRequest(chatHistory, serviceSetting, MemoryCollectionName, queryContent: queryContent, isWithImage: true);

        System.Console.WriteLine("이미지 처리 프롬프트: " + UserMessage);

        chatHistory = await worker.QueryAsync(request);
        System.Console.WriteLine("응답: " + LLMWorkEnv.WorkerContext.Variables.Context);
        System.Console.WriteLine("Query: " + queryContent);
        System.Console.WriteLine(chatHistory.LastContent);
    }

    //public static async Task Run_ImportDocument()
    //{
    //    System.Console.WriteLine("Import Document");

    //    var logger = NullLogger.Instance;
    //    var workEnv = new LLMWorkEnv(AIWorkTypeKind.ServerlessMemory, AIServiceTypeKind.Embedding,
    //        AIServiceVendorKind.OpenAI, OpenaiAPIKey,
    //        MemoryStoreTypeKind.VectorDatabase, MemoryServiceVendorKind.Pinecone, MemoryEnvironment, MemoryAPIKey);
    //    var worker = (MemoryWorker)AIWorkerBuilder.Build(workEnv);

    //    var request = new DocumentRequest("./khn.docx", documentId: "doc001", null);
    //    worker.ImportAsync(request);
    //}

    
    //private static async Task Run_VectorDatabaseWorker_Pinecone_Retrieve_WithQuery()
    //{
    //    System.Console.WriteLine("Retrieve");

    //    var logger = NullLogger.Instance;
    //    var workEnv = new LLMWorkEnv(AIWorkTypeKind.ServerlessMemory, AIServiceTypeKind.Embedding, AIServiceVendorKind.OpenAI,
    //        OpenaiAPIKey,
    //        MemoryStoreTypeKind.VectorDatabase, MemoryServiceVendorKind.Pinecone, MemoryEnvironment, MemoryAPIKey);
    //    var worker = (MemoryWorker)AIWorkerBuilder.Build(workEnv);

    //    //var query = "what's my name?";
    //    //var query = "where did I grow up?";
    //    var query = "where do I live?";
    //    var memoryQuery = MemoryQuery.Create()
    //        .WithQuery(query);

    //    var request = MemoryQueryRequest.Create(MemoryCollectionName, string.Empty, memoryQuery, 2, 0.7);

    //    await foreach (var answer in worker.QueryAsync(request))
    //    {
    //        System.Console.WriteLine($"Answer: {answer.Metadata.Text}");
    //    }

    //    System.Console.WriteLine("Retrieved");
    //}

    //private static async Task Run_VectorDatabaseWorker_Volatile_Retrieve_WithQuery()
    //{
    //    System.Console.WriteLine("Retrieve");

    //    var logger = NullLogger.Instance;
    //    var workEnv = new LLMWorkEnv(AIWorkTypeKind.ServerlessMemory, AIServiceTypeKind.Embedding, AIServiceVendorKind.OpenAI, OpenaiAPIKey,
    //        MemoryStoreTypeKind.VectorDatabase, MemoryServiceVendorKind.Volatile, MemoryEnvironment, MemoryAPIKey);
    //    var worker = (MemoryWorker)AIWorkerBuilder.Build(workEnv);

    //    var datas = new List<MemorySourceData>
    //    {
    //        new MemorySourceData("info1", "My name is Andrea"),
    //        new MemorySourceData("info2", "I've been living in Seattle since 2005"),
    //        new MemorySourceData("info3", "I work as a tourist operator"),
    //        new MemorySourceData("info4", "I visited France and Italy five times since 2015"),
    //        new MemorySourceData("info5", "My family is from New York")
    //    };

    //    var saveRequest = new MemoryUpsertRequest(MemoryCollectionName, string.Empty, datas);
    //    await worker.SaveAsync(saveRequest);

    //    /////////////////////////////////////////////////////////////////////////////


    //    //var query = "what's my name?";
    //    //var query = "where did I grow up?";
    //    var query = "where do I live?";
        
    //    var memoryQuery = MemoryQuery.Create()
    //        .WithQuery(query);

    //    var request = MemoryQueryRequest.Create(MemoryCollectionName, string.Empty, memoryQuery, 2, 0.7);

    //    await foreach (var answer in worker.QueryAsync(request))
    //    {
    //        System.Console.WriteLine($"Answer: {answer.Metadata.Text}");
    //    }

    //    System.Console.WriteLine("Retrieved");
    //}

    //private static async Task Run_InstructionWorker_Query()
    //{
    //    var logger = NullLogger.Instance;
    //    var workEnv = new LLMWorkEnv(AIWorkTypeKind.Instruction, AIServiceTypeKind.TextCompletion, AIServiceVendorKind.OpenAI, OpenaiAPIKey,
    //        MemoryStoreTypeKind.VectorDatabase, MemoryServiceVendorKind.Pinecone, MemoryEnvironment, MemoryAPIKey);
    //    var worker = (InstructionWorker)AIWorkerBuilder.Build(workEnv);

    //    var chatHistory = new ChatHistory();
    //    var userMessage = "김현남 누구야?";

    //    chatHistory.AddUserMessage(userMessage);
    //    var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
    //    var request = new QueryRequest(chatHistory, serviceSetting, MemoryCollectionName);

    //    System.Console.WriteLine("Query: " + userMessage);

    //    chatHistory = await worker.QueryAsync(request);

    //    System.Console.WriteLine(chatHistory.LastContent);
    //}

    //private static async Task Run_EmbeddingWorker_Embedding()
    //{
    //    System.Console.WriteLine("Embedding datas.");

    //    var logger = NullLogger.Instance;
    //    var workEnv = new LLMWorkEnv(AIWorkTypeKind.ServerlessMemory, AIServiceTypeKind.Embedding, AIServiceVendorKind.OpenAI, OpenaiAPIKey,
    //        MemoryStoreTypeKind.VectorDatabase, MemoryServiceVendorKind.Volatile, MemoryEnvironment, MemoryAPIKey);

    //    var worker = (EmbeddingWorker)AIWorkerBuilder.Build(workEnv);

    //    var request = new EmbeddingRequest(new MemorySourceData("info1", "My name is Andrea"));
    //    var embedding =await worker.GenerateEmbeddingsAsync(request);

    //    System.Console.WriteLine("Embedding:");
    //    System.Console.WriteLine(embedding.ToJson());
    //}

    //private static async Task Run_VectorDatabaseWorker_Pinecone_Save()
    //{
    //    System.Console.WriteLine("Saving datas.");

    //    var logger = NullLogger.Instance;
    //    var workEnv = new LLMWorkEnv(AIWorkTypeKind.ServerlessMemory, AIServiceTypeKind.Embedding, AIServiceVendorKind.OpenAI, OpenaiAPIKey,
    //        MemoryStoreTypeKind.VectorDatabase, MemoryServiceVendorKind.Pinecone, MemoryEnvironment, MemoryAPIKey);
    //    var worker = (MemoryWorker)AIWorkerBuilder.Build(workEnv);

    //    var datas = new List<MemorySourceData>
    //    {
    //        new MemorySourceData("info1", "My name is Andrea"),
    //        new MemorySourceData("info2", "I've been living in Seattle since 2005"),
    //        new MemorySourceData("info3", "I work as a tourist operator"),
    //        new MemorySourceData("info4", "I visited France and Italy five times since 2015"),
    //        new MemorySourceData("info5", "My family is from New York")
    //    };

    //    var request = new MemoryUpsertRequest(MemoryCollectionName, string.Empty, datas);
    //    await worker.SaveAsync(request);

    //    System.Console.WriteLine("Saved datas.");
    //}
    //private static async Task Run_VectorDatabaseWorker_Pinecone_Retrieve_WithId()
    //{
    //    System.Console.WriteLine("Retrieve");

    //    var logger = NullLogger.Instance;
    //    var workEnv = new LLMWorkEnv(AIWorkTypeKind.ServerlessMemory, AIServiceTypeKind.Embedding, AIServiceVendorKind.OpenAI, OpenaiAPIKey,
    //        MemoryStoreTypeKind.VectorDatabase, MemoryServiceVendorKind.Pinecone, MemoryEnvironment, MemoryAPIKey);
    //    var worker = (MemoryWorker)AIWorkerBuilder.Build(workEnv);

    //    var memoryQuery = MemoryQuery.Create()
    //        .WithId("info1");
    //    var request = MemoryQueryRequest.Create(MemoryCollectionName, string.Empty, memoryQuery);
    //    var result = await worker.GetAsync(request);

    //    System.Console.WriteLine("Retrieved data:");
    //    System.Console.WriteLine(result?.Metadata.Text);
    //}

    //private static async Task Run_VectorDatabaseWorker_Volatile_Save_Retrieve()
    //{
    //    System.Console.WriteLine("Saving datas.");

    //    var logger = NullLogger.Instance;
    //    var workEnv = new LLMWorkEnv(AIWorkTypeKind.ServerlessMemory, AIServiceTypeKind.Embedding, AIServiceVendorKind.OpenAI, OpenaiAPIKey,
    //        MemoryStoreTypeKind.VectorDatabase, MemoryServiceVendorKind.Volatile, MemoryEnvironment, MemoryAPIKey);
    //    var worker = (MemoryWorker)AIWorkerBuilder.Build(workEnv);

    //    var datas = new List<MemorySourceData>
    //    {
    //        new MemorySourceData("info1", "My name is Andrea"),
    //        new MemorySourceData("info2", "I've been living in Seattle since 2005"),
    //        new MemorySourceData("info3", "I work as a tourist operator"),
    //        new MemorySourceData("info4", "I visited France and Italy five times since 2015"),
    //        new MemorySourceData("info5", "My family is from New York")
    //    };

    //    var request = new MemoryUpsertRequest(MemoryCollectionName, string.Empty, datas);
    //    await worker.SaveAsync(request);

    //    System.Console.WriteLine("Saved datas.");

    //    /////////////////////////////////////////////////////////////////////////////
    //    System.Console.WriteLine("Retrieve");

    //    var query = MemoryQuery.Create()
    //        .WithId("info1");
        
    //    var queryRequest = MemoryQueryRequest.Create(MemoryCollectionName, string.Empty,query);
    //    var result = await worker.GetAsync(queryRequest);

    //    System.Console.WriteLine("Retrieved data:");
    //    System.Console.WriteLine(result?.Metadata.Text);
    //}

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

    private static Dictionary<string, string> GetVectorEmbeddingSampleData()
    {
        return new Dictionary<string, string>
        {
            ["https://github.com/microsoft/semantic-kernel/blob/main/README.md"]
                = "README: Installation, getting started, and how to contribute",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks/02-running-prompts-from-file.ipynb"]
                = "Jupyter notebook describing how to pass prompts from a file to a semantic plugin or function",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks//00-getting-started.ipynb"]
                = "Jupyter notebook describing how to get started with the Semantic Kernel",
            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/plugins/ChatPlugin/ChatGPT"]
                = "Sample demonstrating how to create a chat plugin interfacing with ChatGPT",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/SemanticKernel/Memory/VolatileMemoryStore.cs"]
                = "C# class that defines a volatile embedding store",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/dotnet/KernelHttpServer/README.md"]
                = "README: How to set up a Semantic Kernel Service API using Azure Function Runtime v4",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/chat-summary-webapp-react/README.md"]
                = "README: README associated with a sample chat summary react-based webapp",
        };
    }
}


//        Console.WriteLine("== PART 3b: Recall (similarity search) with Kernel and TextMemoryPlugin 'Recall' function ==");
//        Console.WriteLine("Ask: where do I live?");

//        result = await kernel.InvokeAsync(memoryPlugin["Recall"], new()
//        {
//            [TextMemoryPlugin.InputParam] = "Ask: where do I live?",
//            [TextMemoryPlugin.CollectionParam] = MemoryCollectionName,
//            [TextMemoryPlugin.LimitParam] = "2",
//            [TextMemoryPlugin.RelevanceParam] = "0.79",
//        }, cancellationToken);

//        Console.WriteLine($"Answer: {result.GetValue<string>()}");
//        Console.WriteLine();

//        /*
//        Output:

//            Ask: where did I grow up?
//            Answer:
//                ["My family is from New York","I\u0027ve been living in Seattle since 2005"]

//            Ask: where do I live?
//            Answer:
//                ["I\u0027ve been living in Seattle since 2005","My family is from New York"]
//        */

//        /////////////////////////////////////////////////////////////////////////////////////////////////////
//        // PART 4: TextMemoryPlugin Recall in a Prompt Function
//        //
//        // Looks up related memories when rendering a prompt template, then sends the rendered prompt to
//        // the text generation model to answer a natural language query.
//        /////////////////////////////////////////////////////////////////////////////////////////////////////

//        Console.WriteLine("== PART 4: Using TextMemoryPlugin 'Recall' function in a Prompt Function ==");

//        // Build a prompt function that uses memory to find facts
//        const string RecallFunctionDefinition = @"
//Consider only the facts below when answering questions:

//BEGIN FACTS
//About me: {{recall 'where did I grow up?'}}
//About me: {{recall 'where do I live now?'}}
//END FACTS

//Question: {{$input}}

//Answer:
//";

//        var aboutMeOracle = kernel.CreateFunctionFromPrompt(RecallFunctionDefinition, new OpenAIPromptExecutionSettings() { MaxTokens = 100 });

//        result = await kernel.InvokeAsync(aboutMeOracle, new()
//        {
//            [TextMemoryPlugin.InputParam] = "Do I live in the same town where I grew up?",
//            [TextMemoryPlugin.CollectionParam] = MemoryCollectionName,
//            [TextMemoryPlugin.LimitParam] = "2",
//            [TextMemoryPlugin.RelevanceParam] = "0.79",
//        }, cancellationToken);

//        Console.WriteLine("Ask: Do I live in the same town where I grew up?");
//        Console.WriteLine($"Answer: {result.GetValue<string>()}");

//        /*
//        Approximate Output:
//            Answer: No, I do not live in the same town where I grew up since my family is from New York and I have been living in Seattle since 2005.
//        */

//        /////////////////////////////////////////////////////////////////////////////////////////////////////
//        // PART 5: Cleanup, deleting database collection
//        //
//        /////////////////////////////////////////////////////////////////////////////////////////////////////

//        Console.WriteLine("== PART 5: Cleanup, deleting database collection ==");

//        Console.WriteLine("Printing Collections in DB...");
//        var collections = memoryStore.GetCollectionsAsync(cancellationToken);
//        await foreach (var collection in collections)
//        {
//            Console.WriteLine(collection);
//        }
//        Console.WriteLine();

//        Console.WriteLine("Removing Collection {0}", MemoryCollectionName);
//        await memoryStore.DeleteCollectionAsync(MemoryCollectionName, cancellationToken);
//        Console.WriteLine();

//        Console.WriteLine($"Printing Collections in DB (after removing {MemoryCollectionName})...");
//        collections = memoryStore.GetCollectionsAsync(cancellationToken);
//        await foreach (var collection in collections)
//        {
//            Console.WriteLine(collection);
//        }
//    }




//public static async Task Run_InstructionWorker_Query_Ex()
//{
//    var logger = NullLogger.Instance;
//    var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
//        LLMWorkTypeKind.Instruction, logger);
//    var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//    var chatHistory = new ChatHistory();
//    var userMessage = "What's Ferrari stock price?";

//    System.Console.WriteLine("Query: " + userMessage);

//    chatHistory.AddUserMessage(userMessage);
//    var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
//    var request = new QueryRequest(chatHistory, serviceSetting, true);

//    chatHistory = await worker.QueryAsync(request);

//    System.Console.WriteLine("=========");
//    System.Console.WriteLine("Retrieval: " + LLMWorkEnv.WorkerContext.Variables.Context);
//    System.Console.WriteLine("=========");
//    System.Console.WriteLine("Answer: " + chatHistory.LastContent);
//}




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






//    private static async Task Run_InstructionWorker_NativeFunction_MathSkill()
//    {
//        System.Console.WriteLine("======== NativeFunction_MathSkill ========");

//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);


//        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
//        var request = new PipelineRunRequest(serviceSetting);
//        request.AddPluginFunctionName("MathSkill", "TranslateToNCalc");
//        request.AddPluginFunctionName("MathSkill", "Evaluate");

//        System.Console.WriteLine("Problem: ");
//        var problem = "250, 3240, 288의 최소공배수를 구해.";
//        //var problem = "What is the square root of 625.";
//        System.Console.WriteLine(problem);
//        LLMWorkEnv.WorkerContext.Variables.UpdateInput(problem);
//        var chatHistory = await worker.RunPipelineAsync(request);

//        System.Console.WriteLine("Answer: ");
//        System.Console.WriteLine(chatHistory.PipelineLastContent);
//    }

//    private static async Task SpeechToTextWithMicrophone()
//    {
//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var recorder = new SpeechRecorder("./temp.mp3", worker.AIServiceConnector!);
//        recorder.StartRecording();

//        System.Console.ReadKey();

//        await recorder.StopRecording();
//    }

//    public static async Task<string> Run_InstructionWorker_GenerateImage(string imageDescription)
//    {
//        System.Console.WriteLine("======== Generating Image ========");

//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.ImageGeneration, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
//        var request = new ImageGenerationRequest(imageDescription, 1024, 1024);

//        var image = await worker.GenerateImageAsync(request);

//        System.Console.WriteLine(imageDescription);
//        System.Console.WriteLine("Image URL: " + image);

//        System.Console.WriteLine("이미지 생성 완료.");

//        return image;
//    }

//    public static async Task Run_InstructionWorker_GenerateImage_UsingChat()
//    {
//        System.Console.WriteLine("======== Generating Image UsingChat ========");

//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var chatHistory = new ChatHistory();

//        var userMessage =
//            "An ink sketch style illustration of a small hedgehog holding a piece of watermelon with its tiny paws, taking little bites with its eyes closed in delight.";


//        chatHistory.AddUserMessage(userMessage);
//        System.Console.WriteLine("User: " + userMessage);

//        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
//        var request = new TextGenerationRequest("ImageSkill", "GenerateDescription", chatHistory, serviceSetting);

//        System.Console.WriteLine("Bot: ");
//        chatHistory = await worker.GenerateTextAsync(request);
//        await Run_InstructionWorker_GenerateImage(chatHistory.LastContent);
//    }

//    public static async Task Run_InstructionWorker_TextToSpeech_Stream()
//    {
//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.TextToSpeechSpeed, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var text = "안녕하세요. 반갑습니다. 저는 뉴테크프라임 대표 컨설턴트 김현남입니다. " + GetTtsText();

//        LLMWorkEnv.WorkerContext.Variables.UpdateInput(text);
//        System.Console.WriteLine(text);
//        var request = new AudioGenerationRequest(OpenAIConfigProvider.ProvideTtsVoice(TtsVoiceKind.Alloy));

//        await using var stream = await worker.GenerateAudioStreamAsync(request);
//        PlayTextToSpeechStream(stream);

//        System.Console.WriteLine("TTS 완료.");
//    }

//    private static void PlayTextToSpeechFile(string filepath)
//    {
//        using var mf = new MediaFoundationReader(filepath);
//        using var wo = new WaveOutEvent();
//        wo.Init(mf);
//        wo.Play();
//        while (wo.PlaybackState == PlaybackState.Playing)
//        {
//            Thread.Sleep(1000);
//        }
//    }

//    private static void PlayTextToSpeechStream(Stream stream)
//    {
//        using var mf = new StreamMediaFoundationReader(stream);
//        using var wo = new WaveOutEvent();
//        wo.Init(mf);
//        wo.Play();
//        while (wo.PlaybackState == PlaybackState.Playing)
//        {
//            Thread.Sleep(1000);
//        }
//    }

//    public static async Task Run_InstructionWorker_TextToSpeech_SaveFile()
//    {
//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.TextToSpeechQuality, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var filepath = "./speech.mp3";
//        var text = "안녕하세요. 반갑습니다. 저는 뉴테크프라임 대표 컨설턴트 김현남입니다. " + GetTtsText();
//        LLMWorkEnv.WorkerContext.Variables.UpdateInput(text);
//        System.Console.WriteLine(text);
//        var request = new AudioGenerationRequest(filepath, OpenAIConfigProvider.ProvideTtsVoice(TtsVoiceKind.Onyx));
//        await worker.GenerateAudioAsync(request);

//        System.Console.WriteLine("TTS 완료.");
//    }

//    public static async Task Run_InstructionWorker_AudioTranscription_Ko()
//    {
//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var filepath = "./kmk.mp3";

//        var trimmedAudioFiles = AudioTranscriptionHelper.TrimSilence(filepath);
//        var request = new SpeechToTextRunRequest(SpeechSourceTypeKind.Files, "ko", new byte[] { }, trimmedAudioFiles);

//        var audioTranscription = await worker.RunSpeechToTextAsync(request);
//        System.Console.WriteLine(audioTranscription);
//    }

//    public static async Task Run_InstructionWorker_AudioTranscription_Ko_Correct()
//    {
//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var filepath = "./kmk.mp3";
//        var trimmedAudioFiles = AudioTranscriptionHelper.TrimSilence(filepath);
//        var transcriptionRunRequest =
//            new SpeechToTextRunRequest(SpeechSourceTypeKind.Files, "ko", new byte[] { }, trimmedAudioFiles);

//        var audioTranscription = await worker.RunSpeechToTextAsync(transcriptionRunRequest);
//        System.Console.WriteLine(audioTranscription);


//        workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
//        var request = new PipelineRunRequest(serviceSetting);
//        request.AddPluginFunctionName("AudioSkill", "CorrectKoreanTranscription");
//        LLMWorkEnv.WorkerContext.Variables["CorrectlySpelledWords"] = "생성AI";
//        LLMWorkEnv.WorkerContext.Variables.UpdateInput(audioTranscription);
//        var chatHistory = await worker.RunPipelineAsync(request);
//        System.Console.WriteLine("Correct AudioTranscription:");
//        System.Console.WriteLine(chatHistory.PipelineLastContent);
//    }

//    public static async Task Run_InstructionWorker_AudioTranscription_Correct()
//    {
//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var earningsCallUrl = "https://cdn.openai.com/API/examples/data/EarningsCall.wav";
//        var earningsCallFilepath = "./EarningsCall.wav";

//        await AudioTranscriptionHelper.DownloadAudioFile(earningsCallUrl, earningsCallFilepath);
//        var trimmedAudioFiles = AudioTranscriptionHelper.TrimSilence(earningsCallFilepath);

//        var transcriptionRunRequest =
//            new SpeechToTextRunRequest(SpeechSourceTypeKind.Files, "en", new byte[] { }, trimmedAudioFiles);
//        var audioTranscription = await worker.RunSpeechToTextAsync(transcriptionRunRequest);
//        System.Console.WriteLine("AudioTranscription:" + audioTranscription);

//        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
//        var request = new PipelineRunRequest(serviceSetting);
//        request.AddPluginFunctionName("AudioSkill", "CorrectTranscription");
//        LLMWorkEnv.WorkerContext.Variables["CompanyName"] = "NewTechPrime";
//        LLMWorkEnv.WorkerContext.Variables["CorrectlySpelledWords"] = "UML";
//        LLMWorkEnv.WorkerContext.Variables.UpdateInput(audioTranscription);
//        var chatHistory = await worker.RunPipelineAsync(request);
//        System.Console.WriteLine("Correct AudioTranscription:" + chatHistory.PipelineLastContent);
//    }

//    public static async Task Run_InstructionWorker_AudioTranscription()
//    {
//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var earningsCallUrl = "https://cdn.openai.com/API/examples/data/EarningsCall.wav";
//        var earningsCallFilepath = "./EarningsCall.wav";

//        await AudioTranscriptionHelper.DownloadAudioFile(earningsCallUrl, earningsCallFilepath);
//        var trimmedAudioFiles = AudioTranscriptionHelper.TrimSilence(earningsCallFilepath);

//        var request = new SpeechToTextRunRequest(SpeechSourceTypeKind.Files, "en", new byte[] { }, trimmedAudioFiles);
//        var audioTranscription = await worker.RunSpeechToTextAsync(request);
//        System.Console.WriteLine(audioTranscription);
//    }

//    private static async Task Run_InstructionWorker_NativeFunction_StaticTextSkill()
//    {
//        System.Console.WriteLine("======== NativeFunction_StaticTextSkill ========");

//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);


//        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
//        var request = new PipelineRunRequest(serviceSetting);
//        request.AddPluginFunctionName("StaticTextSkill", "AppendDay");
//        LLMWorkEnv.WorkerContext.Variables.UpdateInput("Today is: ");
//        LLMWorkEnv.WorkerContext.Variables.Set("day", DateTimeOffset.Now.ToString("dddd", CultureInfo.CurrentCulture));

//        System.Console.WriteLine("Answer: ");

//        var chatHistory = await worker.RunPipelineAsync(request);
//        System.Console.WriteLine(chatHistory.PipelineLastContent);
//    }

//    public static async Task Run_InstructionWorker_NativeFunction_TextSkill()
//    {
//        System.Console.WriteLine("======== NativeFunction_TextSkill ========");

//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
//        var request = new PipelineRunRequest(serviceSetting);
//        request.AddPluginFunctionName("TextSkill", "Uppercase");
//        LLMWorkEnv.WorkerContext.Variables.UpdateInput("ciao!");

//        System.Console.WriteLine("Answer: ");

//        var chatHistory = await worker.RunPipelineAsync(request);
//        System.Console.Write(chatHistory.PipelineLastContent);
//    }


//    private static Task PrintSemanticFunctionCategory()
//    {
//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.TextCompletion, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var categories = LLMWorkEnv.PluginStore!.SemanticFunctionCategories;
//        foreach (var category in categories)
//        {
//            System.Console.WriteLine(category.Name);

//            foreach (var subCategory in category.SubCategories)
//            {
//                System.Console.WriteLine("- " + subCategory.Name + ":" + subCategory.Content);
//            }
//        }

//        return Task.CompletedTask;
//    }


//    private static async void Run_InstructionWorker_Generate_Summarize()
//    {
//        var logger = NullLogger.Instance;
//        var workEnv = new LLMWorkEnv(LLMVendorKind.OpenAI, AIServiceTypeKind.SpeechToText, OpenaiAPIKey,
//            LLMWorkTypeKind.Instruction, logger);
//        var worker = LLMWorkerBuilder.BuildInstructionWorker(workEnv);

//        var chatHistory = new ChatHistory();
//        var serviceSetting = LLMServiceSettingBuilder.Build(LLMRequestLevelKind.Middle);
//        var request = new TextGenerationRequest("SummarizeSkill", "Summarize", chatHistory, serviceSetting);

//        var input = GetSummarizeText();
//        LLMWorkEnv.WorkerContext.Variables.UpdateInput(input);

//        chatHistory = await worker.GenerateTextAsync(request);

//        System.Console.WriteLine(chatHistory.LastContent);
//    }


//var expectedText = Guid.NewGuid().ToString();
//var anyFilePath = Guid.NewGuid().ToString();

//var fileSystemConnectorMock = new Mock<IFileSystemConnector>();
//fileSystemConnectorMock
//    .Setup(mock => mock.GetFileContentStreamAsync(It.Is<string>(filePath => filePath.Equals(anyFilePath, StringComparison.Ordinal)),
//        It.IsAny<CancellationToken>()))
//    .ReturnsAsync(Stream.Null);

//var documentConnectorMock = new Mock<IDocumentConnector>();
//documentConnectorMock
//    .Setup(mock => mock.ReadText(It.IsAny<Stream>()))
//    .Returns(expectedText);

//var target = new DocumentPlugin(documentConnectorMock.Object, fileSystemConnectorMock.Object);

//// Act
//string actual = await target.ReadTextAsync(anyFilePath);

//// Assert
//Assert.Equal(expectedText, actual);
//fileSystemConnectorMock.VerifyAll();
//documentConnectorMock.VerifyAll();




/* Use MemoryServerlessClient to run the default import pipeline
* in the same process, without distributed queues.



// Uploading some text, without using files. Hold a copy of the ID to delete it later.
Console.WriteLine("Uploading text about E=mc^2");
var docId = await memory.ImportTextAsync("In physics, mass–energy equivalence is the relationship between mass and energy " +
                                         "in a system's rest frame, where the two quantities differ only by a multiplicative " +
                                         "constant and the units of measurement. The principle is described by the physicist " +
                                         "Albert Einstein's formula: E = m*c^2");
toDelete.Add(docId);

// Simple file upload, with document ID
toDelete.Add("doc001");
Console.WriteLine("Uploading article file about Carbon");
await memory.ImportDocumentAsync("file1-Wikipedia-Carbon.txt", documentId: "doc001");

// Extract memory from images (if OCR enabled)
if (useImages)
{
    toDelete.Add("img001");
    Console.WriteLine("Uploading Image file with a news about a conference sponsored by Microsoft");
    await memory.ImportDocumentAsync(new Document("img001").AddFiles(new[] { "file6-ANWC-image.jpg" }));
}

// Uploading multiple files and adding a user tag, checking if the document already exists
toDelete.Add("doc002");
if (!await memory.IsDocumentReadyAsync(documentId: "doc002"))
{
    Console.WriteLine("Uploading a text file, a Word doc, and a PDF about Semantic Kernel");
    await memory.ImportDocumentAsync(new Document("doc002")
        .AddFiles(new[] { "file2-Wikipedia-Moon.txt", "file3-lorem-ipsum.docx", "file4-SK-Readme.pdf" })
        .AddTag("user", "Blake"));
}
else
{
    Console.WriteLine("doc002 already uploaded.");
}

// Categorizing files with several tags
toDelete.Add("doc003");
if (!await memory.IsDocumentReadyAsync(documentId: "doc003"))
{
    Console.WriteLine("Uploading a PDF with a news about NASA and Orion");
    await memory.ImportDocumentAsync(new Document("doc003")
        .AddFile("file5-NASA-news.pdf")
        .AddTag("user", "Taylor")
        .AddTag("collection", "meetings")
        .AddTag("collection", "NASA")
        .AddTag("collection", "space")
        .AddTag("type", "news"));
}
else
{
    Console.WriteLine("doc003 already uploaded.");
}

// Downloading web pages
toDelete.Add("webPage1");
if (!await memory.IsDocumentReadyAsync("webPage1"))
{
    Console.WriteLine("Uploading https://raw.githubusercontent.com/microsoft/kernel-memory/main/README.md");
    await memory.ImportWebPageAsync("https://raw.githubusercontent.com/microsoft/kernel-memory/main/README.md", documentId: "webPage1");
}
else
{
    Console.WriteLine("webPage1 already uploaded.");
}

// Custom pipelines, e.g. excluding summarization
toDelete.Add("webPage2");
if (!await memory.IsDocumentReadyAsync("webPage2"))
{
    Console.WriteLine("Uploading https://raw.githubusercontent.com/microsoft/kernel-memory/main/docs/security/security-filters.md");
    await memory.ImportWebPageAsync("https://raw.githubusercontent.com/microsoft/kernel-memory/main/docs/security/security-filters.md",
        documentId: "webPage2",
        steps: Constants.PipelineWithoutSummary);
}
else
{
    Console.WriteLine("webPage2 already uploaded.");
}
}

// =======================
// === RETRIEVAL =========
// =======================

if (retrieval)
{
Console.WriteLine("\n====================================\n");

// Question without filters
var question = "What's E = m*c^2?";
Console.WriteLine($"Question: {question}");

var answer = await memory.AskAsync(question, minRelevance: 0.76);
Console.WriteLine($"\nAnswer: {answer.Result}");

Console.WriteLine("\n====================================\n");

// Another question without filters
question = "What's Semantic Kernel?";
Console.WriteLine($"Question: {question}");

answer = await memory.AskAsync(question, minRelevance: 0.76);
Console.WriteLine($"\nAnswer: {answer.Result}\n\n  Sources:\n");

// Show sources / citations
foreach (var x in answer.RelevantSources)
{
    Console.WriteLine(x.SourceUrl != null
        ? $"  - {x.SourceUrl} [{x.Partitions.First().LastUpdate:D}]"
        : $"  - {x.SourceName}  - {x.Link} [{x.Partitions.First().LastUpdate:D}]");
}

if (useImages)
{
    Console.WriteLine("\n====================================\n");
    question = "Which conference is Microsoft sponsoring?";
    Console.WriteLine($"Question: {question}");

    answer = await memory.AskAsync(question, minRelevance: 0.76);
    Console.WriteLine($"\nAnswer: {answer.Result}\n\n  Sources:\n");

    // Show sources / citations
    foreach (var x in answer.RelevantSources)
    {
        Console.WriteLine(x.SourceUrl != null
            ? $"  - {x.SourceUrl} [{x.Partitions.First().LastUpdate:D}]"
            : $"  - {x.SourceName}  - {x.Link} [{x.Partitions.First().LastUpdate:D}]");
    }
}

Console.WriteLine("\n====================================\n");

// Filter question by "user" tag
question = "Any news from NASA about Orion?";
Console.WriteLine($"Question: {question}");

// Blake doesn't know
answer = await memory.AskAsync(question, filter: MemoryFilters.ByTag("user", "Blake"));
Console.WriteLine($"\nBlake Answer (none expected): {answer.Result}");

// Taylor knows
answer = await memory.AskAsync(question, filter: MemoryFilters.ByTag("user", "Taylor"));
Console.WriteLine($"\nTaylor Answer: {answer.Result}\n  Sources:\n");

// Show sources / citations
foreach (var x in answer.RelevantSources)
{
    Console.WriteLine(x.SourceUrl != null
        ? $"  - {x.SourceUrl} [{x.Partitions.First().LastUpdate:D}]"
        : $"  - {x.SourceName}  - {x.Link} [{x.Partitions.First().LastUpdate:D}]");
}

Console.WriteLine("\n====================================\n");

// Filter question by "type" tag, there are news but no articles
question = "What is Orion?";
Console.WriteLine($"Question: {question}");

answer = await memory.AskAsync(question, filter: MemoryFilters.ByTag("type", "article"));
Console.WriteLine($"\nArticles (none expected): {answer.Result}");

answer = await memory.AskAsync(question, filter: MemoryFilters.ByTag("type", "news"));
Console.WriteLine($"\nNews: {answer.Result}");
}

// =======================
// === PURGE =============
// =======================

if (purge)
{
Console.WriteLine("====================================");

foreach (var docId in toDelete)
{
    Console.WriteLine($"Deleting memories derived from {docId}");
    await memory.DeleteDocumentAsync(docId);
}
}

// ReSharper disable CommentTypo
/* ==== OUTPUT ====

====================================

Uploading text about E=mc^2
Uploading article file about Carbon
Uploading Image file with a news about a conference sponsored by Microsoft
Uploading a text file, a Word doc, and a PDF about Semantic Kernel
Uploading a PDF with a news about NASA and Orion
Uploading https://raw.githubusercontent.com/microsoft/kernel-memory/main/README.md
Uploading https://raw.githubusercontent.com/microsoft/kernel-memory/main/docs/security/security-filters.md

====================================

Question: What's E = m*c^2?

Answer: E = m*c^2 is a formula in physics that describes the mass–energy equivalence. This principle, proposed by Albert Einstein, states that the energy of an object (E) is equal to the mass (m) of that object times the speed of light (c) squared. This relationship is observed in a system's rest frame, where mass and energy differ only by a multiplicative constant and the units of measurement.

====================================

Question: What's Semantic Kernel?

Answer: Semantic Kernel (SK) is a lightweight Software Development Kit (SDK) that enables the integration of AI Large Language Models (LLMs) with conventional programming languages. It combines natural language semantic functions, traditional code native functions, and embeddings-based memory to unlock new potential and add value to applications with AI.

SK supports prompt templating, function chaining, vectorized memory, and intelligent planning capabilities. It encapsulates several design patterns from the latest AI research, allowing developers to infuse their applications with plugins like prompt chaining, recursive reasoning, summarization, zero/few-shot learning, contextual memory, long-term memory, embeddings, semantic indexing, planning, retrieval-augmented generation, and accessing external knowledge stores as well as your own data.

Semantic Kernel is available for use with C# and Python and can be explored and used to build AI-first apps. It is an open-source project, inviting developers to contribute and join in its development.

Sources:

- file4-SK-Readme.pdf  - doc002/a166fd04b91a44cd919a300e84931bdf [Friday, December 8, 2023]
- content.url  - webPage1/fbcb60da9d5a4ba1a390e108941fc7ad [Friday, December 8, 2023]
- content.url  - webPage2/79a67b4f470b43549fce1b9a3de21c95 [Friday, December 8, 2023]

====================================

Question: Which conference is Microsoft sponsoring?

Answer: Microsoft is sponsoring the Automotive News World Congress 2023 event, which is taking place in Detroit, Michigan on September 12, 2023.

Sources:

- file6-ANWC-image.jpg  - img001/ac7d8bc0051945a689aa23d1fa9092b2 [Friday, December 8, 2023]
- file5-NASA-news.pdf  - doc003/be2411fdc3e84c5995a7753beb927ecd [Friday, December 8, 2023]
- content.url  - webPage1/fbcb60da9d5a4ba1a390e108941fc7ad [Friday, December 8, 2023]
- file4-SK-Readme.pdf  - doc002/a166fd04b91a44cd919a300e84931bdf [Friday, December 8, 2023]
- file3-lorem-ipsum.docx  - doc002/a1269887842d4748980cbdd7e1aabc12 [Friday, December 8, 2023]

====================================

Question: Any news from NASA about Orion?

Blake Answer (none expected): INFO NOT FOUND

Taylor Answer: Yes, NASA has invited media to see the new test version of the Orion spacecraft and the hardware teams will use to recover the capsule and astronauts upon their return from space during the Artemis II mission. The event is scheduled to take place at 11 a.m. PDT on Wednesday, Aug. 2, at Naval Base San Diego. Teams are currently conducting the first in a series of tests in the Pacific Ocean to demonstrate and evaluate the processes, procedures, and hardware for recovery operations for crewed Artemis missions. The tests will help prepare the team for Artemis II, NASA’s first crewed mission under Artemis that will send four astronauts in Orion around the Moon to checkout systems ahead of future lunar missions. The Artemis II crew – NASA astronauts Reid Wiseman, Victor Glover, and Christina Koch, and CSA (Canadian Space Agency) astronaut Jeremy Hansen – will participate in recovery testing at sea next year.
Sources:

- file5-NASA-news.pdf  - doc003/be2411fdc3e84c5995a7753beb927ecd [Friday, December 8, 2023]

====================================

Question: What is Orion?

Articles (none expected): INFO NOT FOUND

News: Orion is a spacecraft developed by NASA. It is being used in the Artemis II mission, which is NASA's first crewed mission under the Artemis program. The mission will send four astronauts in the Orion spacecraft around the Moon to check out systems ahead of future lunar missions.
====================================
Deleting memories derived from d421ecd8e79747ec8ed5f1db49baba2c202312070451357022920
Deleting memories derived from doc001
Deleting memories derived from img001
Deleting memories derived from doc002
Deleting memories derived from doc003
Deleting memories derived from webPage1
Deleting memories derived from webPage2
*/



//using Microsoft.KernelMemory;
//using Microsoft.KernelMemory.AI.OpenAI;

//// Use this boolean to decide whether to use OpenAI or Azure OpenAI models
//const bool UseAzure = true;

//var azureOpenAIEmbeddingConfig = new AzureOpenAIConfig();
//var azureOpenAITextConfig = new AzureOpenAIConfig();
//var openAIConfig = new OpenAIConfig();

//new ConfigurationBuilder()
//    .AddJsonFile("appsettings.json")
//    .AddJsonFile("appsettings.Development.json", optional: true)
//    .Build()
//    .BindSection("KernelMemory:Services:OpenAI", openAIConfig)
//    .BindSection("KernelMemory:Services:AzureOpenAIText", azureOpenAITextConfig)
//    .BindSection("KernelMemory:Services:AzureOpenAIEmbedding", azureOpenAIEmbeddingConfig);

//// Note: this example is storing data in memory, so summaries are lost once the program completes.
////       You can customize the code to persist the data, or simply point to a Kernel Memory service.
////var memory = new MemoryWebClient("http://127.0.0.1:9001");
//var memory = new KernelMemoryBuilder()
//    .Configure(UseAzure,
//        builder => builder
//            .WithAzureOpenAITextGeneration(azureOpenAITextConfig, new DefaultGPTTokenizer())
//            .WithAzureOpenAITextEmbeddingGeneration(azureOpenAIEmbeddingConfig, new DefaultGPTTokenizer()),
//        builder => builder.WithOpenAI(openAIConfig))
//    .Build<MemoryServerless>();

//// Import a couple of documents to summarize.
//// Note that we're using a custom set of steps, asking the pipeline to just summarize the docs (ie skipping chunking)
//await memory.ImportDocumentAsync(new Document("doc1")
//        .AddFile("file4-SK-Readme.pdf")
//        .AddFile("file5-NASA-news.pdf"),
//    steps: Constants.PipelineOnlySummary);

//// Fetch the list of summaries. The API returns one summary for each file.
//var results = await memory.SearchSummariesAsync(filter: MemoryFilters.ByDocument("doc1"));

//// Print the summaries!
//foreach (var result in results)
//{
//    Console.WriteLine($"== {result.SourceName} summary ==\n{result.Partitions.First().Text}\n");
//}



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
/// 