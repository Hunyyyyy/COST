namespace COTS1.Models
{
    public class EmailListResponse
    {
        public List<EmailMessage> Messages { get; set; }
        public int ResultSizeEstimate { get; set; }
    }

    public class EmailMessage
    {
        public string Id { get; set; }
        public string ThreadId { get; set; }
    }
}
