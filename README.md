### MCPHostApp - Key Implementation Steps for the InProcessToolsPlusMCPTools Project

This project implements a unified tool invocation system that supports both MCP (Model Context Protocol) server tools and in-process tools. Below are the key implementation steps:

### 1. Core Architecture Design

**Unified Tool Interface (`IUnifiedTool`)**:
- Defines a unified tool abstraction with `Name`, `Description`, `ToAIFunction()`, and `InvokeAsync()` methods
- Enables different types of tools to be operated through the same interface

### 2. Tool Wrapper Implementations

**MCP Tool Wrapper (`McpToolWrapper`)**:
- Wraps external MCP server tools (e.g., Brave Search)
- Converts MCP tools to `AIFunction`
- Handles parameter conversion and remote invocations

**In-Process Tool Wrapper (`InProcessToolWrapper<T>`)**:
- Wraps local C# methods as tools
- Uses reflection and `AIFunctionFactory` to create `AIFunction`
- Supports automatic registration of methods with `[Description]` attributes

### 3. Unified Tool Manager (`UnifiedToolManager`)

- **Tool Registration**: Provides `RegisterMcpToolsAsync()` and `RegisterInProcessTool()` methods
- **Unified Interface**: Offers `ChatAsync()` and `ChatStreamAsync()` methods
- **Tool Listing**: Retrieves all available tools via `GetAllAIFunctions()`

### 4. Specific Tool Implementations

**In-Process Tool Examples**:
- `WeatherService`: Provides weather query functionality
- `CalculatorService`: Offers mathematical calculation capabilities (addition, subtraction, multiplication, division, square root, exponentiation)

**External MCP Tools**:
- Brave Search: A search service connected via the MCP protocol

### 5. Main Program Flow (`Program.cs`)

```csharp
// 1. Create Ollama client with function invocation configuration
IChatClient client = new ChatClientBuilder(new OllamaChatClient(...))
  .UseFunctionInvocation(...)
  .Build();

// 2. Create unified tool manager
var toolManager = new UnifiedToolManager(client);

// 3. Register MCP tools
IMcpClient mcpClient = await McpClientFactory.CreateAsync(...);
await toolManager.RegisterMcpToolsAsync(mcpClient);

// 4. Register in-process tools
toolManager.RegisterInProcessTool(new WeatherService());
toolManager.RegisterInProcessTool(new CalculatorService());

// 5. Use unified chat interface
await foreach (var chunk in toolManager.ChatStreamAsync(input))
{
    Console.Write(chunk);
}
```

### 6. Key Technical Features

- **Type Safety**: Uses strongly-typed `AIFunction` and parameter validation
- **Async Support**: Fully supports asynchronous operations and streaming responses
- **Extensibility**: Easily add new tool types through the unified interface
- **Automatic Discovery**: Uses reflection and attributes for automatic tool method discovery and registration
- **Error Handling**: Includes parameter validation and exception handling mechanisms

The core value of this implementation lies in unifying tools from different sources (local methods and remote services) into a single invocation framework, allowing AI models to seamlessly call various functions without needing to care about the specific implementation details of the tools.

## MCPHostApp - InProcessToolsPlusMCPTools 项目关键实现步骤

这个项目实现了一个统一的工具调用系统，能够同时支持 MCP (Model Context Protocol) 服务器工具和进程内工具。以下是关键实现步骤：

### 1. 核心架构设计

**统一工具接口 (`IUnifiedTool`)**：
- 定义了统一的工具抽象，包含 `Name`、`Description`、`ToAIFunction()` 和 `InvokeAsync()` 方法
- 使得不同类型的工具可以通过相同的接口进行操作

### 2. 工具封装器实现

**MCP 工具封装器 (`McpToolWrapper`)**：
- 封装外部 MCP 服务器工具（如 Brave Search）
- 将 MCP 工具转换为 `AIFunction`
- 处理参数转换和远程调用

**进程内工具封装器 (`InProcessToolWrapper<T>`)**：
- 封装本地 C# 方法作为工具
- 使用反射和 `AIFunctionFactory` 创建 `AIFunction`
- 支持带有 `[Description]` 特性的方法自动注册

### 3. 统一工具管理器 (`UnifiedToolManager`)

- **工具注册**：提供 `RegisterMcpToolsAsync()` 和 `RegisterInProcessTool()` 方法
- **统一接口**：提供 `ChatAsync()` 和 `ChatStreamAsync()` 方法
- **工具列表**：通过 `GetAllAIFunctions()` 获取所有可用工具

### 4. 具体工具实现

**进程内工具示例**：
- `WeatherService`：提供天气查询功能
- `CalculatorService`：提供数学计算功能（加减乘除、开方、幂运算）

**外部 MCP 工具**：
- Brave Search：通过 MCP 协议连接的搜索服务

### 5. 主程序流程 (`Program.cs`)

```csharp
// 1. 创建 Ollama 客户端，配置函数调用
IChatClient client = new ChatClientBuilder(new OllamaChatClient(...))
  .UseFunctionInvocation(...)
  .Build();

// 2. 创建统一工具管理器
var toolManager = new UnifiedToolManager(client);

// 3. 注册 MCP 工具
IMcpClient mcpClient = await McpClientFactory.CreateAsync(...);
await toolManager.RegisterMcpToolsAsync(mcpClient);

// 4. 注册进程内工具
toolManager.RegisterInProcessTool(new WeatherService());
toolManager.RegisterInProcessTool(new CalculatorService());

// 5. 使用统一聊天接口
await foreach (var chunk in toolManager.ChatStreamAsync(input))
{
    Console.Write(chunk);
}
```

### 6. 关键技术特点

- **类型安全**：使用强类型的 `AIFunction` 和参数验证
- **异步支持**：全面支持异步操作和流式响应
- **可扩展性**：通过统一接口轻松添加新的工具类型
- **自动发现**：使用反射和特性自动发现和注册工具方法
- **错误处理**：包含参数验证和异常处理机制

这个实现的核心价值在于将不同来源的工具（本地方法和远程服务）统一到同一个调用框架中，让 AI 模型能够无缝地调用各种功能，而不需要关心工具的具体实现方式。
        