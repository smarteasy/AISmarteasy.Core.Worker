콘솔 애플리케이션 AISmarteasy.Core.Worker.Console 프로젝트를 실행하기 위해서는
- WorkerEnv를 추가하고, 아래와 같이 자신의 발급 OPENAI_API_KEY작성한다.

public static class WorkerEnv
{
    public const string OPENAI_API_KEY = "";
}

프롬프트 템플릿을 실행 경로에 추가한다.
