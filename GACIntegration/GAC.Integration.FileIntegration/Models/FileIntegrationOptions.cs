using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAC.Integration.FileIntegration.Models
{
    public class FileIntegrationOptions
    {
        public string InputFolderPath { get; set; } = string.Empty;
        public string PollingCronExpression { get; set; } = "*/1 * * * *";
        public string WmsBaseUrl { get; set; } = string.Empty;
        public string PurchaseOrdersEndpoint { get; set; } = string.Empty;

    }
}
