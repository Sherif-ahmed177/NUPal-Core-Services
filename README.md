<div align="center">

<img src="https://img.shields.io/badge/NUPAL-Core%20Services-0078d4?style=for-the-badge&logo=dotnet&logoColor=white" alt="NUPAL Core Services" height="40"/>

# 🎓 NUPAL — Core Services

### *Nile University Portal & Academic Link Backend*

[![Build Status](https://img.shields.io/github/actions/workflow/status/nileuniversity/nupal-core-services/main_nupal.yml?branch=main&style=flat-square&logo=github-actions&logoColor=white)](https://github.com/nileuniversity/nupal-core-services/actions/workflows/main_nupal.yml)
[![.NET Core](https://img.shields.io/badge/.NET-9-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![MongoDB](https://img.shields.io/badge/MongoDB-Atlas-47A248?style=flat-square&logo=mongodb&logoColor=white)](https://www.mongodb.com/)
[![Redis](https://img.shields.io/badge/Redis-Cache-DC382D?style=flat-square&logo=redis&logoColor=white)](https://redis.io/)
[![Azure Deploy](https://img.shields.io/badge/Deployed%20on-Azure-0089D6?style=flat-square&logo=microsoft-azure)](https://azure.microsoft.com/)

<br/>



</div>

---

## 📌 Overview

**NUPAL Core Services** is the backend engine powering the Nile University Portal & Academic Link (NUPAL). Built using Clean Architecture principles in ASP.NET Core 9, it acts as an intelligent coordinator linking academic management, AI-based advising, and career readiness.

> 🎯 *Empowering Nile University students through intelligent course recommendations, real-time advising, resume-to-job matching, and automated class registration.*

---

## ✨ Features

<table>
<tr>
<td width="33%" valign="top">

### 🤖 AI Advising & RAG
- 💬 **Advising Chatbot**: Real-time academic advising utilizing an LLM-powered model.
- 📚 **Policy RAG**: Instant Q&A referencing university regulations and student handbook.
- 🔍 **Execution Tracing**: Detailed logging of agent pipeline steps in MongoDB for transparency.

</td>
<td width="33%" valign="top">

### 🧠 RL Recommendation
- 📈 **Dynamic Profiling**: Generates student feature vectors (GPA, track, course history).
- 🔮 **Multi-Term Slates**: Computes registration suggestions via Reinforcement Learning.
- ⚡ **Background Queues**: Asynchronously precomputes recommendations to ensure low latency.

</td>
<td width="33%" valign="top">

### 💼 Career & Scheduling
- 📄 **Resume Ingestion**: Stores parsed student CV data.
- 🤝 **Job Fit Calculator**: Computes percent compatibility against Wuzzuf scraped jobs.
- 📅 **Conflict-Free Timetables**: Algorithmic scheduling and constraint validation.

</td>
</tr>
</table>

---

## 🧠 AI Advising & RL Recommendation Pipelines

The backend coordinates dual intelligent engines to assist students throughout their academic journey:

| Pipeline | Core Mechanics | Primary Storage / Cache |
|----------|----------------|-------------------------|
| **Advising Chat & RAG** | Intercepts user chats, resolves terms, queries Policy RAG, and streams response. | `ChatConversation`, `AgentPipelineTrace` |
| **RL Recommendation Queue** | Evaluates student academic progress, schedules precomputations, and pushes recommended course arrays. | `RlRecommendation` in MongoDB, cached in Redis |

**Pipeline Highlights:**
- **Dynamic Worker Service**: Background processes (`PrecomputeBackgroundWorker` and `RlPrecomputeQueueWorker`) continually update recommended schedules.
- **Trace Logs**: Agent tracing monitors exact performance variables, prompting system health signals if OpenAI/HuggingFace microservices experience downtime.
- **Cache-Aside Caching**: High-priority recommendations are stored in Redis (`RedisCacheService`) with automated eviction on student record updates.

---

## 🏗️ Tech Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | ASP.NET Core 9.0 (C# 13) |
| **Database** | MongoDB Atlas (NoSQL Document Store) |
| **Cache** | Redis Cache (StackExchange.Redis) |
| **Security** | JWT Bearer Tokens with custom RBAC |
| **Integration** | HttpClient to Python/ML microservices (Hugging Face) |
| **CI/CD** | GitHub Actions (`main_nupal.yml`) & Azure Web Apps |

---

## 🔐 Authentication & Authorization

- 🔑 **Secure Token Issue**: Custom authentication controller (`StudentsController`) validates student records and signs JWTs.
- 🛡️ **Role-Based Access Control (RBAC)**:
  - `Student` — Ask Advising AI, view profiles, analyze resumes, and compute Job Fit.
  - `Admin` — Import bulk student records, adjust settings, and inspect system diagnostics.

Security Highlights:
- Symmetric signing key validation via JWT Bearer Middleware.
- Claim-based student email context boundaries on career and resume repositories.

---

## 🚀 Getting Started

### Prerequisites

- .NET 9.x SDK
- A running MongoDB server
- A running Redis server

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/nileuniversity/NUPAL-Core-Services.git
cd NUPAL-Core-Services

# 2. Configure Local Development Secrets
dotnet user-secrets set "MONGO_URL" "your-mongodb-connection-string" --project NUPAL.Core.Api/NUPAL.Core.Api.csproj
dotnet user-secrets set "Redis:ConnectionString" "localhost:6379" --project NUPAL.Core.Api/NUPAL.Core.Api.csproj
dotnet user-secrets set "Jwt:Key" "YourSuperSecretSymmetricKey123456!" --project NUPAL.Core.Api/NUPAL.Core.Api.csproj

# 3. Build the Solution
dotnet build Nupal.Core.Api.sln -c Debug

# 4. Start the Application
dotnet run --project NUPAL.Core.Api/NUPAL.Core.Api.csproj
```

Open [http://localhost:5009/swagger/index.html](http://localhost:5009/swagger/index.html) to view the API documentation.

### Environment Variables

You can supply these variables directly inside `appsettings.Development.json` or as environment variables:

```env
MONGO_URL=mongodb+srv://...
Redis__ConnectionString=localhost:6379
Jwt__Key=YourSuperSecretSymmetricKey123456!
Jwt__Issuer=NUPAL_Issuer
Jwt__Audience=NUPAL_Audience
RlServiceUrl=https://...hf.space
AgentServiceUrl=https://...hf.space
RagServiceUrl=https://...hf.space
CareerServices__Url=https://...hf.space
```

---

## 📂 Project Structure

```
NUPAL-Core-Services/
├── NUPAL.Core.Api/            # REST Controllers, Middlewares, and Background Workers
├── NUPAL.Core.Application/    # DTOs, interfaces, and core business services
├── NUPAL.Core.Domain/         # Domain entities (Student, Chat, RlJob, etc.)
├── NUPAL.Core.Infrastructure/ # Repositories, Redis integration, and HttpClient Proxies
└── NUPAL.Core.Tests/          # xUnit unit test suites
```

---

## 📅 Advising & Recommendation Workflow

```
Student accesses advising page
       ↓
API checks Redis Cache for precomputed RL slate
       ↓
[Cache Miss] → Background Worker queries HF RL service
       ↓
Result persisted in MongoDB & cached in Redis
       ↓
Recommendations rendered to student ✅
```

---

## 🌍 Impact

NUPAL is designed with a core vision:

> **Unifying student life with artificial intelligence.**

- 🎓 **Guided Academic Paths**: Personalized Reinforcement Learning models preventing student dropouts and poor course mapping.
- 📚 **Equalized Access to Policies**: Standardizing access to college rules via semantic search.
- 🚀 **Bridging Industry & Academia**: Integrating real-world jobs with university courses to prepare students for the job market.

---


<div align="center">

Made with ❤️ by the NUPAL Team

</div>
