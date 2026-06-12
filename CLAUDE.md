# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Run the API (HTTP: localhost:5150, HTTPS: localhost:7074)
dotnet run

# Build
dotnet build

# Apply migrations / create the SQLite database
dotnet ef database update

# Add a new migration
dotnet ef migrations add <MigrationName>

# Watch mode (auto-restart on file changes)
dotnet watch run
```

Swagger UI is available at `https://localhost:7074/swagger` when running in Development.

## Architecture

This is a .NET 10 REST API using SQLite via EF Core with a strict three-layer architecture:

```
Controller → Service → Repository → AppDbContext (SQLite: libros.db)
```

- **Controllers/** — HTTP layer, route `api/[controller]`. Returns typed HTTP results (Ok, NotFound, CreatedAtAction).
- **Services/** — Business logic. Validates and coordinates before delegating to the repository.
- **Repositories/** — Data access only. Calls `SaveChanges()` directly.
- **data/AppDbContext.cs** — Single `DbContext` with one `DbSet<Libro>`.
- **model/** — EF Core entity classes.

When adding a new entity, the expected flow is: create the model class → add a `DbSet` to `AppDbContext` → add a repository → add a service → add a controller → create and apply a migration.

## Conventions

- Variable, method, and class names are in Spanish (e.g., `ObtenerTodos`, `Agregar`, `Eliminar`).
- Repository methods are synchronous (no async/await). Keep new code consistent with this pattern unless explicitly converting to async.
- Route prefix is `api/[controller]`, lowercase (e.g., `api/libros`).

## Recommendation
-Use comments sparingly. Only comment on complex code and essential information.
