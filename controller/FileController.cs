using System.Security.Claims;
using System.Security.Cryptography;
using FileUploadSvc.data;
using FileUploadSvc.dto;
using FileUploadSvc.model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileUploadSvc.controller;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public FileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] FileUploadModel model)
    {
        if (model == null || model.File == null || model.File.Length == 0)
        {
            return BadRequest(new { description = "No file was uploaded." });
        }


        try
        {
            // Handle file upload logic
            var file = model.File;
            var fileName = Path.GetFileName(file.FileName);
            var contentType = file.ContentType;

            // Simpan file ke database atau sistem penyimpanan
            var fileRecord = new FileRecord
            {
                FileName = fileName,
                ContentType = contentType,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = User.Claims.FirstOrDefault(c => c.Type == "userName")?.Value //User.Identity.Name
            };

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileRecord.Content = memoryStream.ToArray();
            }

            _context.FileRecords.Add(fileRecord);
            await _context.SaveChangesAsync();

            // Berhasil menyimpan file, kirim respons
            return Ok(new { fileId = fileRecord.Id, fileName = fileRecord.FileName });
        }
        catch (Exception ex)
        {
            // Gagal menyimpan file, kirim respons error
            return StatusCode(500, new { description = "Failed to upload file"});
        }

    }


    private void DeleteDirectory(string targetDir)
    {
        string[] files = Directory.GetFiles(targetDir);
        string[] dirs = Directory.GetDirectories(targetDir);

        foreach (string file in files)
        {
            System.IO.File.SetAttributes(file, FileAttributes.Normal);
            System.IO.File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(targetDir, false);
    }



    [HttpPost("upload-chunk")]
    public async Task<IActionResult> UploadFileChunk([FromForm] FileChunkDto model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new ApiResponse<object>(400, "error", "Invalid model state", errors));
        }

        try
        {
            var chunkFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", model.FileName);
            Directory.CreateDirectory(chunkFolder);

            var chunkPath = Path.Combine(chunkFolder, $"{model.ChunkNumber}.chunk");
            using (var fileStream = new FileStream(chunkPath, FileMode.Create))
            {
                await model.Chunk.CopyToAsync(fileStream);
            }

            if (model.ChunkNumber == model.TotalChunks)
            {
                var filePath = Path.Combine(chunkFolder, model.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    for (int i = 1; i <= model.TotalChunks; i++)
                    {
                        var chunkFile = Path.Combine(chunkFolder, $"{i}.chunk");
                        using (var chunkFileStream = new FileStream(chunkFile, FileMode.Open))
                        {
                            await chunkFileStream.CopyToAsync(fileStream);
                        }
                        System.IO.File.Delete(chunkFile); // Hapus chunk setelah digabung
                    }
                }

                // Simpan file ke database atau sistem penyimpanan setelah penggabungan
                var fileRecord = new FileRecord
                {
                    FileName = model.FileName,
                    ContentType = model.Chunk.ContentType,
                    UploadedAt = DateTime.UtcNow,
                    UploadedBy = User.Claims.FirstOrDefault(c => c.Type == "userName")?.Value,
                    Content = System.IO.File.ReadAllBytes(filePath)
                };

                _context.FileRecords.Add(fileRecord);
                await _context.SaveChangesAsync();

                // Hapus folder chunk setelah selesai
                DeleteDirectory(chunkFolder);

                return Ok(new { fileId = fileRecord.Id, fileName = fileRecord.FileName, status = "complete", 
                    message = "File uploaded completely and successfully"});
            }

            return Ok(new { fileName = model.FileName, chunkNo = model.ChunkNumber, status = "partial", message = "Chunk uploaded successfully"});
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>(500, "error", "Failed to upload chunk", null));
        }
    }



    [HttpGet("list")]
    public async Task<IActionResult> GetFiles(int pageNumber = 1, int pageSize = 10)
    {
        var totalFiles = await _context.FileRecords.CountAsync();
        var totalPages = (int)Math.Ceiling(totalFiles / (double)pageSize);

        var files = await _context.FileRecords
            .OrderBy(f => f.UploadedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new FileRecordDto
            {
                Id = f.Id,
                FileName = f.FileName,
                ContentType = f.ContentType,
                UploadedAt = f.UploadedAt,
                UploadedBy = f.UploadedBy
            })
            .ToListAsync();

        var response = new PagedResponse<FileRecordDto>
        {
            TotalCount = totalFiles,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            Items = files
        };

        return Ok(response);
    }

    
    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var fileRecord = await _context.FileRecords.FindAsync(id);
        if (fileRecord == null)
        {
            return NotFound(new {});
        }

        var stream = new MemoryStream(fileRecord.Content);
        return new FileStreamResult(stream, fileRecord.ContentType)
        {
            FileDownloadName = fileRecord.FileName
        };
    }



}
