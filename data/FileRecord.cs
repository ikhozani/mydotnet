namespace FileUploadSvc.data;

public class FileRecord
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public byte[] Content { get; set; }
    public string ContentType { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; }
}
    
