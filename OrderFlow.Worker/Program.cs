using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.UseCases;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Messaging;
using OrderFlow.Infrastructure.Observability;
using OrderFlow.Infrastructure.Repositories;
using OrderFlow.Worker;

    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddDbContext<OrderFlowDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    var rabbitMqSettings = builder.Configuration
        .GetSection("RabbitMq")
        .Get<RabbitMqSettings>() ?? new RabbitMqSettings();

    var kafkaSettings = builder.Configuration
        .GetSection("Kafka")
        .Get<KafkaSettings>() ?? new KafkaSettings();

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

    var host = builder.Build();
    host.Run();