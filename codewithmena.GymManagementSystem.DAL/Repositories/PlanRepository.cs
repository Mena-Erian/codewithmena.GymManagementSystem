using codewithmena.GymManagementSystem.DAL.Contracts.Repositories;
using codewithmena.GymManagementSystem.DAL.DbContexts;
using codewithmena.GymManagementSystem.DAL.Entities;
using codewithmena.GymManagementSystem.DAL.Repositories.Base;

namespace codewithmena.GymManagementSystem.DAL.Repositories
{
    public class PlanRepository : BaseRepository<Plan, int>, IPlanRepository
    {
        public PlanRepository(GymDbContext context) : base(context)
        {
        }

        // Plan-specific repository methods can be added here
    }
}
