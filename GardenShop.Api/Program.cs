using GardenShop.Application;
using GardenShop.Infrastructure;
using GardenShop.Infrastructure.Persistence;
using GardenShop.Infrastructure.Seed;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
	o.SwaggerDoc("v1", new OpenApiInfo { Title = "GardenShop API", Version = "v1" }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI();
}

app.MapControllers();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

