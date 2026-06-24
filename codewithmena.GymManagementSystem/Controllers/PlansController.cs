using codewithmena.GymManagementSystem.DAL.Contracts.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace codewithmena.GymManagementSystem.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanRepository _planRepository;

        public PlansController(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<IActionResult> Index()
        {
            var plans = await _planRepository.GetAllAsync();
            return View(plans);
        }

        // baseUrl/Plans/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan != null)
            {
                plan.IsActive = !plan.IsActive;
                _planRepository.Update(plan);
                await _planRepository.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

