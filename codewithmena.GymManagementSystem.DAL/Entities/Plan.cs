using codewithmena.GymManagementSystem.DAL.Contracts;
using codewithmena.GymManagementSystem.DAL.Entities.Base;

namespace codewithmena.GymManagementSystem.DAL.Entities
{
    public class Plan : BaseEntity<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int DurationDays { get; set; }

    }
}
