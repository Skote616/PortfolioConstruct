using Microsoft.AspNetCore.Components.Forms;

namespace PortfolioConstruct.Services
{
    /// <summary>
    /// Сервис работы с файлами — отвечает только за сохранение файлов на диск.
    /// Вынесен отдельно чтобы легко заменить на облачное хранилище в будущем.
    /// </summary>
    public interface IFileService
    {
        Task<string> SaveImageAsync(IBrowserFile file);
        Task<string> SaveDocumentAsync(IBrowserFile file);
    }
}
