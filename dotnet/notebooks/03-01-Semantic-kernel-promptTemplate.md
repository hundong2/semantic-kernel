# Semantic Kernel 프롬프트 템플릿 렌더링 이해하기

Semantic Kernel에서 프롬프트 템플릿을 만들고 실행하기 전에 실제로 어떻게 작동하는지 보는 과정을 설명합니다. 선택된 코드는 프롬프트가 AI에 전송되기 전의 모습을 미리 확인할 수 있는 중요한 단계입니다.

## 예시 파일

````csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TemplateEngine;
using System;

public class PromptTemplateRenderingExample
{
    public static async Task Main()
    {
        // 1. Kernel 설정 (이전 단계에서 완료됨)
        var kernel = /* kernel 객체 */;
        
        // 2. 프롬프트 템플릿 정의
        string skPrompt = """
        {{$input}}
        
        Summarize the content above.
        """;
        
        Console.WriteLine("=== 프롬프트 템플릿 렌더링 과정 ===\n");
        
        // ===== Step 1: PromptTemplateConfig 생성 =====
        Console.WriteLine("📋 Step 1: PromptTemplateConfig 생성");
        var promptTemplateConfig = new PromptTemplateConfig(skPrompt);
        Console.WriteLine($"설정 생성 완료\n");
        
        // ===== Step 2: KernelPromptTemplateFactory로 템플릿 생성 =====
        Console.WriteLine("📋 Step 2: 프롬프트 템플릿 팩토리를 사용한 템플릿 생성");
        var promptTemplateFactory = new KernelPromptTemplateFactory();
        var promptTemplate = promptTemplateFactory.Create(promptTemplateConfig);
        Console.WriteLine($"템플릿 타입: {promptTemplate.GetType().Name}\n");
        
        // ===== Step 3: 템플릿 렌더링 =====
        Console.WriteLine("📋 Step 3: 템플릿 렌더링 (변수 치환)");
        var renderedPrompt = await promptTemplate.RenderAsync(kernel);
        Console.WriteLine($"렌더링된 프롬프트:\n{renderedPrompt}\n");
        
        // ===== 실제 사용 예시 =====
        Console.WriteLine("=== 실제 데이터로 렌더링 예시 ===\n");
        
        var sampleText = "Semantic Kernel은 AI 기능을 응용 프로그램에 통합하는 SDK입니다.";
        var renderedWithData = await promptTemplate.RenderAsync(kernel, new KernelArguments { ["input"] = sampleText });
        Console.WriteLine($"렌더링된 결과:\n{renderedWithData}");
    }
}
````

## 답변

### 선택된 코드 상세 분석

````csharp
var promptTemplateConfig = new PromptTemplateConfig(skPrompt);

var promptTemplateFactory = new KernelPromptTemplateFactory();
var promptTemplate = promptTemplateFactory.Create(promptTemplateConfig);

var renderedPrompt = await promptTemplate.RenderAsync(kernel);

Console.WriteLine(renderedPrompt);
````

이 코드는 **3단계**의 과정으로 이루어져 있습니다:

### 1단계: PromptTemplateConfig 생성

````csharp
var promptTemplateConfig = new PromptTemplateConfig(skPrompt);
````

**역할**: 프롬프트 템플릿의 설정 정보를 담는 객체 생성

**상세 설명**:
- `skPrompt` 문자열을 받아서 설정 객체로 변환
- 템플릿의 메타데이터(변수, 형식 등)를 파싱
- 프롬프트 문자열 내의 `{{$input}}` 같은 변수 식별

**출력 예시**:
```
설정 객체 생성됨
- 변수: $input
- 템플릿 언어: SK template
```

### 2단계: KernelPromptTemplateFactory로 템플릿 생성

````csharp
var promptTemplateFactory = new KernelPromptTemplateFactory();
var promptTemplate = promptTemplateFactory.Create(promptTemplateConfig);
````

**역할**: 설정을 바탕으로 실제 사용 가능한 템플릿 객체 생성

