# Backend Coding Standards & C# Rules (backend-rules.md)

This document establishes the official backend development guidelines and architectural rules for the .NET 10.0 Gym Management System. Developers and AI agents must strictly adhere to these guidelines to ensure code quality, type safety, performance, and clear structural separation.

---

## 💻 Language Standards: C# 14 & .NET 10.0

The application leverages features of modern C# and the latest .NET 10 runtime. Ensure new backend code conforms to the following:

-   **Explicit Nullability:** The project has `<Nullable>enable</Nullable>` set. You must design schemas and models to explicitly address nullability. Avoid the null-forgiving operator (`!`) unless mapping from frameworks where safety is compile-time guaranteed.
-   **Required Properties:** Use the modern `required` modifier for non-nullable properties inside entities, view models, and DTOs to enforce object initialization:
    ```csharp
    public required string Name { get; set; }
    ```
-   **File-Scoped Namespaces:** Always use file-scoped namespaces to reduce indentation levels:
    ```csharp
    namespace codewithmena.GymManagementSystem.DAL.Repositories;
    ```
-   **Implicit Usings:** Utilize .NET's implicit usings feature. Do not crowd the top of your files with redundant `using System;` or `using System.Collections.Generic;` namespaces.

---

## 📦 Repository Pattern & Persistence Rules

Direct database interactions utilizing EF Core `DbContext` inside Presentation Layer controllers are **strictly forbidden**. All operations must run through decoupled repository abstractions.

### 1. Asynchronous I/O Execution
Database transactions and queries are highly resource-intensive. You must run all database operations asynchronously.
-   Repository method signatures must return a `Task` or `Task<T>`.
-   Method names must be postfixed with the `Async` suffix (e.g., `GetAllAsync`, `GetByIdAsync`, `SaveChangesAsync`).
-   Always await asynchronous tasks rather than calling `.Result` or `.Wait()`, which can block threads and cause deadlocks.

### 2. Read-Only Query Optimization
When executing data queries intended only for display (read-only views), configure Entity Framework to bypass tracking overhead using `.AsNoTracking()`. This drastically optimizes memory allocation and performance.
```csharp
public async Task<IEnumerable<TEntity>> GetAllAsync()
{
    return await _dbSet
        .Where(e => !e.IsDeleted)
        .AsNoTracking() // Performance Optimization
        .ToListAsync();
}
```

### 3. Update & Soft-Delete Lifecycle
To safely modify or logically delete a record:
1.  **Retrieve:** Fetch the entity from the database by its primary key using `GetByIdAsync(id)`.
2.  **Validate:** Ensure the retrieved entity is not null before continuing.
3.  **Execute:** Modify the values (or call `Delete(entity)` to apply soft deletion).
4.  **Mark:** Call the repository's `.Update(entity)` method to flag changes in the tracker (which also updates `UpdatedAt = DateTime.UtcNow` under the hood).
5.  **Persist:** Invoke `await SaveChangesAsync()` to commit the transaction to the database.

---

## 📡 Dependency Injection (DI) & Lifetimes

All backend components must rely exclusively on **Constructor Injection** to receive external dependencies. Never use service location anti-patterns (e.g., accessing DI container inside classes manually).

### Service Lifetimes inside Program.cs:
-   **DbContext (`GymDbContext`):** Scoped lifetime. Registered automatically via `builder.Services.AddDbContext<GymDbContext>(...)`.
-   **Repositories (`IPlanRepository`):** Scoped lifetime. Instantiated once per HTTP request:
    ```csharp
    builder.Services.AddScoped<IPlanRepository, PlanRepository>();
    ```
-   **Services (Future BLL Services):** Scoped lifetime. Group and document registrations by layer within `Program.cs`.

---

## 🛡️ Business Logic Layer (BLL) Isolation Boundaries

Presently, the Presentation Layer (PL) directly injects DAL repository abstractions because the BLL project is empty of concrete code. To maintain long-term architecture:

-   **Thin Controllers:** Controllers in the PL must be kept extremely lightweight. A controller should only parse incoming HTTP requests, route workflows, validate the UI model states, and return Views or API redirects.
-   **No Domain Logic in PL/DAL:** Domain logic, such as validating membership purchase constraints, calculating trainer scheduling overlaps, applying promotions, or checking subscription expiration dates, **must never** reside inside PL Controllers or DAL Repositories.
-   **BLL Service Implementation:** When adding complex workflows, implement them in dedicated service classes inside `codewithmena.GymManagementSystem.BLL`.
    *   BLL Services should inject DAL Repositories to interact with the database.
    *   PL Controllers will then inject BLL Services, achieving clean segregation.

---

## 🚨 Input Validation & Error Handling

-   **Model State Verification:** Prior to executing backend actions from form submissions, PL Controllers must verify UI payload integrity:
    ```csharp
    [HttpPost]
    public async Task<IActionResult> Create(PlanViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Redirects user to fix input errors with visual cues
        }
        // Proceed with backend execution...
    }
    ```
-   **Global Exception Handling:** Keep controllers clean of individual try-catch blocks for database standard errors. Rely on the global ASP.NET Core MVC middleware (`app.UseExceptionHandler("/Home/Error")`) to capture, log, and redirect users to standard user-friendly error pages securely.
