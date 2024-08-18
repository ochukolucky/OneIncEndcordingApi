using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IEncodingService
    {
        Task<string> StartEncoding(string input);
        Task StreamEncoding(string encodingId, HttpResponse response, CancellationToken cancellationToken);
        Task<string> CancelEncoding(string encodingId);



    }
}
