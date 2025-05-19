using Microsoft.AspNetCore.Http;

namespace ViberLounge.Infrastructure.Services;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file);
    bool DeleteFile(string fileUrl);
}
