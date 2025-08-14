using Microsoft.EntityFrameworkCore;
using rian_p01_back.src.Data;
using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Data.Repositories.Implementations;
using rian_p01_back.src.Services.Interfaces;
using rian_p01_back.src.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    connectionString = connectionString
        .Replace("${DB_SERVER}", Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost")
        .Replace("${DB_PORT}", Environment.GetEnvironmentVariable("DB_PORT") ?? "1433")
        .Replace("${DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME") ?? "starcatcherDB")
        .Replace("${DB_USER}", Environment.GetEnvironmentVariable("DB_USER") ?? "sa")
        .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "StrongPassword123!");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IConsorcioRepository, ConsorcioRepository>();
builder.Services.AddScoped<ICotasRepository, CotasRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped<IConsorcioService, ConsorcioService>();
builder.Services.AddScoped<ICotasService, CotasService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
