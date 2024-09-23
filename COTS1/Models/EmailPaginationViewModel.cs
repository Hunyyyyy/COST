namespace COTS1.Models
{
    public class EmailPaginationViewModel
    {
        public List<EmailSummary> Emails { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}