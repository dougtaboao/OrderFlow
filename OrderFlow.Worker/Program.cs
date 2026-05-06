using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Services.Orders;
using OrderFlow.Application.Strategies;
using OrderFlow.Application.UseCases;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Messaging;
using OrderFlow.Infrastructure.Observability;
using OrderFlow.Infrastructure.Repositories;
using OrderFlow.Worker;
using Serilog;

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

    var rabbitMqSettings = builder.Configuration
        .GetSection("RabbitMq")
        .Get<RabbitMqSettings>() ?? new RabbitMqSettings();

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

    builder.Services.AddSingleton(rabbitMqSettings);
    builder.Services.AddSingleton(kafkaSettings);

    builder.Services.AddScoped<ICorrelationContext, CorrelationContext>();
    builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrderFlowDbContext>());

    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();

    builder.Services.AddScoped<IProcessOrderUseCase, ProcessOrderUseCase>();
    builder.Services.AddScoped<IPublishOutboxMessagesUseCase, PublishOutboxMessagesUseCase>();

    builder.Services.AddScoped<IIntegrationMessagePublisher, RabbitMqIntegrationMessagePublisher>();
    builder.Services.AddScoped<IOrderEventPublisher, KafkaOrderEventPublisher>();

    builder.Services.AddHostedService<Worker>();
    builder.Services.AddHostedService<OutboxPublisherWorker>();

    builder.Services.AddScoped<IBuyOrderService, BuyOrderService>();
    builder.Services.AddScoped<ISellOrderService, SellOrderService>();
    builder.Services.AddScoped<ITransferOrderService, TransferOrderService>();

    builder.Services.AddScoped<IOrderProcessingStrategy, BuyOrderProcessingStrategy>();
    builder.Services.AddScoped<IOrderProcessingStrategy, SellOrderProcessingStrategy>();
    builder.Services.AddScoped<IOrderProcessingStrategy, TransferOrderProcessingStrategy>();

    builder.Services.AddScoped<IOrderProcessingStrategyResolver, OrderProcessingStrategyResolver>();

var host = builder.Build();
    host.Run();