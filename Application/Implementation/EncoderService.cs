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

        public async IAsyncEnumerable<string> EncodeToBase64Async(string input, CancellationToken cancellationToken)
        {
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
            Random random = new Random();

            foreach (char c in encoded)
            {
                await Task.Delay(random.Next(1000, 5000), cancellationToken);
                yield return c.ToString();

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
            }
        }


    }
    
}
