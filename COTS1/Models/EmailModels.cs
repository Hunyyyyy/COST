namespace COTS1.Models
{
    public class EmailList
    {
        public List<EmailModels> Messages { get; set; }
    }

    public class EmailModels
    {
        public string Id { get; set; }
        public string ThreadId { get; set; }
        public List<string> LabelIds { get; set; }
        public string Snippet { get; set; }
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        public string MimeType { get; set; }
        public List<Part> Parts { get; set; }
        public List<HeaderDetail> Headers { get; set; } // Sửa đây
    }

    public class Part
    {
        public string MimeType { get; set; }
        public string Filename { get; set; }
        public Body Body { get; set; }
        public List<Part> Parts { get; set; }
    }

    public class Body
    {
        public string Data { get; set; }
    }

    public class HeaderDetail
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
