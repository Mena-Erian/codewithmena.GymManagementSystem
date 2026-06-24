using codewithmena.GymManagementSystem.DAL.Contracts.Repositories.Base;
using codewithmena.GymManagementSystem.DAL.DbContexts;
using codewithmena.GymManagementSystem.DAL.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace codewithmena.GymManagementSystem.DAL.Repositories.Base
{
    public class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected readonly GymDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(GymDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Returns all non-deleted records.
        /// </summary>
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Returns a single non-deleted record by its primary key, or null if not found.
        /// </summary>
        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet
                .Where(e => e.Id.Equals(id) && !e.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Adds a new entity to the context (not yet saved to DB).
        /// </summary>
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Marks entity as modified (not yet saved to DB).
        /// </summary>
        public void Update(TEntity entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Performs a soft delete — sets IsDeleted = true (not yet saved to DB).
        /// </summary>
        public void Delete(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Persists all pending changes to the database.
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
