using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Services.Orders;
using OrderFlow.Application.Strategies;
using OrderFlow.Application.UseCases;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Cache;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Gateways;
using OrderFlow.Infrastructure.HealthChecks;
using OrderFlow.Infrastructure.Messaging;
using OrderFlow.Infrastructure.Observability;
using OrderFlow.Infrastructure.Repositories;
using OrderFlow.Worker;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    //.WriteTo.Console()
    .WriteTo.File("logs/orderflow-worker-.log", rollingInterval: RollingInterval.Day));

builder.Services.AddDbContext<OrderFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var messagingSettings = builder.Configuration.GetSection("Messaging").Get<MessagingSettings>() ?? new();
var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>() ?? new();
var workerSettings = builder.Configuration.GetSection("Workers").Get<WorkerSettings>() ?? new ();
var sqsSettings = builder.Configuration.GetSection("Sqs").Get<SqsSettings>() ?? new();
var kafkaSettings = builder.Configuration.GetSection("Kafka").Get<KafkaSettings>() ?? new();
var redisSettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>() ?? new();

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(KafkaOptions.SectionName));
builder.Services.AddSingleton<IKafkaTopicInitializer, KafkaTopicInitializer>();
builder.Services.AddSingleton(messagingSettings);
builder.Services.AddSingleton(rabbitMqSettings);
builder.Services.AddSingleton(sqsSettings);
builder.Services.AddSingleton(kafkaSettings);
builder.Services.AddSingleton(redisSettings);
builder.Services.AddSingleton(workerSettings);

var otlpEndpoint = builder.Configuration["OpenTelemetry:OtlpEndpoint"];
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("orderflow-worker"))
    .WithTracing(tracing =>
    {
        tracing.AddSource("OrderFlow").AddHttpClientInstrumentation();
        if (!string.IsNullOrWhiteSpace(otlpEndpoint)) tracing.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
    })
    .WithMetrics(metrics =>
    {
        metrics.AddMeter("OrderFlow").AddRuntimeInstrumentation().AddPrometheusExporter();
        if (!string.IsNullOrWhiteSpace(otlpEndpoint)) metrics.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
    });

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddDbContextCheck<OrderFlowDbContext>("sqlserver", tags: ["ready"])
    .AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: ["ready"])
    .AddCheck<KafkaHealthCheck>("kafka", tags: ["ready"]);

builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisSettings.ConnectionString));
builder.Services.AddScoped<ICorrelationContext, CorrelationContext>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrderFlowDbContext>());
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
builder.Services.AddScoped<IOrderAuditReadModelRepository, OrderAuditReadModelRepository>();
builder.Services.AddScoped<IOrderCacheService, RedisOrderCacheService>();
builder.Services.AddScoped<IProcessOrderUseCase, ProcessOrderUseCase>();
builder.Services.AddScoped<IPublishOutboxMessagesUseCase, PublishOutboxMessagesUseCase>();
builder.Services.AddScoped<IRiskAnalysisGateway, FakeRiskAnalysisGateway>();
builder.Services.AddScoped<IBuyOrderService, BuyOrderService>();
builder.Services.AddScoped<ISellOrderService, SellOrderService>();
builder.Services.AddScoped<ITransferOrderService, TransferOrderService>();
builder.Services.AddScoped<IOrderProcessingStrategy, BuyOrderProcessingStrategy>();
builder.Services.AddScoped<IOrderProcessingStrategy, SellOrderProcessingStrategy>();
builder.Services.AddScoped<IOrderProcessingStrategy, TransferOrderProcessingStrategy>();
builder.Services.AddScoped<IOrderProcessingStrategyResolver, OrderProcessingStrategyResolver>();
builder.Services.AddScoped<IOrderEventPublisher, KafkaOrderEventPublisher>();

if (messagingSettings.Provider == MessagingProvider.Sqs)
{
    builder.Services.AddScoped<IIntegrationMessagePublisher, SqsIntegrationMessagePublisher>();
}
else
{
    builder.Services.AddScoped<IIntegrationMessagePublisher, RabbitMqIntegrationMessagePublisher>();
}

if (workerSettings.EnableOrderConsumer)
{
    if (messagingSettings.Provider == MessagingProvider.Sqs)
    {
        builder.Services.AddHostedService<SqsWorker>();
    }
    else
    {
        builder.Services.AddHostedService<Worker>();
    }
}

if (workerSettings.EnableOutboxPublisher)
{
    builder.Services.AddHostedService<OutboxPublisherWorker>();
}

if (workerSettings.EnableKafkaAudit)
{
    builder.Services.AddHostedService<KafkaOrderStatusChangedAuditWorker>();
}

var app = builder.Build();

await app.Services.GetRequiredService<IKafkaTopicInitializer>().InitializeAsync(app.Lifetime.ApplicationStopping);

app.MapPrometheusScrapingEndpoint();
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = x => x.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = x => x.Tags.Contains("ready") });

await app.RunAsync();
