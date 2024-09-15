using COTS1.Data;

namespace COTS1.Models
{
    public class ProjectModel
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Status { get; set; }

        public int? ManagerId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual User? Manager { get; set; }
        public List<ProjectUserModel> Users { get; set; }
        public int ProjectProgress { get; set; }
    }
}
