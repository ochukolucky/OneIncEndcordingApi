using Application.Implementation;
using Application.Interface;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<IEncodingService, EncodingService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();


// Add CORS services to the container
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()   // Allows any origin
                   .AllowAnyHeader()   // Allows any headers
                   .AllowAnyMethod();  // Allows any HTTP methods
        });
});


var app = builder.Build();

app.MapPost("/encode", async ([FromBody]EncodeRequest request, HttpResponse response, IEncodingService encodingService, CancellationToken cancellationToken) =>
{
    await encodingService.EncodeAsync(request.Input, response, cancellationToken);
});





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS middleware
app.UseCors("AllowAnyOrigin");

app.UseHttpsRedirection();





app.Run();

