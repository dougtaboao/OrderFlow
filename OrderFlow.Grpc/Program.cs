using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.UseCases;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Grpc.Services;
using OrderFlow.Infrastructure.Cache;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Repositories;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddScoped<IGetOrderByIdUseCase, GetOrderByIdUseCase>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddDbContext<OrderFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var redisSettings = builder.Configuration
    .GetSection("Redis")
    .Get<RedisSettings>() ?? new RedisSettings();

builder.Services.AddSingleton(redisSettings);

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisSettings.ConnectionString));

builder.Services.AddScoped<IOrderCacheService, RedisOrderCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<OrderQueryGrpcService>();

app.MapGet("/", () => "OrderFlow gRPC service.");

app.Run();
