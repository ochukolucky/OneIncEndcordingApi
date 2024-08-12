using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IEncoderService
    {
        Task<string> EncodeToBase64(string input, CancellationToken cancellationToken);


        IAsyncEnumerable<string> EncodeToBase64Async(string input, CancellationToken cancellationToken);
        

    }
}
