using GAC.Integration.FileIntegration.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GAC.Integration.FileIntegration.Services
{
    public class WmsPushService : IWmsPushService
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;

        public WmsPushService(HttpClient httpClient, IOptions<FileIntegrationOptions> options)
        {
            _httpClient = httpClient;
            _endpoint = options.Value.PurchaseOrdersEndpoint;
        }

        public async Task PushToWmsAsync(PurchaseOrder dto)
        {
            var response = await _httpClient.PostAsJsonAsync(_endpoint, dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
