# TaskManager API

A production-grade Task Management REST API built with ASP.NET Core following 
Clean Architecture principles.

## Tech Stack

- **Runtime:** .NET 10, ASP.NET Core Web API
- **Database:** SQL Server + Entity Framework Core
- **Validation:** FluentValidation
- **Testing:** xUnit, Moq, FluentAssertions
- **CI/CD:** GitHub Actions
- **Docs:** Swagger / OpenAPI

## Architecture

The solution follows Clean Architecture with four layers:
```
TaskManager.Domain          → Entities, Interfaces, Enums (no dependencies)
TaskManager.Application     → Services, DTOs, Validators (depends on Domain)
TaskManager.Infrastructure  → EF Core, Repositories, UnitOfWork (depends on Domain)
TaskManager.API             → Controllers, Middleware, DI wiring (depends on Application)
```

Dependencies point inward only — Domain has zero framework dependencies.

## Design Patterns Used

| Pattern | Where |
|---|---|
| Repository | `ITaskRepository`, `IUserRepository`, `IProjectRepository` |
| Unit of Work | `IUnitOfWork` — single `SaveChangesAsync()` per operation |
| Result | `Result<T>` — expected failures as return values, not exceptions |
| Factory Method | `TaskItem.Create()`, `User.Create()`, `Project.Create()` |

## SOLID Principles Applied

- **S** — Each entity owns its own creation and business rules
- **O** — New services added without modifying existing ones
- **L** — All repository implementations are substitutable for their interfaces
- **I** — `ITaskService` and `IProjectService` are focused, not one giant interface
- **D** — Services depend on `IUnitOfWork`, never on `AppDbContext` directly

## Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (local or Docker)

### Run locally
```bash
# Clone the repo
git clone https://github.com/YOUR_USERNAME/TaskManager.git
cd TaskManager

# Update connection string in TaskManager.API/appsettings.json

# Apply migrations
dotnet ef database update --project TaskManager.Infrastructure --startup-project TaskManager.API

# Run the API
dotnet run --project TaskManager.API --launch-profile http
```

Open `http://localhost:5178` to see Swagger UI.

### Run tests
```bash
dotnet test TaskManager.Tests
```

## CI/CD Pipeline

Every push to `main` and every Pull Request automatically:

1. Restores NuGet packages (cached for speed)
2. Builds the solution in Release configuration
3. Runs all 26 unit tests — pipeline fails if any test fails
4. Publishes the API as a deployable artifact

## API Endpoints

### Projects
| Method | Endpoint | Description |
|---|---|---|
| GET | /api/projects | Get all projects |
| GET | /api/projects/{id} | Get project by ID |
| POST | /api/projects | Create a project |
| DELETE | /api/projects/{id} | Delete a project |

### Tasks
| Method | Endpoint | Description |
|---|---|---|
| GET | /api/tasks | Get all tasks |
| GET | /api/tasks/{id} | Get task by ID |
| GET | /api/tasks/user/{userId} | Get tasks by user |
| POST | /api/tasks | Create a task |
| PATCH | /api/tasks/{id}/status | Update task status |
| DELETE | /api/tasks/{id} | Delete a task |