using Microsoft.AspNetCore.Components.Forms;

namespace PortfolioConstruct.Services
{
    public class FileService : IFileService
    {
        public async Task<string> SaveImageAsync(IBrowserFile file)
        {
            var dir = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(dir);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
            var fullPath = Path.Combine(dir, fileName);
            await using var fs = new FileStream(fullPath, FileMode.Create);
            await file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(fs);
            return $"/uploads/{fileName}";
        }

        public async Task<string> SaveDocumentAsync(IBrowserFile file)
        {
            var dir = Path.Combine("wwwroot", "documents");
            Directory.CreateDirectory(dir);
            var ext      = Path.GetExtension(file.Name);
            var safeName = Path.GetFileNameWithoutExtension(file.Name)
                .Replace(" ", "_").Replace(",", "").Replace(";", "");
            var fileName = $"{safeName}_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(dir, fileName);
            await using var fs = new FileStream(fullPath, FileMode.Create);
            await file.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024).CopyToAsync(fs);
            return $"/documents/{fileName}|{file.Name}";
        }
    }
}
