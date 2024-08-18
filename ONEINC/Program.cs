using Application.Implementation;
using Application.Interface;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<IEncodingService, EncodingService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(); 

var configurationOptions = ConfigurationOptions.Parse("localhost:6379");
configurationOptions.AbortOnConnectFail = false;

var redis = ConnectionMultiplexer.Connect(configurationOptions);



// Register the Redis connection multiplexer as a singleton
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configurationOptions = ConfigurationOptions.Parse(builder.Configuration.GetSection("Redis")["ConnectionString"]);


    return ConnectionMultiplexer.Connect(configurationOptions);
});





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

//endpoint
app.MapPost("/encode", async ([FromBody] EncodingRequest request, IEncodingService encodingService) =>
{
    await encodingService.StartEncoding(request.Input);
});

app.MapGet("/encode/{encodingId}", async (string encodingId, HttpResponse response, IEncodingService encodingService, CancellationToken cancellationToken) =>
{
    await encodingService.StreamEncoding(encodingId, response, cancellationToken);
});

app.MapPost("/encode/{encodingId}/cancel", async (string encodingId, IEncodingService encodingService) =>
{
    await encodingService.CancelEncoding(encodingId);
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

