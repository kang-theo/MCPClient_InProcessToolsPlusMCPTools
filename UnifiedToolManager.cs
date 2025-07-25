using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

public class UnifiedToolManager
{
  private readonly List<IUnifiedTool> _tools = [];
  private readonly IChatClient _chatClient;

  public UnifiedToolManager(IChatClient chatClient)
  {
    _chatClient = chatClient;
  }

  // Register MCP tools
  public async Task RegisterMcpToolsAsync(IMcpClient mcpClient)
  {
    var mcpTools = await mcpClient.ListToolsAsync();
    foreach (var tool in mcpTools)
    {
      _tools.Add(new McpToolWrapper(tool, mcpClient));
    }
  }

  // Register in-process tools
  public void RegisterInProcessTool<T>(T instance) where T : class
  {
    var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance)
      .Where(m => m.GetCustomAttribute<DescriptionAttribute>() != null);

    foreach (var method in methods)
    {
      _tools.Add(new InProcessToolWrapper<T>(instance, method));
    }
  }

  // Get all tools as AIFunction for the chat client
  public IEnumerable<AIFunction> GetAllAIFunctions()
  {
    return _tools.Select(t => t.ToAIFunction());
  }

  // Get tool by name for manual invocation
  public IUnifiedTool? GetTool(string name)
  {
    return _tools.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
  }

  // List all available tools
  public void ListTools()
  {
    Console.WriteLine("Available unified tools:");
    foreach (var tool in _tools)
    {
      Console.WriteLine($"- {tool.Name}: {tool.Description}");
    }
  }

  // Unified chat with all tools
  public async Task<string> ChatAsync(string prompt, CancellationToken cancellationToken = default)
  {
    var messages = new List<ChatMessage> { new(ChatRole.User, prompt) };
    var chatOptions = new ChatOptions
    {
      Tools = GetAllAIFunctions().Cast<AITool>().ToList()
    };

    var response = await _chatClient.GetResponseAsync(messages, chatOptions, cancellationToken);
    //return response.Message.Text ?? string.Empty;
    return response.Messages.ToString();
  }

  // Streaming chat with all tools
  public async IAsyncEnumerable<string> ChatStreamAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    var messages = new List<ChatMessage> { new(ChatRole.User, prompt) };
    var chatOptions = new ChatOptions
    {
      Tools = GetAllAIFunctions().Cast<AITool>().ToList()
    };

    await foreach (var update in _chatClient.GetStreamingResponseAsync(messages, chatOptions, cancellationToken))
    {
      if (!string.IsNullOrEmpty(update.Text))
      {
        yield return update.Text;
      }
    }
  }
}