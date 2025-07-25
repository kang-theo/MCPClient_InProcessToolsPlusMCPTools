using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

public interface IUnifiedTool
{
  string Name { get; }
  string Description { get; }
  AIFunction ToAIFunction();
  Task<object> InvokeAsync(AIFunctionArguments parameters, CancellationToken cancellationToken = default);
}

// Wrapper for MCP tools
public class McpToolWrapper(McpClientTool mcpTool, IMcpClient mcpClient) : IUnifiedTool
{
  public string Name => mcpTool.Name;
  public string Description => mcpTool.Description ?? string.Empty;

  public AIFunction ToAIFunction()
  {
    // Create a simple function
    return AIFunctionFactory.Create(
      method: InvokeAsync,
      name: mcpTool.Name,
      description: mcpTool.Description ?? string.Empty
    );
  }

  public async Task<object> InvokeAsync(AIFunctionArguments parameters, CancellationToken cancellationToken = default)
  {
    // Convert AIFunctionArguments to dictionary
    var paramDict = new Dictionary<string, object?>();
    foreach (var param in parameters)
    {
      paramDict[param.Key] = param.Value;
    }

    var result = await mcpClient.CallToolAsync(
      mcpTool.Name, 
      paramDict.AsReadOnly(), 
      progress: null, 
      cancellationToken: cancellationToken);
    //return result.Content ?? string.Empty;
    return result.Content;
  }
}

// Wrapper for in-process tools
public class InProcessToolWrapper<T> : IUnifiedTool where T : class
{
  private readonly T _instance;
  private readonly MethodInfo _method;
  private readonly AIFunction _aiFunction;

  public InProcessToolWrapper(T instance, MethodInfo method)
  {
    _instance = instance;
    _method = method;
    _aiFunction = AIFunctionFactory.Create(_method, _instance);
  }

  public string Name => _aiFunction.Name;
  public string Description => _aiFunction.Description ?? string.Empty;

  public AIFunction ToAIFunction() => _aiFunction;

  public async Task<object> InvokeAsync(AIFunctionArguments parameters, CancellationToken cancellationToken = default)
  {
    return await _aiFunction.InvokeAsync(parameters, cancellationToken) ?? string.Empty;
  }
}