namespace COTS1.Models
{
    public class MailViewModel
    {
        public List<EmailSummary> ReceivedEmails { get; set; }
        public List<EmailSummary> SentEmails { get; set; }
        public List<EmailSummary> TaskEmails { get; set; }
    }
}
