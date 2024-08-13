using Application.Interface;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementation
{
    public class EncoderService : IEncoderService
    {
        public async IAsyncEnumerable<char> EncodeAsync(string input, CancellationToken cancellationToken)
        {
            //encript the incoming request to base64
            byte[] textBytes = Encoding.UTF8.GetBytes(input);
            string base64 = Convert.ToBase64String(textBytes);

            //loop each one
            foreach (char c in base64)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                await Task.Delay(new Random().Next(1000, 5001), cancellationToken);
                yield return c;

            }
        }
            public async Task<string> EncodeToBase64(string input, CancellationToken cancellationToken)
        {
            try
			{
                var base64String = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input));
                var encodedString = "";

                foreach (var character in base64String)
                {
                    await Task.Delay(new Random().Next(1000, 5000), cancellationToken);
                    encodedString += character;

                    // Simulate cancellation
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return encodedString;
                    }
                }

                return encodedString;
            }
            catch (Exception)
			{

				throw;
			}
        }

    }
    
}
