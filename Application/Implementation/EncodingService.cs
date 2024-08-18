using Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementation
{
    public class EncodingService : IEncodingService
    {
        private readonly ILogger<EncodingService> _logger;
        private readonly IConnectionMultiplexer _redis;

        public EncodingService(ILogger<EncodingService> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
        }

        public async Task<string> StartEncoding(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "Input cannot be empty";
            }

            var encodingId = Guid.NewGuid().ToString();
            var db = _redis.GetDatabase();

            try
            {
                // Store the input and status in Redis with a 10-minute expiration
                var tasks = new[]
                  {
            db.StringSetAsync($"encoding:{encodingId}:input", input, TimeSpan.FromMinutes(10)),
            db.StringSetAsync($"encoding:{encodingId}:status", "pending", TimeSpan.FromMinutes(10))
        };

                await Task.WhenAll(tasks);

                // Return the encoding ID to the client
                return encodingId;
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogError(ex, "Failed to connect to Redis while starting encoding process.");
                return "503, Unable to connect to the encoding service. Please try again later.";
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "Redis operation timed out while starting encoding process.");
                return "504, The encoding service is currently slow to respond. Please try again later.";
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "An error occurred with Redis while starting encoding process.");
                return "500, An error occurred while preparing the encoding process. Please try again.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while starting the encoding process.");
                return "500, An unexpected error occurred. Please try again later.";
            }

        }

        public async Task StreamEncoding(string encodingId, HttpResponse response, CancellationToken cancellationToken)
        {
            response.Headers.Add("Content-Type", "text/event-stream");
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Connection", "keep-alive");

            var db = _redis.GetDatabase();
            var input = await db.StringGetAsync($"encoding:{encodingId}:input");

            if (!input.HasValue)
            {
                await response.WriteAsync("data: Encoding not found or expired\n\n");
                return;
            }
            try
            {
                await db.StringSetAsync($"encoding:{encodingId}:status", "processing");

                _logger.LogInformation($"About to encode incoming request at {DateTime.UtcNow}");
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
                _logger.LogInformation($"Encoded incoming request at {DateTime.UtcNow}");

                var random = new Random();

                foreach (var character in base64)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        await response.WriteAsync($"data: Encoding cancelled\n\n");
                        await db.StringSetAsync($"encoding:{encodingId}:status", "cancelled");
                        return;
                    }

                    await Task.Delay(random.Next(1000, 5001), cancellationToken);
                    await response.WriteAsync($"data: {character}\n\n");
                    await response.Body.FlushAsync(cancellationToken);
                }

                await db.StringSetAsync($"encoding:{encodingId}:status", "completed");
            }
            catch (OperationCanceledException)
            {
                await response.WriteAsync($"data: Encoding cancelled\n\n");
                await db.StringSetAsync($"encoding:{encodingId}:status", "cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while encoding the input.");
                await response.WriteAsync($"data: An error occurred while encoding\n\n");
                await db.StringSetAsync($"encoding:{encodingId}:status", "error");
            }
            finally
            {
                // Clean up the Redis entries
                await db.KeyDeleteAsync(new RedisKey[] {
                $"encoding:{encodingId}:input",
                $"encoding:{encodingId}:status"
            });
            }
        }
        public async Task<string> CancelEncoding(string encodingId)
        {
            var db = _redis.GetDatabase();
            var status = await db.StringGetAsync($"encoding:{encodingId}:status");

            if (!status.HasValue || status != "processing")
            {
                return "No active encoding process found with the given ID.";
            }

            await db.StringSetAsync($"encoding:{encodingId}:status", "cancelling");
            return "Cancellation request received.";
        }

    }
}