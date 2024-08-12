using Application.Implementation;
using Application.Interface;
using Domain.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ONEINC.EndpointsRegistration
{
    public static class Api
    {
      

        public static void configureApi(this WebApplication app) 
        {
            // All of my Api endpoint mapping 
            app.MapPost(pattern: "/Users/encodeUserTest", EncodeTest);
            app.MapPost(pattern: "/Users/encodeUserTest2", EncodeTest2);
        }

        private static async Task<IResult> EncodeTest([FromBody] EncordRequest request, CancellationToken cancellationToken, IEncoderService encoderService)
        {

            try
            {
                var encodedString = await encoderService.EncodeToBase64(request.input, cancellationToken);
                var charList = encodedString.ToCharArray().ToList();
                return Results.Ok(charList); 
            }
            catch (Exception ex)
            {
                return Results.Ok(ex.Message);
            }

        }

        private static async IAsyncEnumerable<string> EncodeTest2([FromBody] EncordRequest request, CancellationToken cancellationToken, IEncoderService encoderService)
        {

            await foreach (var character in encoderService.EncodeToBase64Async(request.input, cancellationToken))
            {
                yield return character;
            }


        }

    }

}

