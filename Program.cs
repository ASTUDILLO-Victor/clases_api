using Microsoft.EntityFrameworkCore;
using MiApi.Data;
using MiApi.Model;
using MiApi.Repositories;
using MiApi.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlite("Data Source=libros.db"));

// Agregar estas dos líneas
builder.Services.AddScoped<LibroRepository>();
builder.Services.AddScoped<LibroService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();