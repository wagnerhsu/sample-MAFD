using A2A;
using A2A.AspNetCore;
using WeatherAgentServer;

var builder = WebApplication.CreateBuilder(args);

// 注册 Agent 处理器与 Agent Card（同时装配 A2AServer、ITaskStore 等基础设施）
builder.Services.AddA2AAgent<WeatherAgent>(WeatherAgent.GetAgentCard("https://localhost:7011/weather"));

var app = builder.Build();

// Add JSON-RPC endpoint for A2A
app.MapA2A("/weather");
// Add HTTP+JSON (REST) endpoint for A2A
app.MapHttpA2A(app.Services.GetRequiredService<IA2ARequestHandler>(), "/weather");
// Add well-known agent card endpoint for A2A discovery
app.MapWellKnownAgentCard(app.Services.GetRequiredService<AgentCard>());

app.Run();
