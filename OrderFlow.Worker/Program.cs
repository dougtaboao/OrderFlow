using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Services.Orders;
using OrderFlow.Application.Strategies;
using OrderFlow.Application.UseCases;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Cache;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Gateways;
using OrderFlow.Infrastructure.Messaging;
using OrderFlow.Infrastructure.Observability;
using OrderFlow.Infrastructure.Repositories;
using OrderFlow.Worker;
using Serilog;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddSerilog((services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.File(
            path: "logs/orderflow-worker-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
        });

    builder.Services.AddDbContext<OrderFlowDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    var messagingSettings = builder.Configuration
    .GetSection("Messaging")
    .Get<MessagingSettings>() ?? new MessagingSettings();

    var rabbitMqSettings = builder.Configuration
        .GetSection("RabbitMq")
        .Get<RabbitMqSettings>() ?? new RabbitMqSettings();

    var sqsSettings = builder.Configuration
    .GetSection("Sqs")
    .Get<SqsSettings>() ?? new SqsSettings();

    var kafkaSettings = builder.Configuration
        .GetSection("Kafka")
        .Get<KafkaSettings>() ?? new KafkaSettings();

    builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
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

    builder.Services.AddSingleton(messagingSettings);
    builder.Services.AddSingleton(sqsSettings);
    builder.Services.AddSingleton(rabbitMqSettings);
    builder.Services.AddSingleton(kafkaSettings);

    builder.Services.AddScoped<ICorrelationContext, CorrelationContext>();
    builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrderFlowDbContext>());

    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();

    builder.Services.AddScoped<IProcessOrderUseCase, ProcessOrderUseCase>();
    builder.Services.AddScoped<IPublishOutboxMessagesUseCase, PublishOutboxMessagesUseCase>();

    builder.Services.AddScoped<IRiskAnalysisGateway, FakeRiskAnalysisGateway>();

if (messagingSettings.Provider == MessagingProvider.Sqs)
    {
    Console.WriteLine($"Provider configurado: {messagingSettings.Provider}");
    builder.Services.AddScoped<IIntegrationMessagePublisher, SqsIntegrationMessagePublisher>();
        builder.Services.AddHostedService<SqsWorker>();
    }
    else
    {
    Console.WriteLine($"Provider configurado: {messagingSettings.Provider}");
    builder.Services.AddScoped<IIntegrationMessagePublisher, RabbitMqIntegrationMessagePublisher>();
        builder.Services.AddHostedService<Worker>();
    }

    var redisSettings = builder.Configuration
    .GetSection("Redis")
    .Get<RedisSettings>() ?? new RedisSettings();

    builder.Services.AddSingleton(redisSettings);

    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(redisSettings.ConnectionString));

    builder.Services.AddScoped<IOrderCacheService, RedisOrderCacheService>();

    builder.Services.AddHostedService<OutboxPublisherWorker>();

    builder.Services.AddScoped<IBuyOrderService, BuyOrderService>();
    builder.Services.AddScoped<ISellOrderService, SellOrderService>();
    builder.Services.AddScoped<ITransferOrderService, TransferOrderService>();
    builder.Services.AddHostedService<KafkaOrderCompletedAuditWorker>();

builder.Services.AddScoped<IOrderProcessingStrategy, BuyOrderProcessingStrategy>();
    builder.Services.AddScoped<IOrderProcessingStrategy, SellOrderProcessingStrategy>();
    builder.Services.AddScoped<IOrderProcessingStrategy, TransferOrderProcessingStrategy>();

    builder.Services.AddScoped<IOrderProcessingStrategyResolver, OrderProcessingStrategyResolver>();

    builder.Services.AddScoped<IOrderEventPublisher, KafkaOrderEventPublisher>();

    var host = builder.Build();
    host.Run();