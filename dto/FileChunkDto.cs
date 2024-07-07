using System.ComponentModel.DataAnnotations;

namespace FileUploadSvc.dto;

public class FileChunkDto
{
    [Required]
    public IFormFile Chunk { get; set; }
    [Required]
    public string FileName { get; set; }
    [Required]
    public int ChunkNumber { get; set; }
    [Required]
    public int TotalChunks { get; set; }
}
