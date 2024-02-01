using AISmarteasy.Service;
using AISmarteasy.Service.Pinecone;

namespace AISmarteasy.Core.Worker;

public static class AIWorkerBuilder
{
    public static AIWorker Build(LLMWorkEnv workEnv)
    {
        switch (workEnv.WorkType)
        {
            case AIWorkTypeKind.Instruction:
            {
                IMemoryStore? memoryStore;
                switch (workEnv.MemoryStoreType)
                {
                    case MemoryStoreTypeKind.VectorDatabase:
                    {
                        switch (workEnv.MemoryServiceVendor)
                        {
                            case MemoryServiceVendorKind.Pinecone:
                                Verifier.NotNullOrWhitespace(workEnv.MemoryServiceEnvironment);
                                Verifier.NotNullOrWhitespace(workEnv.MemoryServiceAPIKey);

                                memoryStore = new PineconeMemoryStore(workEnv.MemoryServiceEnvironment,
                                    workEnv.MemoryServiceAPIKey);
                                break;
                            default:
                                memoryStore = new NullMemoryStore();
                                break;
                        }

                        break;
                    }
                    default:
                        memoryStore = new VolatileMemoryStore();
                        break;
                }

                Verifier.NotNull(memoryStore);
                IMemory memory = new ServerlessMemory();
                MemoryWorker memoryWorker = new MemoryWorker(workEnv, memory);

                return new InstructionWorker(workEnv, memoryWorker);
            }
        }

        return new NullWorker(workEnv);
    }
}
