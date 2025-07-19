using GAC.Integration.FileIntegration;
using GAC.Integration.FileIntegration.Jobs;
using GAC.Integration.FileIntegration.Models;
using GAC.Integration.FileIntegration.Services;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<FileIntegrationOptions>(
    builder.Configuration.GetSection("FileIntegration"));

builder.Services.AddSingleton<IFileParserService, XmlFileParserService>();
builder.Services.AddHttpClient<IWmsPushService, WmsPushService>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<FileIntegrationOptions>>().Value;
    client.BaseAddress = new Uri(settings.WmsBaseUrl);
});

builder.Services.AddHostedService<LegacyFilePollingJob>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
