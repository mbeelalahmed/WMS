using Cronos;
using GAC.Integration.FileIntegration.Models;
using GAC.Integration.FileIntegration.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GAC.Integration.FileIntegration.Jobs
{
    public class LegacyFilePollingJob : BackgroundService
    {
        private readonly string _folderPath;
        private readonly CronExpression _cron;
        private readonly IFileParserService _parser;
        private readonly IWmsPushService _wmsPusher;
        private readonly int _maxDegreeOfParallelism = 4;
        private DateTime _nextRun;

        public LegacyFilePollingJob(
            IFileParserService parser,
            IWmsPushService wmsPusher,
            IOptions<FileIntegrationOptions> options)
        {
            _parser = parser;
            _wmsPusher = wmsPusher;
            _folderPath = options.Value.InputFolderPath;
            _cron = CronExpression.Parse(options.Value.PollingCronExpression);
            _nextRun = DateTime.UtcNow;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("LegacyFilePollingJob started.");

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} after {timeSpan.TotalSeconds}s due to: {exception.Message}");
                    });

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                if (now < _nextRun)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue; 
                }

                Console.WriteLine($"Polling folder {_folderPath} at {now:O}");

                string[] files;
                try
                {
                    files = Directory.GetFiles(_folderPath, "*.xml");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to list files: {ex.Message}");
                    files = Array.Empty<string>();
                }

                var semaphore = new SemaphoreSlim(_maxDegreeOfParallelism);
                var tasks = files.Select(file => ProcessFileWithSemaphoreAsync(file, semaphore, retryPolicy, stoppingToken));
                await Task.WhenAll(tasks);

                _nextRun = _cron.GetNextOccurrence(now) ?? now.AddMinutes(1);
                Console.WriteLine($"Next run scheduled at {_nextRun:O}");
            }

            Console.WriteLine("LegacyFilePollingJob stopped.");
        }

        private async Task ProcessFileWithSemaphoreAsync(string file, SemaphoreSlim semaphore, IAsyncPolicy retryPolicy, CancellationToken token)
        {
            await semaphore.WaitAsync(token);
            try
            {
                await retryPolicy.ExecuteAsync(() => ProcessFileAsync(file));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to process file {file}: {ex.Message}");
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task ProcessFileAsync(string file)
        {
            Console.WriteLine($"Processing file: {file}");
            var orders = await _parser.ParseFileAsync(file);
            foreach (var order in orders)
            {
                await _wmsPusher.PushToWmsAsync(order);
            }
            var processedPath = file + ".processed";
            //File.Move(file, processedPath);
            Console.WriteLine($"Moved file to: {processedPath}");
        }

    }
}
