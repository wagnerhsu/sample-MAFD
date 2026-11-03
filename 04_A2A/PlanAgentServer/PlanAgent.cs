using A2A;

namespace PlanAgentServer;

/// <summary>
/// A2A Agent —— 以 A2A 协议对外提供能力。
/// </summary>
public sealed class PlanAgent : IAgentHandler
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
            🎡 **景点推荐**
            
            为您推荐上海必游景点：
            
            **历史文化类**
            1. 🏛️ 外滩 - 欣赏万国建筑博览群
            2. 🏯 豫园 - 江南古典园林代表
            3. 🕌 城隍庙 - 品尝地道上海小吃
            
            **现代都市类**
            4. 🗼 东方明珠塔 - 上海地标，俯瞰浦江两岸
            5. 🌆 陆家嘴 - 金融中心，上海之巅
            6. 🛍️ 南京路步行街 - 购物天堂
            
            **文艺休闲类**
            7. 🎨 田子坊 - 文艺小店聚集地
            8. 📚 武康路 - 梧桐树下的法式风情
            9. 🌳 世纪公园 - 城市绿肺，亲子游首选
            
            📅 建议游玩时间：3-4 天可覆盖主要景点
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
        Name = "plan agent",
        Description = "travel plan & attraction agent",
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
                Id = "attraction-recommendation",
                Name = "景点推荐",
                Description = "推荐目的地的热门景点和游玩路线，包括历史文化、现代都市、文艺休闲等类型",
                Tags = ["attraction", "sightseeing", "tourism", "travel"],
                Examples = ["上海有什么好玩的", "北京必去的景点", "杭州西湖怎么玩"],
                InputModes = ["text"],
                OutputModes = ["text"]
            }
        ],
    };
}
