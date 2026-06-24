# Gym Management System — Agent Manual (AGENTS.md)

Welcome, AI Agent! This document is your comprehensive master manual for the Gym Management System codebase. It is designed to get you fully oriented, enable you to understand the architecture, follow established patterns, run automation tasks, and complete features or bug fixes with zero friction.

---

## 🚀 Quick Tech Stack Reference

- **Language & Runtime:** C# 14, .NET 10.0
- **Framework:** ASP.NET Core 10.0 MVC (Model-View-Controller)
- **Data Access:** Entity Framework Core 10.0 (SQL Server Provider)
- **Database:** Microsoft SQL Server (LocalDB or standard instance)
- **Frontend Assets:** Bootstrap 5.3.x, Bootstrap Icons 1.11.0, jQuery, jQuery Validation / Unobtrusive, Google Fonts (*Baloo Thambi 2* for headers, *Rubik* for body)

---

## 📁 Repository Blueprint

The solution utilizes a clean N-Tier layered architecture divided into three primary projects. Always adhere to these structural boundaries:

```
codewithmena.GymManagementSystem.slnx (Solution File)
│
├── 📂 codewithmena.GymManagementSystem/ (Presentation Layer - PL)
│   ├── 📂 Controllers/          # Handles user requests & redirects (e.g., PlansController)
│   ├── 📂 Models/               # ViewModels and UI-specific models (e.g., ErrorViewModel)
│   ├── 📂 Views/                # Razor Views (.cshtml) grouped by controller directories
│   │   ├── 📂 Shared/           # Common layouts (_Layout.cshtml), partials, and errors
│   │   └── 📂 Plans/            # Views for managing Membership Plans
│   ├── 📂 wwwroot/              # Static files (CSS style.css, JS, images, libraries)
│   ├── 📄 appsettings.json      # App configurations & DB Connection Strings
│   └── 📄 Program.cs            # App entry point, services DI registrations, and HTTP pipeline
│
├── 📂 codewithmena.GymManagementSystem.BLL/ (Business Logic Layer - BLL)
│   # Currently empty of .cs files. Placed as an intermediary layer.
│   # In future iterations, controllers should inject BLL services instead of DAL repositories directly.
│
└── 📂 codewithmena.GymManagementSystem.DAL/ (Data Access Layer - DAL)
    ├── 📂 DbContexts/           # GymDbContext for EF Core
    ├── 📂 Entities/             # Database tables represented as C# POCO classes
    │   └── 📂 Base/             # Generic BaseEntity<TKey> (Id, CreatedAt, UpdatedAt, IsDeleted)
    ├── 📂 Contracts/            # Interfaces for entities and repositories
    │   └── 📂 Repositories/     # Generic and entity-specific repository interfaces
    ├── 📂 Repositories/         # Concrete repository implementations (BaseRepository, PlanRepository)
    ├── 📂 Configurations/       # Fluent API entity schemas and database constraints
    └── 📂 Migrations/           # EF Core Database Migrations
```

---

## 🛠️ Essential Command Cheat Sheet

Always execute these commands from the **workspace root directory** where the solution file resides.

### 1. Build and Run the Application
To build the entire solution and start the MVC web application:
```powershell
dotnet run --project codewithmena.GymManagementSystem
```

### 2. Database Migrations (EF Core)
Since the `DbContext` resides in the **DAL** project and the application settings (Connection Strings) reside in the **PL** project, you **MUST** specify both `--project` and `--startup-project` flags when managing migrations.

*   **Add a new migration:**
    ```powershell
    dotnet ef migrations add <MigrationName> --project codewithmena.GymManagementSystem.DAL --startup-project codewithmena.GymManagementSystem
    ```
*   **Apply migrations to the database (Update Database):**
    ```powershell
    dotnet ef database update --project codewithmena.GymManagementSystem.DAL --startup-project codewithmena.GymManagementSystem
    ```
*   **Remove the last migration (if not applied to DB):**
    ```powershell
    dotnet ef migrations remove --project codewithmena.GymManagementSystem.DAL --startup-project codewithmena.GymManagementSystem
    ```

---

## 🔄 Step-by-Step Feature Implementation Workflow

When asked to implement a new entity or extend an existing feature, follow this precise sequence to maintain the architectural integrity of the repository:

### Step 1: Define the Database Entity
1.  Navigate to `codewithmena.GymManagementSystem.DAL/Entities`.
2.  Create your entity class inheriting from `BaseEntity<TKey>` (e.g., `BaseEntity<int>` or `BaseEntity<Guid>`).
3.  Use C# nullable reference types (`#nullable enable` is project-default) and the `required` modifier for non-nullable database columns.
    *Example:*
    ```csharp
    public class Member : BaseEntity<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; }
    }
    ```

