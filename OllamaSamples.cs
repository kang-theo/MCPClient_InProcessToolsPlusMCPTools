using System.ComponentModel;
using Microsoft.Extensions.AI;
//using Microsoft.Extensions.AI.Ollama;
//using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.Abstractions;
//using Microsoft.SemanticKernel.Skills.Core;
//using Microsoft.SemanticKernel.Connectors.AI.Ollama.ChatCompletion;

/*
 * Question: The population of Adelaide and then the weather in Melbourne
 * https://github.com/dotnet/ai-samples/blob/main/src/microsoft-extensions-ai/ollama/OllamaExamples/ToolCalling.cs
 * https://modelcontextprotocol.io/docs/concepts/tools
 * https://modelcontextprotocol.io/quickstart/client#c%23
 * https://learn.microsoft.com/en-us/dotnet/ai/microsoft-extensions-ai?utm_source=chatgpt.com#tool-calling
*/
public static class OllamaSamples
{
  public static async Task ToolCallingAsync()
  {
    [ToolInfo(
      Name = "GetWeather",
      Description = "Get the weather and return sunny or rainy",
      InputSchema = "{ \"type\": \"object\", \"properties\": {} }",
      AnnotationTitle = "Get Weather",
      AnnotationReadOnlyHint = true,
      AnnotationDestructiveHint = false,
      AnnotationIdempotentHint = false,
      AnnotationOpenWorldHint = false
    )]
    string GetWeather()
    {
      var randomValue = Random.Shared.NextDouble();
      Console.WriteLine($"[GetWeather Tool] Random value used: {randomValue:F2}");

      return randomValue > 0.5 ? "It's sunny" : "It's raining";
    }

    [Description("Gets the population")]
    string GetPopulation()
    {
      var randomValue = Random.Shared.NextDouble();
      Console.WriteLine($"[GetPopulation Tool] Random value used: {randomValue:F2}");

      return randomValue > 0.5 ? "Above 5 million" : "Below 5 million";
    }


    //var functions = new SKFunctionCollection();
    //functions.AddFunction(SKFunction.FromNativeMethod(GetWeather));

    var chatOptions = new ChatOptions
    {
      Tools = [AIFunctionFactory.Create(GetWeather),AIFunctionFactory.Create(GetPopulation)]
    };

    const string endpoint = "http://10.0.4.3:11434/";
    const string modelId = "qwen2.5:32b";

    IChatClient weatherClient = new OllamaChatClient(endpoint, modelId)
        .AsBuilder()
        .UseFunctionInvocation(
        configure: cfg =>
          {
            cfg.MaximumIterationsPerRequest = 10;
            cfg.AllowConcurrentInvocation = false;
          })
        .Build();

    while (true)
    {
      Console.WriteLine("Prompt: ");
      var prompt = Console.ReadLine();
      var response = await weatherClient.GetResponseAsync(prompt, chatOptions);
      Console.WriteLine($"Response: {response}");
    }
  }
}

[AttributeUsage(AttributeTargets.Method)]
public class ToolInfoAttribute : Attribute
{
  public string Name { get; set; }
  public string Description { get; set; }
  public object InputSchema { get; set; }
  public string AnnotationTitle { get; set; }
  public bool AnnotationReadOnlyHint { get; set; }
  public bool AnnotationDestructiveHint { get; set; }
  public bool AnnotationIdempotentHint { get; set; }
  public bool AnnotationOpenWorldHint { get; set; }
}

//public class ToolAnnotations
//{
//  public string Title { get; set; }
//  public bool? ReadOnlyHint { get; set; }
//  public bool? DestructiveHint { get; set; }
//  public bool? IdempotentHint { get; set; }
//  public bool? OpenWorldHint { get; set; }
//}