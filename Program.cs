using Microsoft.EntityFrameworkCore;
using CadeirasAPI.Models;
using CadeirasAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DBConnection>();

var app = builder.Build();

// Configura o HTTP ao Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
