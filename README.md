# NUPAL Core Services

Backend services for NU PAL, built with ASP.NET Core and MongoDB.

## Prerequisites
- .NET SDK 9.x
- MongoDB Atlas or local MongoDB

## Configuration
- Set `MONGO_URL` via one of:
  - User Secrets (Development):
    - `dotnet user-secrets set "MONGO_URL" "<your-mongodb-uri>" --project NUPAL.Core.Api/NUPAL.Core.Api.csproj`
  - Environment variable: set `MONGO_URL` in your shell or hosting environment
  - `appsettings.Development.json`: add `{ "MONGO_URL": "<your-mongodb-uri>" }`

## Build & Run
- Build: `dotnet build NUPAL.Core.Api/NUPAL.Core.Api.csproj -c Debug`
- Run: `dotnet run --project NUPAL.Core.Api/NUPAL.Core.Api.csproj`
- API base URL: `http://localhost:5009`

## Key Endpoints
- `POST /api/students/login` – authenticate student with `email` and `password`
- `POST /api/students/import` – import student record

## Projects
- `NUPAL.Core.Domain` – entities and domain models
- `NUPAL.Core.Application` – use cases and service interfaces
- `NUPAL.Core.Infrastructure` – data access and repositories
- `NUPAL.Core.Api` – ASP.NET Core Web API

