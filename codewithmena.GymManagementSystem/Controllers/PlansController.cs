using codewithmena.GymManagementSystem.Database.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace codewithmena.GymManagementSystem.Controllers
{
    public class PlansController : Controller
    {
        private readonly GymDbContext gymDbContext;

        public PlansController()
        {
         this.gymDbContext = new GymDbContext();
        }
        public async Task<IActionResult> Index()

        {
            var plans = await gymDbContext.Plans.ToListAsync();

            return View(plans);
        }

        // baseUrl/Plans/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var plan = await gymDbContext.Plans.FindAsync(id);
            if (plan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }


        [HttpPost]
        public async Task<IActionResult> Activate(int id)
        {
            var plan = await gymDbContext.Plans.FindAsync(id);
            if (plan != null)
            {
                plan.IsActive = !plan.IsActive;
                await gymDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
