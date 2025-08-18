using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using rian_p01_back.src.Data;
using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Data.Repositories.Implementations;
using rian_p01_back.src.Services.Interfaces;
using rian_p01_back.src.Services.Implementations;
using rian_p01_back.Src.Middlewares;

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!))
    };

    // Configurar eventos para customizar as respostas de erro
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            // Impede a resposta padrão
            context.HandleResponse();
            
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            
            var message = context.Error == "invalid_token" ? "Token invalido" : 
                         context.ErrorDescription?.Contains("expired") == true ? "Token expirado" : 
                         "Token nao fornecido ou invalido";
            
            var response = new { message = message };
            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Headers["Token-Expired"] = "true";
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
