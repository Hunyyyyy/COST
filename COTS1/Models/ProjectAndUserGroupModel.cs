namespace COTS1.Models
{
    public class ProjectAndUserGroupModel
    {
        public ProjectModel Project { get; set; }
        public List<ProjectUserModel>? ProjectUserModel { get; set; }
    }
}