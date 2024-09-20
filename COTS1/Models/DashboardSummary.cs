namespace COTS1.Models
{
    public class DashboardSummary
    {
        public int TotalParticipatedProjects { get; set; }
        public int TotalApprovedTasks { get; set; }
        public int TotalReceivedTasks { get; set; }
        public int PendingApprovalTasks { get; set; }
        public int isWorking { get; set; }
        public int refusedTasks { get; set; }
        public List<ProjectProgressInfo>? projectProgressList { get; set; }
    }
    public class ProjectProgressInfo
    {
        public string ProjectName { get; set; }  // Tên dự án
        public int Progress { get; set; }        // Tiến độ dự án
    }
}
