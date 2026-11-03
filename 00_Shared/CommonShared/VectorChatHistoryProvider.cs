using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System.Text.Json;

namespace CommonShared;

/// <summary>
/// 将对话历史持久化到向量存储的 <see cref="ChatHistoryProvider"/> 实现。
/// 会话的存储键保存在 <see cref="AgentSession.StateBag"/> 中，
/// 因此可以随会话状态的序列化一起持久化，会话恢复后仍能定位到同一份历史记录。
/// </summary>
public sealed class VectorChatHistoryProvider : ChatHistoryProvider
{
    private readonly ProviderSessionState<State> _sessionState;
    private readonly VectorStore _vectorStore;
    private IReadOnlyList<string>? _stateKeys;

    /// <summary>
    /// 初始化向量存储对话历史提供器。
    /// </summary>
    /// <param name="vectorStore">用于保存对话历史的向量存储。</param>
    /// <param name="stateInitializer">可选的会话状态初始化委托，用于自定义存储键的生成方式。</param>
    /// <param name="stateKey">可选的会话状态键名，默认使用当前类型名。</param>
    public VectorChatHistoryProvider(
        VectorStore vectorStore,
        Func<AgentSession?, State>? stateInitializer = null,
        string? stateKey = null)
    {
        this._sessionState = new ProviderSessionState<State>(
            stateInitializer ?? (_ => new State(Guid.NewGuid().ToString("N"))),
            stateKey ?? this.GetType().Name,
            JsonSerializerOptions.Web);
        this._vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
    }

    /// <inheritdoc />
    public override IReadOnlyList<string> StateKeys => this._stateKeys ??= [this._sessionState.StateKey];

    /// <summary>
    /// 获取指定会话在向量存储中的键。
    /// </summary>
    public string GetSessionDbKey(AgentSession session)
        => this._sessionState.GetOrInitializeState(session).SessionDbKey;

    /// <summary>
    /// 从向量存储中读取该会话的历史消息。
    /// </summary>
    protected override async ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(
        InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        var state = this._sessionState.GetOrInitializeState(context.Session);

        var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);

        var records = new List<ChatHistoryItem>();
        await foreach (var record in collection.GetAsync(
            x => x.SessionId == state.SessionDbKey, 10,
            new() { OrderBy = x => x.Descending(y => y.Timestamp) },
            cancellationToken))
        {
            records.Add(record);
        }

        var messages = records.ConvertAll(x => JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage!)!);
        messages.Reverse();
        return messages;
    }

    /// <summary>
    /// 把本轮请求与响应写入向量存储。
    /// </summary>
    protected override async ValueTask StoreChatHistoryAsync(
        InvokedContext context,
        CancellationToken cancellationToken = default)
    {
        var state = this._sessionState.GetOrInitializeState(context.Session);

        var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);

        var allNewMessages = context.RequestMessages.Concat(context.ResponseMessages ?? []);

        await collection.UpsertAsync(allNewMessages.Select(x => new ChatHistoryItem()
        {
            Key = state.SessionDbKey + x.MessageId,
            Timestamp = DateTimeOffset.UtcNow,
            SessionId = state.SessionDbKey,
            SerializedMessage = JsonSerializer.Serialize(x),
            MessageText = x.Text
        }), cancellationToken);
    }

    /// <summary>
    /// 保存在 <see cref="AgentSession.StateBag"/> 中的会话级状态。
    /// </summary>
    public sealed class State
    {
        public State(string sessionDbKey)
        {
            this.SessionDbKey = sessionDbKey ?? throw new ArgumentNullException(nameof(sessionDbKey));
        }

        public string SessionDbKey { get; }
    }

    /// <summary>
    /// 保存在向量存储中的单条历史记录。
    /// </summary>
    private sealed class ChatHistoryItem
    {
        [VectorStoreKey]
        public string? Key { get; set; }

        [VectorStoreData]
        public string? SessionId { get; set; }

        [VectorStoreData]
        public DateTimeOffset? Timestamp { get; set; }

        [VectorStoreData]
        public string? SerializedMessage { get; set; }

        [VectorStoreData]
        public string? MessageText { get; set; }
    }
}
