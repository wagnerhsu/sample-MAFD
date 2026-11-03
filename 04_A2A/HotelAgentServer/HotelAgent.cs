using A2A;

namespace HotelAgentServer;

/// <summary>
/// A2A Agent —— 以 A2A 协议对外提供能力。
/// </summary>
public sealed class HotelAgent : IAgentHandler
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
            🏨 **酒店推荐**
            
            根据您的需求，为您推荐以下酒店：
            
            **豪华型 ⭐⭐⭐⭐⭐**
            1. 上海外滩华尔道夫酒店
               - 📍 外滩核心位置，江景房
               - 💰 ¥2,500/晚起
               - ⭐ 评分 4.9/5.0
            
            **舒适型 ⭐⭐⭐⭐**
            2. 上海静安香格里拉大酒店
               - 📍 静安寺商圈，交通便利
               - 💰 ¥1,200/晚起
               - ⭐ 评分 4.7/5.0
            
            **经济型 ⭐⭐⭐**
            3. 全季酒店（上海南京路店）
               - 📍 南京路步行街旁
               - 💰 ¥380/晚起
               - ⭐ 评分 4.5/5.0
            
            💡 提示：建议提前预订，周末和节假日价格可能上涨 20-50%。
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
        Name = "hotel-a2a-agent",
        Description = "hotel information agent",
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
                Id = "hotel-recommendation",
                Name = "酒店推荐",
                Description = "根据目的地和预算推荐合适的酒店，包括豪华型、舒适型、经济型",
                Tags = ["hotel", "accommodation", "booking", "travel"],
                Examples = ["推荐上海的酒店", "上海外滩附近有什么好酒店", "预算500以内的北京酒店"],
                InputModes = ["text"],
                OutputModes = ["text"]
            }
        ],
    };
}
