# Server Monitor

A multi-user web application for monitoring servers and services. Each user keeps their own
list of targets; a background service checks each one on its own interval, pushes status
changes to the browser in real time, raises alerts when something goes down, and charts
uptime, response time and error rate over time.

Status: in progress. Expected completion: 2026-09-15.

## Features

- HTTP/HTTPS, ICMP ping and TCP port checks, each with a configurable interval and timeout
- Live dashboard that updates without a page refresh
- Alerts raised on a down transition and resolved automatically on recovery, with in-app toasts
- Uptime, average response time and error rate over a 24 hour, 7 day or 30 day window
- Cookie-based sign-in with per-user data isolation and an `Admin` role for user management

## Architecture

Clean architecture, with dependencies pointing inward:

| Project | Responsibility |
| --- | --- |
| `src/ServerMonitor.Domain` | Entities and enums. No dependencies. |
| `src/ServerMonitor.UseCases` | Business logic behind repository and checker ports. |
| `src/ServerMonitor.Infrastructure` | EF Core, ASP.NET Identity, the checkers, background services. |
| `src/ServerMonitor.Web` | Blazor Server UI and the composition root. |
| `tests/ServerMonitor.UnitTests` | Unit tests for the use case layer. |

The UI runs in Blazor's `InteractiveServer` render mode, so check results reach the browser
over the circuit's SignalR connection.

## Prerequisites

- .NET 10 SDK
- SQL Server LocalDB (installed with Visual Studio; the connection string lives in
  `src/ServerMonitor.Web/appsettings.json`)

## Running

```
dotnet run --project src/ServerMonitor.Web --launch-profile http
```

Migrations are applied at startup, so the database is created on first run. Register an
account at `/Account/Register`, then add a target.

## Configuration

`Monitoring` section of `appsettings.json`:

| Setting | Default | Meaning |
| --- | --- | --- |
| `PollIntervalSeconds` | 5 | How often the engine looks for targets that are due |
| `MaxConcurrentChecks` | 10 | Upper bound on checks running at once |
| `RetentionDays` | 30 | Age at which check results are deleted |

To seed an administrator on startup, set `Seed:AdminEmail` and `Seed:AdminPassword` — use
user secrets rather than committing them:

```
dotnet user-secrets --project src/ServerMonitor.Web set "Seed:AdminPassword" "<password>"
```

## Tests

```
dotnet test
```
