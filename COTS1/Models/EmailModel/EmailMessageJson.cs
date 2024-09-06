namespace COTS1.Models.EmailModel
{
    public class EmailMessageJson
    {
        public string Id { get; set; }            // Id của email
        public string ThreadId { get; set; }      // Id của chuỗi email
        public List<string> LabelIds { get; set; } // Các label của email
        public string Snippet { get; set; }       // Đoạn trích của nội dung email
        public string HistoryId { get; set; }     // Id lịch sử thay đổi của email
        public string InternalDate { get; set; }  // Ngày gửi (thời gian Unix timestamp)
        public MessagePart Payload { get; set; }  // Nội dung của email
        public int SizeEstimate { get; set; }     // Kích thước ước tính của email
        public string Raw { get; set; }
    }
    public class MessagePart
    {
        public string PartId { get; set; }        // Id của phần MIME
        public string MimeType { get; set; }      // Loại MIME (ví dụ: text/plain, text/html)
        public string Filename { get; set; }      // Tên file nếu là file đính kèm
        public List<Header> Headers { get; set; } // Các tiêu đề của phần MIME
        public MessagePartBody Body { get; set; } // Nội dung của phần MIME
        public List<MessagePart> Parts { get; set; } // Các phần con của phần MIME (nếu có)
    }

    // Đại diện cho một Header của phần MIME
    public class Header
    {
        public string Name { get; set; }          // Tên của header (ví dụ: From, To, Subject)
        public string Value { get; set; }         // Giá trị của header (ví dụ: địa chỉ email người gửi)
    }

    // Đại diện cho nội dung của một phần MIME
    public class MessagePartBody
    {
        public string AttachmentId { get; set; }  // Id của tệp đính kèm (nếu có)
        public int Size { get; set; }             // Kích thước của phần MIME
        public string Data { get; set; }          // Dữ liệu của phần MIME (base64 URL-safe)
    }
}