### Step 2: Configure the Database Schema (Fluent API)
1.  Navigate to `codewithmena.GymManagementSystem.DAL/Configurations`.
2.  Create a configuration class inheriting from `BaseEntityConfiguration<TEntity, TKey>`.
3.  Override the `Configure` method. Always call `base.Configure(builder);` first to apply base audit/soft-delete properties.
4.  Specify constraints like max lengths, precision for decimals, required fields, and unique indexes/check constraints.
    *Example:*
    ```csharp
    public class MemberConfiguration : BaseEntityConfiguration<Member, int>
    {
        public override void Configure(EntityTypeBuilder<Member> builder)
        {
            base.Configure(builder); // Applies Id, CreatedAt, UpdatedAt, IsDeleted configuration

            builder.Property(m => m.FirstName).HasMaxLength(50).IsRequired();
            builder.Property(m => m.LastName).HasMaxLength(50).IsRequired();
            builder.Property(m => m.Email).HasMaxLength(100).IsRequired();
            builder.HasIndex(m => m.Email).IsUnique();
        }
    }
    ```

### Step 3: Add DbSet to GymDbContext
1.  Open `codewithmena.GymManagementSystem.DAL/DbContexts/GymDbContext.cs`.
2.  Add a `DbSet<TEntity>` property for your new entity.
    *(Note: Configurations are auto-discovered using `modelBuilder.ApplyConfigurationsFromAssembly` in OnModelCreating, so you do not need to register the configuration manually).*

### Step 4: Create EF Migrations
1.  Add a new migration using the standard multi-project command listed in the cheat sheet.
2.  Apply it to the local SQL Server database using `database update`.

### Step 5: Establish Repository Contracts & Implementations
1.  **Contract (Interface):**
    *   Navigate to `codewithmena.GymManagementSystem.DAL/Contracts/Repositories`.
    *   Create `I<Entity>Repository` inheriting from `IBaseRepository<TEntity, TKey>`.
2.  **Implementation:**
    *   Navigate to `codewithmena.GymManagementSystem.DAL/Repositories`.
    *   Create `<Entity>Repository` inheriting from `BaseRepository<TEntity, TKey>` and implementing `I<Entity>Repository`.
3.  **Dependency Injection Registration:**
    *   Open `codewithmena.GymManagementSystem/Program.cs`.
    *   Register your repository with Scoped lifetime:
        ```csharp
        builder.Services.AddScoped<I{Entity}Repository, {Entity}Repository>();
        ```

### Step 6: Design the Service Layer in BLL (Recommended Refactoring)
*   *Note:* The codebase currently routes PL directly to DAL repositories because BLL is empty.
*   To decouple the layers, define a Service Interface (e.g., `IMemberService`) and its concrete class (`MemberService`) inside `codewithmena.GymManagementSystem.BLL`.
*   Inject repositories into your service, execute business validations, and map Entities to ViewModels/DTOs.
*   Register services in `Program.cs` under the BLL layer.

### Step 7: Build Controllers & Views in PL
1.  Create a controller in `Controllers/` injecting your service/repository.
2.  Create associated Views in `Views/{EntityName}/` adhering to standard naming conventions (`Index.cshtml`, `Details.cshtml`, `Create.cshtml`, `Edit.cshtml`).
3.  Use Bootstrap 5 styling and match the existing theme colors (`--primary-color: #070093`).

---

## 🛡️ Crucial Design Patterns & Guards

### 1. Soft-Delete by Default
- **Never perform hard deletes (`DELETE FROM Table`)** on entities inheriting from `BaseEntity`.
- Calling the repository method `Delete(entity)` automatically sets `IsDeleted = true` and updates `UpdatedAt = DateTime.UtcNow`.
- The `BaseRepository<TEntity, TKey>` methods `GetAllAsync()` and `GetByIdAsync()` automatically filter out records where `IsDeleted == true`.
- *Caution:* If you write custom repository queries using EF's `_dbSet` or raw SQL, you **MUST** append `.Where(e => !e.IsDeleted)` manually.

### 2. Audit Triggers
- `CreatedAt` is configured to default to `GETDATE()` on the SQL Server tier via EF Core Fluent API. Let the DB set this.
- `UpdatedAt` is updated automatically to `DateTime.UtcNow` by the repository when calling `Update(entity)` or `Delete(entity)`.

### 3. Nullability & Validation Rules
- The projects use `<Nullable>enable</Nullable>`. Always respect warnings. Avoid using forgiving operators (`!`) unless mapping from frameworks where guaranteed.
- Use ASP.NET validation attributes in your ViewModels (or Entities if bound directly) and check `ModelState.IsValid` inside controllers before processing actions.

---

## 🔎 Self-Correction & Troubleshooting

- **EF Core commands fail:** Ensure you are in the workspace root directory and are supplying both the `--project` (DAL) and `--startup-project` (PL) switches.
- **Connection Issues:** Verify the database connection string in `codewithmena.GymManagementSystem/appsettings.json`. It defaults to `Server=.;Database=GymDB;Trusted_Connection=true;TrustServerCertificate=true;` which expects a local SQL Server default instance.
- **Static Assets Not Loading:** ASP.NET Core MVC 10.0 uses `.MapStaticAssets()` in `Program.cs` and `.WithStaticAssets()` on standard routes for optimized web assets caching. Make sure stylesheet hrefs use the Razor tilde path resolution, e.g., `~/css/style.css`.
- **Styling Mismatch:** Always use variables and utility classes defined in `wwwroot/css/style.css` rather than hardcoding hexadecimal colors. Use CSS variables such as `var(--primary-color)` or utility classes like `.text-primary-color` and `.bg-primary-color` to preserve visual coherence.
