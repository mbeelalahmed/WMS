using GAC.Integration.FileIntegration.Models;

namespace GAC.Integration.FileIntegration.Services
{
    public interface IFileParserService
    {
        Task<IEnumerable<PurchaseOrder>> ParseFileAsync(string filePath);
    }
}
