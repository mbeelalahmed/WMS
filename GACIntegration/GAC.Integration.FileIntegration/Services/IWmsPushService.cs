using GAC.Integration.FileIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAC.Integration.FileIntegration.Services
{
    public interface IWmsPushService
    {
        Task PushToWmsAsync(PurchaseOrder dto);
    }
}
