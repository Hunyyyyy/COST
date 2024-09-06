namespace COTS1.Models
{
    public class EmailSummary
    {
        public string Id { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Snippet { get; set; }
        public string SentDate { get; set; }
        public List<string> BodyContents { get; set; }
        public List<AttachmentInfo> Attachments { get; set; }
    }
}
