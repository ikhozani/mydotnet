namespace FileUploadSvc.dto;

public class FileRecordDto
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; }
}