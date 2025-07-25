using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

/*Question: The local phone number of restaurants in Chinatown in Adelaide, and then the weather of Melbourne, and then Add 2 3*/
// Create Ollama client - OllamApiClient vs OllamChatClient
IChatClient client = new ChatClientBuilder(new OllamaChatClient(new Uri("http://10.0.4.3:11434"), "qwen2.5:32b"))
  .UseFunctionInvocation(
    configure: cfg =>
    {
      cfg.MaximumIterationsPerRequest = 10;
      cfg.AllowConcurrentInvocation = false;
    })
  .Build();

// MCP server: Brave Search configuration
bool isWindows = OperatingSystem.IsWindows();
string command = isWindows ? "cmd.exe" : "/bin/sh";
string[] arguments = isWindows
    ? new[] { "/C", "set BRAVE_API_KEY=xxxxxx && npx -y @modelcontextprotocol/server-brave-search" }
    : new[] { "-c", "BRAVE_API_KEY=xxxxxx npx -y @modelcontextprotocol/server-brave-search" };


// Create the unified tool manager
var toolManager = new UnifiedToolManager(client);

// Register MCP tools
IMcpClient mcpClient = await McpClientFactory.CreateAsync(
  new StdioClientTransport(new StdioClientTransportOptions()
  {
    Command = command,
    Arguments = arguments,
    Name = "MCP Server Brave Search",
  }));

await toolManager.RegisterMcpToolsAsync(mcpClient);

// Register in-process tools
toolManager.RegisterInProcessTool(new WeatherService());
toolManager.RegisterInProcessTool(new CalculatorService());

// List all available tools
toolManager.ListTools();

// Use unified chat interface
while (true)
{
  Console.Write("Your Query: ");
  var input = Console.ReadLine();
  if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
    break;

  Console.Write("Answer: ");
  await foreach (var chunk in toolManager.ChatStreamAsync(input))
  {
    Console.Write(chunk);
  }
  Console.WriteLine();
}