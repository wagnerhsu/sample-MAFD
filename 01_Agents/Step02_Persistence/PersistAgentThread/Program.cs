// Load configuration
using CommonShared;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using PersistAgentThread.Infrastructure;
using System.ClientModel;
using System.Text.Json;

var config = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
#if DEBUG
    .AddJsonFile($"appsettings.Secrets.json", optional: true, reloadOnChange: true)
#endif
    .Build();
var openAIProvider = config.GetSection("OpenAI").Get<OpenAIProvider>();

// Step1. Create one Agent
var mazdaAgent = new OpenAIClient(
        new ApiKeyCredential(openAIProvider.ApiKey),
        new OpenAIClientOptions { Endpoint = new Uri(openAIProvider.Endpoint) })
    .GetChatClient(openAIProvider.ModelId)
    .AsIChatClient()
    .AsAIAgent(name: "Powerful Assistant", instructions: "You are a helpful assistant who responds user message in Mazda cars.");
// Step2. Start a new session
var userMessage = "Hello, can you tell me about the Mazda 3?";
Console.WriteLine($"User> {userMessage}");
var session = await mazdaAgent.CreateSessionAsync();
var agentResponse = await mazdaAgent.RunAsync(userMessage, session);
Console.WriteLine($"Agent> {agentResponse}");
// Step3. Persist the session
var serializedSession = (await mazdaAgent.SerializeSessionAsync(session, JsonSerializerOptions.Web)).GetRawText();
var chatConversation = new CustomChatHistory(serializedSession);
var dbContext = new ChatHistoryDbContext();
dbContext.Database.EnsureCreated();
dbContext.ChatHistories.Add(chatConversation);
await dbContext.SaveChangesAsync();
// Step4. Load the session later from database
var savedConversation = dbContext.ChatHistories.First(c => c.Id == chatConversation.Id);
var loadedSession = JsonSerializer.Deserialize<JsonElement>(savedConversation.Context, JsonSerializerOptions.Web);
var resumedSession = await mazdaAgent.DeserializeSessionAsync(loadedSession, JsonSerializerOptions.Web);
// Step5. Continue the conversation
Console.WriteLine();
userMessage = "What are the features of this car?";
Console.WriteLine($"User> {userMessage}");
agentResponse = await mazdaAgent.RunAsync(userMessage, resumedSession);
Console.WriteLine($"Agent> {agentResponse}");

Console.ReadKey();