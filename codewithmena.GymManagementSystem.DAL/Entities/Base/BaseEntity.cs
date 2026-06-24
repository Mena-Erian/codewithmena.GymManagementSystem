using codewithmena.GymManagementSystem.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace codewithmena.GymManagementSystem.DAL.Entities.Base
{
    public abstract class BaseEntity<TKey> : IBaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        public required TKey Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
