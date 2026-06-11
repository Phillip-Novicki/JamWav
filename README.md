# JamWav

JamWav is a social platform for connecting concert-goers with people who share their music taste.

## Tech Stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- ASP.NET Identity + JWT authentication
- xUnit (unit + integration tests)

## Architecture

The solution follows Clean Architecture, split into four projects (each with a matching test project):

- **JamWav.Domain** — entities and core business rules
- **JamWav.Application** — interfaces, DTOs, and application logic
- **JamWav.Infrastructure** — EF Core persistence, repositories, and external service integrations
- **JamWav.Web** — ASP.NET Core API (controllers, auth, composition root)

## Features

- JWT-based authentication and user accounts
- Bands, Events, and Friends management APIs
- Spotify OAuth integration for music taste profiles (in progress)

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server instance

### Configuration

Add an `appsettings.Development.json` under `JamWav.Web` with your connection string:

```json
{
  "ConnectionStrings": {
    "JamWavDb": "Server=...;Database=JamWav;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Run locally

```bash
dotnet restore
dotnet ef database update --project JamWav.Infrastructure --startup-project JamWav.Web
dotnet run --project JamWav.Web
```

### Run tests

```bash
dotnet test
```

## Roadmap

- [ ] Spotify OAuth + user music taste profiles
- [ ] AI-powered recommendation service
- [ ] Live events integration (Ticketmaster/Songkick)
- [ ] Containerized deployment with CI/CD
