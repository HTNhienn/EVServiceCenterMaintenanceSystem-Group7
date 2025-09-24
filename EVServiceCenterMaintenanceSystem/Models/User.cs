using System;

namespace EVServiceCenterMaintenanceSystem.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string? Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
