namespace codewithmena.GymManagementSystem.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int DurationDays { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
