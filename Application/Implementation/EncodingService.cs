using Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        public EncodingService(ILogger<EncodingService> logger)
        {
            _logger = logger;
        }
        public async Task EncodeAsync(string input, HttpResponse response, CancellationToken cancellationToken)
        {
            try
            {
                // Check if incoming request is null
                if (string.IsNullOrEmpty(input))
                {
                    response.StatusCode = 400;
                    await response.WriteAsync("Input cannot be empty");
                    return;
                }

                response.Headers.Add("Content-Type", "text/event-stream");
                response.Headers.Add("Cache-Control", "no-cache");
                response.Headers.Add("Connection", "keep-alive");

                //about to encript user input
                _logger.LogInformation($"About to endcord incoming request at {DateTime.UtcNow}");
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
                //encripted user input
                _logger.LogInformation($"endcorded incoming request at {DateTime.UtcNow}");
                var random = new Random();

                foreach (var character in base64)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    await Task.Delay(random.Next(1000, 5001), cancellationToken);
                    await response.WriteAsync($"data: {character}\n\n");
                    await response.Body.FlushAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while encoding the input.");
                response.StatusCode = 500;
                await response.WriteAsync("An internal server error occurred.");
            }
        }
    }
}
