# OrderFlow - Inventario Arquitetural

> Gerado em 2026-07-13 16:48:21.

> Analise textual automatica dos arquivos C#.
> O resultado deve ser validado contra o codigo-fonte.

## Resumo

- Arquivos C# analisados: 154
- Tipos encontrados: 150
- Projetos encontrados: 10

## Projetos

- OrderFlow.Api: 9 tipos
- OrderFlow.Application: 45 tipos
- OrderFlow.Application.Tests: 1 tipos
- OrderFlow.Domain: 17 tipos
- OrderFlow.Domain.Tests: 1 tipos
- OrderFlow.Grpc: 1 tipos
- OrderFlow.Infrastructure: 42 tipos
- OrderFlow.IntegrationTests: 14 tipos
- OrderFlow.Simulator: 16 tipos
- OrderFlow.Worker: 4 tipos

## Indice por categoria

### API - Controllers

- AuthController (class)
- OrderAuditController (class)
- OrdersController (class)

### API - Middlewares

- CorrelationIdMiddleware (class)
- ExceptionHandlingMiddleware (class)
- UserContextLoggingMiddleware (class)

### API - Other

- Program (class)

### API - Security

- CurrentUser (class)

### API - Settings

- JwtSettings (class)

### Application - DTOs

- CreateOrderRequest (class)
- CreateOrderResponse (class)
- GetOrderAuditResponse (class)
- GetOrderByIdResponse (class)
- OrderEventDto (class)
- ProcessOrderResult (class)
- RiskAnalysisResult (class)

### Application - Interfaces

- ICreateOrderUseCase (interface)
- ICreateOrderValidator (interface)
- IGetOrderAuditUseCase (interface)
- IGetOrderByIdUseCase (interface)
- IIntegrationMessagePublisher (interface)
- IOrderCacheService (interface)
- IOrderEventPublisher (interface)
- IOrderMessagePublisher (interface)
- IOrderProcessingStrategyResolver (interface)
- IProcessOrderUseCase (interface)
- IPublishOutboxMessagesUseCase (interface)
- IRiskAnalysisGateway (interface)

### Application - Messaging

- KafkaSettings (class)
- OrderCompletedIntegrationEvent (class)
- OrderCreatedMessage (class)
- OrderStatusChangedIntegrationEvent (class)

### Application - Observability

- LogEvents (class)
- LogProperties (class)
- Metrics (class)
- Telemetry (class)

### Application - Other

- ICurrentUser (interface)
- Roles (class)

### Application - Services

- BuyOrderService (class)
- IBuyOrderService (interface)
- ISellOrderService (interface)
- ITransferOrderService (interface)
- SellOrderService (class)
- TransferOrderService (class)

### Application - Strategies

- BuyOrderProcessingStrategy (class)
- OrderProcessingStrategyResolver (class)
- SellOrderProcessingStrategy (class)
- TransferOrderProcessingStrategy (class)

### Application - Use Cases

- CreateOrderUseCase (class)
- GetOrderAuditUseCase (class)
- GetOrderByIdUseCase (class)
- ProcessOrderUseCase (class)
- PublishOutboxMessagesUseCase (class)

### Application - Validators

- CreateOrderValidator (class)

### Domain - Common

- BaseEntity (class)

### Domain - Entities

- Order (class)
- OrderEvent (class)
- OutboxMessage (class)

### Domain - Enums

- OrderEventType (enum)
- OrderPriority (enum)
- OrderStatus (enum)
- OrderType (enum)
- StatusPedido (enum)

### Domain - Exceptions

- DomainException (class)

### Domain - Interfaces

- ICorrelationContext (interface)
- IOrderAuditReadModelRepository (interface)
- IOrderProcessingStrategy (interface)
- IOrderRepository (interface)
- IOutboxMessageRepository (interface)
- IUnitOfWork (interface)

### Domain - Read Models

- OrderAuditReadModel (class)

### gRPC

- OrderQueryGrpcService (class)

### Infrastructure - Cache

- RedisOrderCacheService (class)
- RedisSettings (class)

### Infrastructure - Data

- AddCorrelationIdToOutboxMessages (class)
- AddCorrelationIdToOutboxMessages (class)
- AddOrderAuditReadModel (class)
- AddOrderAuditReadModel (class)
- AddOrderTypePriorityExternalReference (class)
- AddOrderTypePriorityExternalReference (class)
- AddOutboxMessages (class)
- AddOutboxMessages (class)
- AddRowVersionToOrder (class)
- AddRowVersionToOrder (class)
- AddSpecificOrderFields (class)
- AddSpecificOrderFields (class)
- BaseConfigutationTreino (class)
- BaseEntityConfiguration (class)
- InitialCreate (class)
- InitialCreate (class)
- OrderAuditReadModelConfiguration (class)
- OrderConfiguration (class)
- OrderEventConfiguration (class)
- OrderFlowDbContext (class)
- OrderFlowDbContextModelSnapshot (class)
- OutboxMessageConfiguration (class)
- UpdateOrderDomainFields (class)
- UpdateOrderDomainFields (class)

### Infrastructure - Gateways

- FakeRiskAnalysisGateway (class)

### Infrastructure - Health Checks

- KafkaHealthCheck (class)
- RabbitMqHealthCheck (class)

### Infrastructure - Messaging

- KafkaOrderEventPublisher (class)
- MessagingProvider (enum)
- MessagingSettings (class)
- RabbitMqIntegrationMessagePublisher (class)
- RabbitMqOrderMessagePublisher (class)
- RabbitMqSettings (class)
- SqsIntegrationMessagePublisher (class)
- SqsSettings (class)

### Infrastructure - Observability

- CorrelationContext (class)

### Infrastructure - Repositories

- InMemoryOrderRepository (class)
- OrderAuditReadModelRepository (class)
- OrderRepository (class)
- OutboxMessageRepository (class)

### Simulator

- DefaultSimulationScenario (class)
- ISimulationRunner (interface)
- ISimulationScenario (interface)
- MarketOpenSimulationScenario (class)
- OrderGenerationOptions (class)
- OrderGenerator (class)
- OrderSender (class)
- ProgressRenderer (class)
- ScenarioSettingsResolver (class)
- SimulationHostedService (class)
- SimulationRunner (class)
- SimulationScenarioResolver (class)
- SimulationStatistics (class)
- SimulationStatistics (class)
- SimulatorSettings (class)
- StatisticsPrinter (class)

### Tests

- CreateOrderApiIntegrationTests (class)
- CreateOrderUseCaseTests (class)
- DatabaseFixture (class)
- FakeAuthenticationHandler (class)
- FakeCurrentUser (class)
- IntegrationTestFixture (class)
- KafkaFixture (class)
- KafkaIntegrationTests (class)
- OrderFlowApiFactory (class)
- OrderRepositoryIntegrationTests (class)
- OrderRepositorySqlServerIntegrationTests (class)
- OrderRepositorySqlServerIntegrationTests_bkp (class)
- OrderTests (class)
- RabbitMqFixture (class)
- RabbitMqOrderMessagePublisherTests (class)
- RedisOrderCacheServiceTests (class)

### Worker

- KafkaOrderStatusChangedAuditWorker (class)
- OutboxPublisherWorker (class)
- SqsWorker (class)
- Worker (class)

## Catalogo detalhado

# API - Controllers

## AuthController

- Projeto: OrderFlow.Api
- Tipo: class
- Namespace: OrderFlow.Api.Controllers
- Arquivo: OrderFlow.Api\Controllers\AuthController.cs
