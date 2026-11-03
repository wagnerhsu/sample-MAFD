using A2A;

namespace WeatherAgentServer;

/// <summary>
/// A2A Agent —— 以 A2A 协议对外提供能力。
/// </summary>
public sealed class WeatherAgent : IAgentHandler
{
    /// <summary>
    /// 处理 A2A 请求消息，并把结果写回事件队列。
    /// </summary>
    public async Task ExecuteAsync(
        RequestContext context,
        AgentEventQueue eventQueue,
        CancellationToken cancellationToken)
    {
        // 以无状态消息（Message）的方式直接回复客户端
        var responder = new MessageResponder(eventQueue, context.ContextId);
        await responder.ReplyAsync($"""
            🌤️ **天气查询结果**
            
            查询时间：{DateTime.Now:yyyy-MM-dd HH:mm}
            
            **北京天气**
            - 今日：晴转多云，气温 -2°C ~ 8°C
            - 明日：多云，气温 0°C ~ 10°C
            - 后日：阴，气温 2°C ~ 9°C
            
            **上海天气**
            - 今日：多云，气温 5°C ~ 12°C
            - 明日：小雨，气温 6°C ~ 10°C
            - 后日：阴转晴，气温 4°C ~ 11°C
            
            👔 穿衣建议：北京较冷，建议穿羽绒服；上海温和，建议穿夹克外套，带好雨具。
            """, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 取消正在执行的任务（本示例无需额外处理）。
    /// </summary>
    public Task CancelAsync(
        RequestContext context,
        AgentEventQueue eventQueue,
        CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// 构造 Agent Card，供客户端通过 well-known 地址发现本 Agent 的能力。
    /// </summary>
    public static AgentCard GetAgentCard(string agentUrl) => new()
    {
        Name = "weather agent",
        Description = "weather information agent",
        Version = "1.0.0",
        SupportedInterfaces =
        [
            new AgentInterface
            {
                Url = agentUrl,
                ProtocolBinding = "JSONRPC",
                ProtocolVersion = "1.0",
            }
        ],
        DefaultInputModes = ["text"],
        DefaultOutputModes = ["text"],
        Capabilities = new AgentCapabilities
        {
            Streaming = true,
            PushNotifications = false,
        },
        Skills =
        [
            new AgentSkill
            {
                Id = "weather-query",
                Name = "天气查询",
                Description = "查询指定城市的天气预报，包括温度、降水概率、穿衣建议等",
                Tags = ["weather", "forecast", "climate"],
                Examples = ["上海明天天气怎么样", "成都这周的天气预报", "杭州下雨吗"],
                InputModes = ["text"],
                OutputModes = ["text"]
            }
        ],
    };
}
