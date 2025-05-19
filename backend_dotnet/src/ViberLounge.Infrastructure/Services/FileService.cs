using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace ViberLounge.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _productImagesPath = "images/product";
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly long _maxFileSize = 5 * 1024 * 1024;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            // if (string.IsNullOrEmpty(_environment.WebRootPath))
            // {
            //     // Use ContentRootPath como base se WebRootPath for nulo
            //     string contentRoot = _environment.ContentRootPath;
            //     string webRootPath = Path.Combine(contentRoot, "wwwroot");

            //     // Criar diretório wwwroot se não existir
            //     if (!Directory.Exists(webRootPath))
            //         Directory.CreateDirectory(webRootPath);

            //     // Hack para definir WebRootPath dinamicamente
            //     var webRootField = _environment.GetType().GetField("_webRootPath",
            //         System.Reflection.BindingFlags.Instance |
            //         System.Reflection.BindingFlags.NonPublic);

            //     if (webRootField != null)
            //         webRootField.SetValue(_environment, webRootPath);
            //     else
            //         throw new InvalidOperationException("Não foi possível definir WebRootPath. Configure manualmente em Program.cs");
            // }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new InvalidOperationException($"Tipo de arquivo não permitido. Extensões permitidas: {string.Join(", ", _allowedExtensions)}");

            if (file.Length > _maxFileSize)
                throw new InvalidOperationException($"Arquivo muito grande. Tamanho máximo permitido: {_maxFileSize / (1024 * 1024)}MB");

            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            
            string uploadsFolder = Path.Combine(_environment.WebRootPath, _productImagesPath);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            
            string filePath = Path.Combine(uploadsFolder, fileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            
            return $"/{_productImagesPath}/{fileName}";
        }

        public bool DeleteFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return false;

            try
            {
                string relativePath = fileUrl.TrimStart('/');
                string fullPath = Path.Combine(_environment.WebRootPath, relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir arquivo: {ex.Message}");
                return false;
            }
        }
    }
}