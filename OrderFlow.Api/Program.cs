
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OrderFlow.Api.Middlewares;
using OrderFlow.Api.Security;
using OrderFlow.Api.Settings;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Security;
using OrderFlow.Application.UseCases;
using OrderFlow.Application.Validators;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Cache;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.HealthChecks;
using OrderFlow.Infrastructure.Messaging;
using OrderFlow.Infrastructure.Observability;
using OrderFlow.Infrastructure.Repositories;
using Serilog;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>() ?? new JwtSettings();

var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

// builder.Services.AddAuthorization();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanCreateOrder", policy =>
        policy.RequireRole(Roles.Trader, Roles.Admin));

    options.AddPolicy("CanViewOrder", policy =>
        policy.RequireRole(Roles.Viewer, Roles.Trader, Roles.Admin));

    options.AddPolicy("CanManageOperations", policy =>
        policy.RequireRole(Roles.Admin));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "logs/orderflow-api-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


// builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OrderFlow API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT no formato: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var rabbitMqSettings = builder.Configuration
    .GetSection("RabbitMq")
    .Get<RabbitMqSettings>() ?? new RabbitMqSettings();

builder.Services.AddSingleton(rabbitMqSettings);

var kafkaSettings = builder.Configuration
    .GetSection("Kafka")
    .Get<KafkaSettings>() ?? new KafkaSettings();

builder.Services.AddSingleton(kafkaSettings);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource("OrderFlow")
            .AddConsoleExporter();
    });

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddMeter("OrderFlow")
            .AddConsoleExporter();
    });

builder.Services.AddDbContext<OrderFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddDbContextCheck<OrderFlowDbContext>("sqlserver")
    .AddCheck<RabbitMqHealthCheck>("rabbitmq")
    .AddCheck<KafkaHealthCheck>("kafka");

var redisSettings = builder.Configuration
    .GetSection("Redis")
    .Get<RedisSettings>() ?? new RedisSettings();

builder.Services.AddSingleton(redisSettings);

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisSettings.ConnectionString));

builder.Services.AddScoped<IOrderCacheService, RedisOrderCacheService>();

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrderFlowDbContext>());
builder.Services.AddScoped<ICreateOrderUseCase, CreateOrderUseCase>();
builder.Services.AddScoped<IGetOrderByIdUseCase, GetOrderByIdUseCase>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();

builder.Services.AddScoped<ICorrelationContext, CorrelationContext>();

builder.Services.AddScoped<ICreateOrderValidator, CreateOrderValidator>();

builder.Services.AddScoped<IGetOrderAuditUseCase, GetOrderAuditUseCase>();
builder.Services.AddScoped<IOrderAuditReadModelRepository, OrderAuditReadModelRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<UserContextLoggingMiddleware>();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();

public partial class Program { }
