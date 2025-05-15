using Microsoft.AspNetCore.Http;

namespace ViberLounge.Infrastructure.Services;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file);
    void DeleteFile(string fileUrl);
}
