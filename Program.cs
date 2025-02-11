using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Scalar.AspNetCore;
using TestAPICreateWithC_.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EmployeeContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDbContext<TodoContext>(options => options.UseSqlite(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
