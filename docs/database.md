# Database Schema & EF Core Configurations (database.md)

This document details the database specifications, Entity Framework Core 10 mappings, base entities, audit strategies, soft-delete mechanisms, and migration workflows utilized within the Gym Management System.

---

## 🛢️ Database Platform & Connectivity

-   **Database Engine:** Microsoft SQL Server
-   **ORM:** Entity Framework Core 10.0 (SqlServer provider)
-   **Connection String Key:** `DefaultConnection` in `appsettings.json`
-   **Development Config:** Uses standard SQL Express or local instance (`Server=.;Database=GymDB;Trusted_Connection=true;TrustServerCertificate=true;`).

---

## 🏛️ Base Entity Architecture

Every persistent domain entity in the system inherits from a generic abstract class, `BaseEntity<TKey>`, located in the namespace `codewithmena.GymManagementSystem.DAL.Entities.Base`. This ensures standardized primary keys, audit metrics, and logical deletes across all tables.

```
                  ┌─────────────────────────────┐
                  │      IBaseEntity<TKey>      │ (Interface)
                  └──────────────┬──────────────┘
                                 ▲
                                 │
                  ┌──────────────┴──────────────┐
                  │     BaseEntity<TKey>        │ (Abstract Base)
                  └──────────────┬──────────────┘
                                 ▲
                                 │
                   ┌─────────────┴─────────────┐
                   │           Plan            │ (Concrete Table Entity)
                   └───────────────────────────┘
```

### Base Properties Definition:
-   **`Id` (`TKey`):** The unique primary identifier. Usually configured as `int` (mapping to an auto-incrementing `IDENTITY` column in SQL Server) or `Guid`.
-   **`CreatedAt` (`DateTime`):** Record creation timestamp. Auto-populated by the database engine upon execution of an `INSERT` command.
-   **`UpdatedAt` (`DateTime?`):** Audit field tracking modifications. Set manually via repository update methods, or null if the record was never edited.
-   **`IsDeleted` (`bool`):** Logical indicator representing whether a record is soft-deleted. Default value is `false` (0).

---

## 📐 Fluent API Database Mapping

EF Core configurations are modularized and isolated in the `Configurations/` folder rather than bloating the main `GymDbContext` class. Configurations inherit from `BaseEntityConfiguration<TEntity, TKey>`, establishing baseline table standards:

```csharp
public abstract class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // 1. Primary Key Configuration
        builder.HasKey(e => e.Id);

        // 2. Audit Mapping
        builder.Property(e => e.CreatedAt)
               .HasDefaultValueSql("GETDATE()") // Defaults to server date on INSERT
               .IsRequired();

        builder.Property(e => e.UpdatedAt)
               .IsRequired(false);

        // 3. Soft Delete Setup
        builder.Property(e => e.IsDeleted)
               .HasDefaultValue(false)
               .IsRequired();
    }
}
```

---

## 📋 Schema Definition: `Plans` Table

The `Plans` table stores membership subscriptions available for gym members. The schema is configured via `PlanConfiguration.cs`:

| Column Name    | SQL Data Type  | Nullability | Default / Constraint                                      | Description                                  |
| :------------- | :------------- | :---------- | :-------------------------------------------------------- | :------------------------------------------- |
| `Id`           | `int`          | NOT NULL    | `IDENTITY(1,1)` (Primary Key)                             | Unique identifier for the plan               |
| `Name`         | `nvarchar(50)` | NOT NULL    | Max Length: 50                                            | Name of the membership plan                  |
| `Description`  | `nvarchar(200)`| NOT NULL    | Max Length: 200                                           | Details of what the membership grants        |
| `Price`        | `decimal(10,2)`| NOT NULL    | Precision: 10, Scale: 2                                   | Standard subscription cost (EGP)             |
| `IsActive`     | `bit`          | NOT NULL    | -                                                         | Denotes if plan is purchaseable              |
| `DurationDays` | `int`          | NOT NULL    | -                                                         | Length of membership in days                 |
| `CreatedAt`    | `datetime2`    | NOT NULL    | `GETDATE()`                                               | Audit creation date                          |
| `UpdatedAt`    | `datetime2`    | NULL        | -                                                         | Audit modification date                      |
| `IsDeleted`    | `bit`          | NOT NULL    | `0` (False)                                               | Logical soft-delete flag                     |

### Specialized Constraints:
-   **Table Check Constraint (`CHK_Plan_DurationCheck`):**
    The configuration executes raw SQL to bind a check constraint on the `DurationDays` column to prevent illogical values:
    ```csharp
    builder.ToTable(tb =>
    {
        tb.HasCheckConstraint("CHK_Plan_DurationCheck", "[DurationDays] Between 1 and 365");
    });
    ```

---

## 🧹 Soft-Delete Mechanics

Hard `DELETE` statements are prohibited for entities inheriting from `BaseEntity<TKey>`. The logic is self-managed in `BaseRepository.cs`:

### 1. Repository Soft Delete:
Calling `Delete(entity)` marks the record as modified and flags it as deleted, updating the timestamp.
```csharp
public void Delete(TEntity entity)
{
    entity.IsDeleted = true;
    entity.UpdatedAt = DateTime.UtcNow;
    _dbSet.Update(entity);
}
```

### 2. Automatic Read Filtering:
Every read request directed through `GetAllAsync` or `GetByIdAsync` filters out deleted data silently.
```csharp
public async Task<IEnumerable<TEntity>> GetAllAsync()
{
    return await _dbSet
        .Where(e => !e.IsDeleted) // Filters deleted records automatically
        .AsNoTracking()
        .ToListAsync();
}
```

*Note:* If you bypass the repository using EF Core `_context.Set<T>()` or direct SQL, you are required to append `IsDeleted == false` to your search predicates manually.

---

## 🏗️ Managing Migrations

When altering the database model, use C# CLI tools to record changes and update local tables. Run these commands from the repository root.

### Create Migration Command:
Registers changes made to entity models in a new migration class inside the DAL assembly.
```powershell
dotnet ef migrations add <MigrationName> --project codewithmena.GymManagementSystem.DAL --startup-project codewithmena.GymManagementSystem
```

### Update Database Command:
Applies pending migrations on the target server specified by the connection string.
```powershell
dotnet ef database update --project codewithmena.GymManagementSystem.DAL --startup-project codewithmena.GymManagementSystem
```
