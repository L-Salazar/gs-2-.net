using RemoteReady.Data.AppData;
using RemoteReady.Data.Repositories;
using RemoteReady.Data.Repositories.Interfaces;
using RemoteReady.Interfaces;
using RemoteReady.UseCases;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Threading.RateLimiting;
using RemoteReady.Infraestructure.Data.HealthCheck;

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext com Oracle
builder.Services.AddDbContext<ApplicationContext>(options => {
    options.UseOracle(builder.Configuration.GetConnectionString("Oracle"));
});

// Configuração de Health Checks
builder.Services.AddHealthChecks()
                // Liveness
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
                // Readiness
                .AddCheck<OracleHealthCheck>("oracle_query", tags: new[] { "ready" });

// Configuração de autenticação JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Secretkey"]!.ToString());
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

// Registro de dependências - Repositories
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddTransient<IEmpresaRepository, EmpresaRepository>();
builder.Services.AddTransient<IUserPostRepository, UserPostRepository>();
builder.Services.AddTransient<IBlogPostRepository, BlogPostRepository>();
// TODO: Adicionar outros repositories aqui

// Registro de dependências - UseCases
builder.Services.AddScoped<IUsuarioUseCase, UsuarioUseCase>();
builder.Services.AddScoped<IEmpresaUseCase, EmpresaUseCase>();
builder.Services.AddScoped<IBlogPostUseCase, BlogPostUseCase>();
builder.Services.AddScoped<IUserPostUseCase, UserPostUseCase>();
// TODO: Adicionar outros use cases aqui

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.ExampleFilters();

    // Informações da API
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RemoteReady API",
        Version = "v1",
        Description = "API para gerenciamento de usuários, posts, empresas e educação sobre trabalho remoto",
        Contact = new OpenApiContact
        {
            Name = "RemoteReady - FIAP",
            Email = "remoteready@fiap.com.br"
        }
    });

    // Configuração JWT no Swagger
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, new string[] { } }
    });
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

// Configuração de Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // Definindo a política de limitação de taxa
    options.AddFixedWindowLimiter("rateLimitePolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, _) =>
    {
        context.HttpContext.Response.Headers.Append("X-RateLimit-Limit", "5");
        context.HttpContext.Response.Headers.Append("X-RateLimit-Remaining", "0");
        context.HttpContext.Response.Headers.Append("X-RateLimit-Reset", DateTime.UtcNow.AddSeconds(10).ToString("r"));
        await Task.CompletedTask;
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

// Configuração de Health Checks endpoints
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();

public partial class Program { }