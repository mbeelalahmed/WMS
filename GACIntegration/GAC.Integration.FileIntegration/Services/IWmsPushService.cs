using GAC.Integration.FileIntegration.Models;

namespace GAC.Integration.FileIntegration.Services
{
    public interface IWmsPushService
    {
        Task PushToWmsAsync(PurchaseOrder dto);
    }
}
