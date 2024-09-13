using COTS1.Data;

namespace COTS1.Models
{
    public class ProjectUserModel
    {
        public int ProjectUserId { get; set; }

        public int ProjectId { get; set; }
        public string? Email { get; set; }
        public int UserId { get; set; }

        public string Role { get; set; } = null!;

        public virtual Project Project { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