**상세 설명**:
- `KernelPromptTemplateFactory`: 템플릿 생성을 담당하는 팩토리 클래스
- `Create()`: 설정 정보를 받아 `IPromptTemplate` 인터페이스를 구현한 객체 반환
- 반환된 `promptTemplate`은 렌더링할 수 있는 상태로 준비됨

**비유**: 
- `Config`: 레시피의 재료 목록
- `Factory`: 요리사
- `Template`: 완성된 요리 도구

### 3단계: 템플릿 렌더링

````csharp
var renderedPrompt = await promptTemplate.RenderAsync(kernel);
````

**역할**: 프롬프트 템플릿의 변수들을 실제 값으로 치환

**상세 설명**:
- `RenderAsync()`: 비동기로 템플릿을 렌더링
- `kernel`: 필요한 함수나 변수 정보 제공
- `{{$input}}` 같은 플레이스홀더를 실제 데이터로 변환

**예시**:

| 원본 템플릿 | 렌더링된 결과 |
|-----------|----------|
| `{{$input}}\n\nSummarize the content above.` | `(입력 데이터가 여기 표시)\n\nSummarize the content above.` |

### 4단계: 결과 출력

````csharp
Console.WriteLine(renderedPrompt);
````

**역할**: 렌더링된 프롬프트를 출력하여 확인

**중요성**: AI에 실제로 전송될 프롬프트를 미리 볼 수 있음

### 실제 동작 흐름

````csharp
// 원본 템플릿 ({{$input}} 포함)
string skPrompt = """
{{$input}}

Summarize the content above.
""";

// Step 1: 설정 생성
var promptTemplateConfig = new PromptTemplateConfig(skPrompt);
// → "{{$input}}" 변수 식별

// Step 2: 팩토리로 템플릿 생성
var promptTemplateFactory = new KernelPromptTemplateFactory();
var promptTemplate = promptTemplateFactory.Create(promptTemplateConfig);
// → 실행 가능한 템플릿 객체 생성

// Step 3: 렌더링
var renderedPrompt = await promptTemplate.RenderAsync(kernel);
// → {{$input}} 플레이스홀더 제거 (아직 데이터 없음)

// 출력:
// ""
// Summarize the content above.
````

### 왜 이 과정이 필요한가?

1. **디버깅**: AI에 전송되기 전에 프롬프트의 모습을 확인
2. **검증**: 변수가 올바르게 설정되었는지 확인
3. **최적화**: 프롬프트 구조를 개선하기 전에 미리 볼 수 있음
4. **에러 방지**: 잘못된 템플릿 문법을 조기에 발견

### 더 간단한 방식

위의 긴 과정 대신 다음과 같이 간단히 실행할 수 있습니다:

````csharp
// 방식 1: 직접 함수 생성 및 실행
var summaryFunction = kernel.CreateFunctionFromPrompt(skPrompt);
var result = await kernel.InvokeAsync(summaryFunction, new() { ["input"] = inputText });
Console.WriteLine(result);

// 방식 2: 한 줄로 프롬프트 실행
var result = await kernel.InvokePromptAsync(skPrompt, new() { ["input"] = inputText });
Console.WriteLine(result);
````

### 선택된 코드가 유용한 이유

- **단계별 확인**: 각 단계에서 무엇이 일어나는지 명확히 보임
- **학습 목적**: Semantic Kernel의 내부 작동 원리 이해
- **고급 커스터마이징**: 필요시 각 단계를 개별적으로 수정 가능

### 추가 자료

- [Semantic Kernel 프롬프트 템플릿 가이드](https://learn.microsoft.com/en-us/semantic-kernel/concepts/prompts/prompt-template-syntax)
- [Semantic Kernel 공식 문서 - 템플릿 엔진](https://learn.microsoft.com/en-us/semantic-kernel/concepts/prompts/template-engine)
- [SK 템플릿 언어 문법](https://github.com/microsoft/semantic-kernel/blob/main/docs/PROMPT_TEMPLATE_LANGUAGE.md)
- [Semantic Kernel GitHub 예제](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples)
- [비동기 프로그래밍 (async/await) 이해하기](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/)