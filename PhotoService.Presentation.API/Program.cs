using Azure.Storage.Blobs;
using PhotoService.Application;
using PhotoService.Infrastructure;
using PhotoService.Presentation.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);




var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapImageEndpoints();





app.Run();


