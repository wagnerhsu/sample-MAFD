# MAFD - Microsoft Agent Framework Demos

## Project Overview
Microsoft Agent Framework Demos is an open-source project collection designed for learning and demonstrating the Microsoft Agent Framework (MAF).  
This project comprises 20+ meticulously written blog articles, covering a complete learning path from fundamental concepts to advanced workflow orchestration.  
It is particularly suitable for .NET developers who wish to master Microsoft's latest agent development framework.

## Prerequisites
- **.NET 10 SDK** — every sample targets `net10.0` (see `global.json` for the pinned SDK version)
- **Microsoft Agent Framework 1.27.0** — the samples use the stable 1.27.0 line of `Microsoft.Agents.AI.*`.
  A few packages have no stable release yet and track the matching prerelease of the same 1.27.0 line
  (`Microsoft.Agents.AI.Declarative` → `1.27.0-rc1`, `Microsoft.Agents.AI.Hosting` /
  `Microsoft.Agents.AI.DevUI` / `Microsoft.Agents.AI.Hosting.AGUI.AspNetCore` → `1.27.0-preview.261030.1`,
  `Microsoft.Agents.AI.Hosting.OpenAI` → `1.27.0-alpha.261030.1`), and the A2A protocol SDK is
  `A2A.AspNetCore 1.0.0-preview4`.

Each sample reads its model credentials from `appsettings.json` plus an optional
`appsettings.Secrets.json` (git-ignored) that you create locally with your `OpenAI` section.

## Blog Articles Index
Below is the collection of the 20+ blog articles covered in this project:
[https://edisontalk.cnblogs.com/collections/29894](https://www.cnblogs.com/edisontalk/collections/29894)

## Features
### 🚀 Complete Learning Path
Systematic tutorials from zero to advanced features  
Covers all core concepts of the Microsoft Agent Framework
Includes abundant practical code examples

### 🔧 Integrated Tech Stack
Pure .NET technology stack, tailored for .NET development teams  
Combined with ASP.NET Core dependency injection and lifecycle management  
Supports multiple LLM service providers (OpenAI, Azure OpenAI, etc.)

### 🛠️ Real-world Application Scenarios
AI Agent autonomous decision-making and task execution
Complex multi-agent workflow orchestration  
Tool function calls and manual approval processes  
Short-term memory and session management

### 📚 Comprehensive Coverage
‌AI Agent Fundamentals‌ - Core agent components and interaction patterns  
‌Workflow Management‌ - Structured processing of complex processes  
‌Multi-Agent Collaboration‌ - Parallel, sequential, handover, group chat, etc.  
‌Tool Calling‌ - Function calls, manual approval mechanisms
‌State Management‌ - Thread isolation, data persistence  
‌Performance Optimization‌ - Production environment best practices

## Links and Resources
‌Official Documentation‌: [Microsoft Learn - Agent Framework Tutorials](https://learn.microsoft.com/en-us/agents/)  
‌Blog Article Series‌: [Quick Start on MAF Series](https://www.cnblogs.com/edisontalk/p/-/quick-start-on-maf-chatper01)  
‌Model Service Provider‌: [SiliconCloud Free Registration](https://cloud.siliconflow.cn/i/DomqCefW)

## Contact
For questions or suggestions, feel free to contact via:  
Creating an Issue on the [GitHub repository](https://github.com/EdisonTalk/MAFD)  
Contacting the blog author [Edison](https://www.cnblogs.com/edisontalk)