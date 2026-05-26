using Azure.Storage.Blobs;
using PhotoService.Application;
using PhotoService.Presentation.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();

builder.Services.AddSingleton(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("AzureBlobStorage")
        ?? throw new InvalidOperationException("AzureBlobStorage conntectionString is missing");

    return new BlobServiceClient(connectionString);
});



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapImageEndpoints();





app.Run();


