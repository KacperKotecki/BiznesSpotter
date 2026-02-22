# BiznesSpotter AI Agent Instructions

## Project Overview
BiznesSpotter is an ASP.NET Core 8.0 MVC application that analyzes business potential by combining Google Places data with demographic statistics from the Polish GUS (Statistics Poland) API.

## Architecture & Data Flow
- **Architecture Style**: N-Tier (Layered) Architecture. Keep clear boundaries between Presentation (Controllers/Views), Business Logic (Services), and Data Access/External Integrations.
- **Controllers**: `HomeController` is the primary entry point. It should be "thin" - delegating complex logic to services and mapping results to ViewModels (e.g., `SearchMapViewModel`).
- **Core Service**: `BusinessAnalysisService` orchestrates the main business logic. It concurrently fetches data from external APIs to improve performance.
- **External Integrations**:
  - `GooglePlacesService`: Handles Geocoding and Places API calls.
  - `GusService`: Integrates with `bdl.stat.gov.pl` to fetch population and demographic data.
- **Models**: Separated into `Domain` (core business logic), `ViewModels` (UI representation), and API-specific models (`Gus`, `GooglePlaces`).

## Coding Conventions & Patterns
- **Dependency Injection (DI)**: Strictly use DI. Inject interfaces (e.g., `IGusService`) rather than concrete implementations to ensure loose coupling and testability.
- **Options Pattern**: Use strongly-typed classes (e.g., `GoogleMapsOptions`) for configuration instead of reading directly from `IConfiguration`.
- **Result Pattern**: Prefer returning a `Result<T>` object from business services to handle success/failure states explicitly, rather than throwing exceptions for expected business errors or returning nulls.
- **Repository & Unit of Work Patterns**: Use repositories to abstract data access logic away from business services. Combine this with the Unit of Work pattern to manage transactions and ensure that multiple repository operations are committed to the database as a single atomic operation, keeping EF Core `DbContext` usage isolated.
- **SOLID Principles**: Adhere to SOLID. Specifically, ensure Single Responsibility (classes do one thing) and Dependency Inversion (depend on abstractions).
- **DRY & KISS**: Don't Repeat Yourself (extract common logic) and Keep It Simple, Stupid (avoid over-engineering simple CRUD operations).
- **Asynchronous Programming**: Always use `async/await`. Prefer concurrent execution (`Task.WhenAll`) when calling multiple independent external APIs.
- **JSON Serialization**: Use `System.Text.Json` for parsing external API responses.
- **Nullability**: Nullable reference types are enabled (`<Nullable>enable</Nullable>`). Use `?` appropriately and handle nulls gracefully.
- **Logging**: Inject `ILogger<T>` and log warnings/errors for failed API calls or invalid parameters.

## Developer Workflows
- **Run/Build**: Standard `dotnet build` and `dotnet run` from the `BiznesSpoter.Web` directory.
- **Database**: Uses SQLite (`app.db`) with EF Core. 
  - Add migration: `dotnet ef migrations add <MigrationName> --project BiznesSpoter.Web`
  - Update DB: `dotnet ef database update --project BiznesSpoter.Web`
- **Secrets**: Do not hardcode API keys. Use `dotnet user-secrets` or `appsettings.Development.json` for `GoogleMaps:ApiKey`.
