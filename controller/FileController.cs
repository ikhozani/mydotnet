using System.Security.Claims;
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

        // using (var memoryStream = new MemoryStream())
        // {
        //     await model.File.CopyToAsync(memoryStream);
        //     var fileRecord = new FileRecord
        //     {
        //         FileName = model.File.FileName,
        //         Content = memoryStream.ToArray(),
        //         ContentType = model.File.ContentType,
        //         UploadedAt = DateTime.UtcNow,
        //         UploadedBy = User.Claims.FirstOrDefault(c => c.Type == "userName")?.Value //User.Identity.Name
                
        //     };

        //     _context.FileRecords.Add(fileRecord);
        //     await _context.SaveChangesAsync();
        // }


        // return Ok("File uploaded successfully.");

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
            // return NotFound(new ApiResponse<object>(404, "error", "File not found", null));
            return NotFound(new {});
        }

        return File(fileRecord.Content, fileRecord.ContentType, fileRecord.FileName);
    }    

}
