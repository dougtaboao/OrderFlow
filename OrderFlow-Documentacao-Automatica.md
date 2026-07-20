# OrderFlow - Documentacao Automatica

> Gerado automaticamente em 2026-07-13 14:18:37.

> Este documento representa um snapshot tecnico da solucao.
> Revise o arquivo antes de compartilhar.

## Solucoes encontradas

- OrderFlow.slnx

## Estrutura da solucao

~~~~text
.github/
    workflows/
        orderflow-ci.yml
.gitignore
coverage-report/
    class.js
    icon_cog.svg
    icon_cog_dark.svg
    icon_cube.svg
    icon_cube_dark.svg
    icon_fork.svg
    icon_fork_dark.svg
    icon_info-circled.svg
    icon_info-circled_dark.svg
    icon_minus.svg
    icon_minus_dark.svg
    icon_plus.svg
    icon_plus_dark.svg
    icon_search-minus.svg
    icon_search-minus_dark.svg
    icon_search-plus.svg
    icon_search-plus_dark.svg
    icon_sponsor.svg
    icon_star.svg
    icon_star_dark.svg
    icon_up-dir.svg
    icon_up-dir_active.svg
    icon_up-down-dir.svg
    icon_up-down-dir_dark.svg
    icon_wrench.svg
    icon_wrench_dark.svg
    index.htm
    index.html
    main.js
    OrderFlow.Application_BuyOrderProcessingStrategy.html
    OrderFlow.Application_BuyOrderService.html
    OrderFlow.Application_CreateOrderRequest.html
    OrderFlow.Application_CreateOrderResponse.html
    OrderFlow.Application_CreateOrderUseCase.html
    OrderFlow.Application_CreateOrderValidator.html
    OrderFlow.Application_GetOrderAuditResponse.html
    OrderFlow.Application_GetOrderAuditUseCase.html
    OrderFlow.Application_GetOrderByIdResponse.html
    OrderFlow.Application_GetOrderByIdUseCase.html
    OrderFlow.Application_KafkaSettings.html
    OrderFlow.Application_Metrics.html
    OrderFlow.Application_OrderCompletedIntegrationEvent.html
    OrderFlow.Application_OrderCreatedMessage.html
    OrderFlow.Application_OrderEventDto.html
    OrderFlow.Application_OrderProcessingStrategyResolver.html
    OrderFlow.Application_OrderStatusChangedIntegrationEvent.html
    OrderFlow.Application_ProcessOrderResult.html
    OrderFlow.Application_ProcessOrderUseCase.html
    OrderFlow.Application_PublishOutboxMessagesUseCase.html
    OrderFlow.Application_RiskAnalysisResult.html
    OrderFlow.Application_SellOrderProcessingStrategy.html
    OrderFlow.Application_SellOrderService.html
    OrderFlow.Application_Telemetry.html
    OrderFlow.Application_TransferOrderProcessingStrategy.html
    OrderFlow.Application_TransferOrderService.html
    OrderFlow.Domain_BaseEntity.html
    OrderFlow.Domain_DomainException.html
    OrderFlow.Domain_Order.html
    OrderFlow.Domain_OrderAuditReadModel.html
    OrderFlow.Domain_OrderEvent.html
    OrderFlow.Domain_OutboxMessage.html
    report.css
docker-compose.yml
Gerar-Documentacao-OrderFlow.ps1
Gerar-Documentacao-OrderFlow-Corrigido.ps1
Gerar-Documentacao-OrderFlow-SemCrases.ps1
OrderFlow.Api/
    appsettings.Development.json
    appsettings.json
    Controllers/
        AuthController.cs
        OrderAuditController.cs
        OrdersController.cs
    Dockerfile
    logs/
        orderflow-api-20260428.log
        orderflow-api-20260429.log
        orderflow-api-20260504.log
        orderflow-api-20260505.log
        orderflow-api-20260512.log
        orderflow-api-20260515.log
        orderflow-api-20260518.log
        orderflow-api-20260519.log
        orderflow-api-20260520.log
        orderflow-api-20260521.log
        orderflow-api-20260526.log
        orderflow-api-20260527.log
        orderflow-api-20260529.log
    Middlewares/
        CorrelationIdMiddleware.cs
        ExceptionHandlingMiddleware.cs
        UserContextLoggingMiddleware.cs
    OrderFlow.Api.csproj
    OrderFlow.Api.http
    Program.cs
    Properties/
    Security/
        CurrentUser.cs
    Settings/
        JwtSettings.cs
OrderFlow.Application/
OrderFlow.Application.Tests/
    OrderFlow.Application.Tests.csproj
    UseCases/
        CreateOrderUseCaseTests.cs
    Dtos/
        CreateOrderRequest.cs
        CreateOrderResponse.cs
        GetOrderAuditResponse.cs
        GetOrderByIdResponse.cs
        OrderEventDto.cs
        ProcessOrderResult.cs
        RiskAnalysisResult.cs
    Interfaces/
        ICreateOrderUseCase.cs
        ICreateOrderValidator.cs
        IGetOrderAuditUseCase.cs
        IGetOrderByIdUseCase.cs
        IIntegrationMessagePublisher.cs
        IOrderCacheService.cs
        IOrderEventPublisher.cs
        IOrderMessagePublisher.cs
        IOrderProcessingStrategyResolver.cs
        IProcessOrderUseCase.cs
        IPublishOutboxMessagesUseCase.cs
        IRiskAnalysisGateway.cs
    Messaging/
        KafkaSettings.cs
        OrderCompletedIntegrationEvent.cs
        OrderCreatedMessage.cs
        OrderStatusChangedIntegrationEvent.cs
    Observability/
        LogEvents.cs
        LogProperties.cs
        Metrics.cs
        Telemetry.cs
    OrderFlow.Application.csproj
    Security/
        ICurrentUser.cs
        Roles.cs
    Services/
        Orders/
            BuyOrderService .cs
            IBuyOrderService.cs
            ISellOrderService.cs
            ITransferOrderService.cs
            SellOrderService.cs
            TransferOrderService.cs
    Strategies/
        BuyOrderProcessingStrategy.cs
        OrderProcessingStrategyResolver.cs
        SellOrderProcessingStrategy.cs
        TransferOrderProcessingStrategy.cs
    UseCases/
        CreateOrderUseCase.cs
        GetOrderAuditUseCase.cs
        GetOrderByIdUseCase.cs
        ProcessOrderUseCase.cs
        PublishOutboxMessagesUseCase.cs
    Validators/
        CreateOrderValidator.cs
OrderFlow.Domain/
OrderFlow.Domain.Tests/
    Entities/
        OrderTests.cs
    OrderFlow.Domain.Tests.csproj
    Common/
        BaseEntity.cs
    Entities/
        Order.cs
        OrderEvent.cs
        OutboxMessage.cs
    Enums/
        OrderEventType.cs
        OrderPriority.cs
        OrderStatus.cs
        OrderType.cs
        StatusPedido.cs
    Exceptions/
        DomainException.cs
    Interfaces/
        ICorrelationContext.cs
        IOrderAuditReadModelRepository.cs
        IOrderProcessingStrategy.cs
        IOrderRepository.cs
        IOutboxMessageRepository.cs
        IUnitOfWork.cs
    OrderFlow.Domain.csproj
    ReadModels/
        OrderAuditReadModel.cs
OrderFlow.Grpc/
OrderFlow.Grpc.Client/
    OrderFlow.Grpc.Client.csproj
    Program.cs
    appsettings.Development.json
    appsettings.json
    Dockerfile
    OrderFlow.Grpc.csproj
    Program.cs
    Properties/
    Protos/
        order_query.proto
    Services/
        OrderQueryGrpcService.cs
OrderFlow.Infrastructure/
    Cache/
        RedisOrderCacheService.cs
        RedisSettings.cs
    Data/
        Configurations/
            Common/
                BaseConfigutationTreino.cs
                BaseEntityConfiguration.cs
            OrderAuditReadModelConfiguration.cs
            OrderConfiguration.cs
            OrderEventConfiguration.cs
        Migrations/
            20260408172736_InitialCreate.cs
            20260408172736_InitialCreate.Designer.cs
            20260413191740_AddOutboxMessages.cs
            20260413191740_AddOutboxMessages.Designer.cs
            20260415145046_AddRowVersionToOrder.cs
            20260415145046_AddRowVersionToOrder.Designer.cs
            20260415155419_AddCorrelationIdToOutboxMessages.cs
            20260415155419_AddCorrelationIdToOutboxMessages.Designer.cs
            20260424180612_AddOrderTypePriorityExternalReference.cs
            20260424180612_AddOrderTypePriorityExternalReference.Designer.cs
            20260427173811_AddSpecificOrderFields.cs
            20260427173811_AddSpecificOrderFields.Designer.cs
            20260429171655_UpdateOrderDomainFields.cs
            20260429171655_UpdateOrderDomainFields.Designer.cs
            20260521200326_AddOrderAuditReadModel.cs
            20260521200326_AddOrderAuditReadModel.Designer.cs
            OrderFlowDbContextModelSnapshot.cs
        OrderFlowDbContext.cs
        OutboxMessageConfiguration.cs
    Gateways/
        FakeRiskAnalysisGateway.cs
    HealthChecks/
        KafkaHealthCheck.cs
        RabbitMqHealthCheck.cs
    Messaging/
        KafkaOrderEventPublisher.cs
        MessagingProvider.cs
        MessagingSettings.cs
        RabbitMqIntegrationMessagePublisher.cs
        RabbitMqOrderMessagePublisher.cs
        RabbitMqSettings.cs
        SqsIntegrationMessagePublisher.cs
        SqsSettings.cs
    Observability/
        CorrelationContext .cs
    OrderFlow.Infrastructure.csproj
    Repositories/
        InMemoryOrderRepository.cs
        OrderAuditReadModelRepository.cs
        OrderRepository.cs
        OutboxMessageRepository.cs
OrderFlow.IntegrationTests/
    Api/
        CreateOrderApiIntegrationTests.cs
        OrderFlowApiFactory.cs
    Cache/
        RedisOrderCacheServiceTests.cs
    Fakes/
        FakeAuthenticationHandler .cs
        FakeCurrentUser.cs
    Fixtures/
        DatabaseFixture.cs
        IntegrationTestFixture.cs
        KafkaFixture.cs
        RabbitMqFixture.cs
    Messaging/
        KafkaIntegrationTests.cs
        RabbitMqOrderMessagePublisherTests.cs
    OrderFlow.IntegrationTests.csproj
    Repositories/
        OrderRepositoryIntegrationTests.cs
        OrderRepositorySqlServerIntegrationTests.cs
        OrderRepositorySqlServerIntegrationTests_bkp.cs
OrderFlow.Simulator/
    appsettings.json
    Configuration/
        OrderGenerationOptions.cs
        SimulatorSettings.cs
    HostedServices/
        SimulationHostedService.cs
    Models/
        SimulationStatistics.cs
    OrderFlow.Simulator.csproj
    Program.cs
    run-simulator.ps1
    Scenarios/
        DefaultSimulationScenario.cs
        ISimulationScenario.cs
        MarketOpenSimulationScenario.cs
        SimulationScenarioResolver.cs
    Services/
        ISimulationRunner.cs
        OrderGenerator.cs
        OrderSender.cs
        ProgressRenderer.cs
        ScenarioSettingsResolver.cs
        SimulationRunner.cs
        SimulationStatistics.cs
        StatisticsPrinter.cs
OrderFlow.slnx
OrderFlow.slnx.bkp
OrderFlow.Worker/
    appsettings.Development.json
    appsettings.json
    Dockerfile
    KafkaOrderStatusChangedAuditWorker.cs
    logs/
        orderflow-worker-20260428.log
        orderflow-worker-20260429.log
        orderflow-worker-20260504.log
        orderflow-worker-20260505.log
        orderflow-worker-20260512.log
        orderflow-worker-20260515.log
        orderflow-worker-20260518.log
        orderflow-worker-20260519.log
        orderflow-worker-20260520.log
        orderflow-worker-20260521.log
        orderflow-worker-20260526.log
        orderflow-worker-20260527.log
    OrderFlow.Worker.csproj
    OutboxPublisherWorker.cs
    Program.cs
    Properties/
    SqsWorker.cs
    Worker.cs
OrderFlow-Documentacao-Automatica.md
prometheus.yml
~~~~

## Projetos .NET

- OrderFlow.Api\OrderFlow.Api.csproj
- OrderFlow.Application.Tests\OrderFlow.Application.Tests.csproj
- OrderFlow.Application\OrderFlow.Application.csproj
- OrderFlow.Domain.Tests\OrderFlow.Domain.Tests.csproj
- OrderFlow.Domain\OrderFlow.Domain.csproj
- OrderFlow.Grpc.Client\OrderFlow.Grpc.Client.csproj
- OrderFlow.Grpc\OrderFlow.Grpc.csproj
- OrderFlow.Infrastructure\OrderFlow.Infrastructure.csproj
- OrderFlow.IntegrationTests\OrderFlow.IntegrationTests.csproj
- OrderFlow.Simulator\OrderFlow.Simulator.csproj
- OrderFlow.Worker\OrderFlow.Worker.csproj

## Pacotes NuGet por projeto


### OrderFlow.Api\OrderFlow.Api.csproj

_Nao foi possivel interpretar o projeto: A propriedade 'PackageReference' não foi encontrada neste objeto. Verifique se a propriedade existe._

### OrderFlow.Application.Tests\OrderFlow.Application.Tests.csproj

_Nao foi possivel interpretar o projeto: A propriedade 'PackageReference' não foi encontrada neste objeto. Verifique se a propriedade existe._

### OrderFlow.Application\OrderFlow.Application.csproj

_Nao foi possivel interpretar o projeto: A propriedade 'PackageReference' não foi encontrada neste objeto. Verifique se a propriedade existe._

### OrderFlow.Domain.Tests\OrderFlow.Domain.Tests.csproj

_Nao foi possivel interpretar o projeto: A propriedade 'PackageReference' não foi encontrada neste objeto. Verifique se a propriedade existe._

### OrderFlow.Domain\OrderFlow.Domain.csproj

_Nao foi possivel interpretar o projeto: A propriedade 'ItemGroup' não foi encontrada neste objeto. Verifique se a propriedade existe._

### OrderFlow.Grpc.Client\OrderFlow.Grpc.Client.csproj

_Nao foi possivel interpretar o projeto: A propriedade 'PackageReference' não foi encontrada neste objeto. Verifique se a propriedade existe._

### OrderFlow.Grpc\OrderFlow.Grpc.csproj

_Nao foi possivel interpretar o projeto: A propriedade 'PackageReference' não foi encontrada neste objeto. Verifique se a propriedade existe._

### OrderFlow.Infrastructure\OrderFlow.Infrastructure.csproj

_Nao foi possivel interpretar o projeto: A propriedade 'PackageReference' não foi encontrada neste objeto. Verifique se a propriedade existe._

### OrderFlow.IntegrationTests\OrderFlow.IntegrationTests.csproj

_Nao foi possivel interpretar o projeto: A propriedade 'PackageReference' não foi encontrada neste objeto. Verifique se a propriedade existe._

### OrderFlow.Simulator\OrderFlow.Simulator.csproj

_Nao foi possivel interpretar o projeto: A propriedade 'PackageReference' não foi encontrada neste objeto. Verifique se a propriedade existe._

### OrderFlow.Worker\OrderFlow.Worker.csproj

_Nao foi possivel interpretar o projeto: A propriedade 'PackageReference' não foi encontrada neste objeto. Verifique se a propriedade existe._

## Dependencias entre projetos

_Erro ao ler referencias de OrderFlow.Api\OrderFlow.Api.csproj._
_Erro ao ler referencias de OrderFlow.Application.Tests\OrderFlow.Application.Tests.csproj._
_Erro ao ler referencias de OrderFlow.Application\OrderFlow.Application.csproj._
_Erro ao ler referencias de OrderFlow.Domain.Tests\OrderFlow.Domain.Tests.csproj._
_Erro ao ler referencias de OrderFlow.Domain\OrderFlow.Domain.csproj._
_Erro ao ler referencias de OrderFlow.Grpc.Client\OrderFlow.Grpc.Client.csproj._
_Erro ao ler referencias de OrderFlow.Grpc\OrderFlow.Grpc.csproj._
_Erro ao ler referencias de OrderFlow.Infrastructure\OrderFlow.Infrastructure.csproj._
_Erro ao ler referencias de OrderFlow.IntegrationTests\OrderFlow.IntegrationTests.csproj._
_Erro ao ler referencias de OrderFlow.Simulator\OrderFlow.Simulator.csproj._
_Erro ao ler referencias de OrderFlow.Worker\OrderFlow.Worker.csproj._

## Arquivos importantes

- .github\workflows\orderflow-ci.yml
- docker-compose.yml
- OrderFlow.Api\appsettings.Development.json
- OrderFlow.Api\appsettings.json
- OrderFlow.Api\Dockerfile
- OrderFlow.Api\Program.cs
- OrderFlow.Grpc.Client\Program.cs
- OrderFlow.Grpc\appsettings.Development.json
- OrderFlow.Grpc\appsettings.json
- OrderFlow.Grpc\Dockerfile
- OrderFlow.Grpc\Program.cs
- OrderFlow.Simulator\appsettings.json
- OrderFlow.Simulator\Program.cs
- OrderFlow.Worker\appsettings.Development.json
- OrderFlow.Worker\appsettings.json
- OrderFlow.Worker\Dockerfile
- OrderFlow.Worker\Program.cs

## Conteudo dos arquivos importantes

> Revise este trecho antes de compartilhar.
> O script mascara alguns padroes comuns de segredo, mas a revisao humana continua obrigatoria.

### .github\workflows\orderflow-ci.yml

~~~~yml
name: OrderFlow CI

on:
  workflow_dispatch:

  push:
    branches:
      - main
      - feature/**
  pull_request:
    branches:
      - main

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2022-latest
        env:
          ACCEPT_EULA: "Y"
          SA_PASSWORD=***REDACTED***
        ports:
          - 1433:1433
        options: >-
          --health-cmd "/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'OrderFlow@123' -Q 'SELECT 1' -C"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 10
          --health-start-period 20s

      redis:
        image: redis:7
        ports:
          - 6379:6379
        options: >-
          --health-cmd "redis-cli ping"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
          --health-start-period 10s

      rabbitmq:
        image: rabbitmq:3-management
        ports:
          - 5672:5672
          - 15672:15672
        options: >-
          --health-cmd "rabbitmq-diagnostics -q ping"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 10
          --health-start-period 20s
          
      kafka:
        image: apache/kafka:3.9.0
        ports:
          - 9092:9092
        env:
          KAFKA_NODE_ID: 1
          KAFKA_PROCESS_ROLES: broker,controller
          KAFKA_CONTROLLER_QUORUM_VOTERS: 1@localhost:9093
          KAFKA_LISTENERS: PLAINTEXT://:9092,CONTROLLER://:9093
          KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://localhost:9092
          KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: CONTROLLER:PLAINTEXT,PLAINTEXT:PLAINTEXT
          KAFKA_CONTROLLER_LISTENER_NAMES: CONTROLLER
          KAFKA_AUTO_CREATE_TOPICS_ENABLE: "true"
        options: >-
          --health-cmd "/opt/kafka/bin/kafka-topics.sh --bootstrap-server localhost:9092 --list"
          --health-interval 10s
          --health-timeout 10s
          --health-retries 20
          --health-start-period 60s

    env:
      ConnectionStrings__DefaultConnection: "Server=localhost,1433;Database=OrderFlowTestsDb;User Id=***REDACTED***;Password=***REDACTED***;TrustServerCertificate=True;"
      Redis__ConnectionString: localhost:6379,abortConnect=false
      Redis__OrderCacheExpirationMinutes: 5
      RabbitMq__HostName: localhost
      RabbitMq__QueueName: order-created
      RabbitMq__DeadLetterQueueName: order-created-dlq
      RabbitMq__MaxRetryCount: 3
      Kafka__BootstrapServers: localhost:9092
      Kafka__OrderCreatedTopic: order-created-test
      Kafka__ConsumerGroup: orderflow-tests

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore dependencies
        run: dotnet restore OrderFlow.slnx

      - name: Build solution
        run: dotnet build OrderFlow.slnx --configuration Release --no-restore

      - name: Run tests with coverage
        run: dotnet test OrderFlow.slnx --configuration Release --no-build --collect:"XPlat Code Coverage"
  
  docker-build-and-push:
    runs-on: ubuntu-latest
    needs: build-and-test
    if: github.event_name != 'pull_request'

    permissions:
      contents: read
      packages: write

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Set lowercase owner
        run: echo "IMAGE_OWNER=${GITHUB_REPOSITORY_OWNER,,}" >> $GITHUB_ENV

      - name: Login to GHCR
        run: echo "${{ secrets.GITHUB_TOKEN }}" | docker login ghcr.io -u ${{ github.actor }} --password-stdin

      - name: Build and push API Docker image
        run: |
          docker build \
            -f OrderFlow.Api/Dockerfile \
            -t ghcr.io/${IMAGE_OWNER}/orderflow-api:latest \
            -t ghcr.io/${IMAGE_OWNER}/orderflow-api:${{ github.sha }} \
            .

          docker push ghcr.io/${IMAGE_OWNER}/orderflow-api:latest
          docker push ghcr.io/${IMAGE_OWNER}/orderflow-api:${{ github.sha }}

      - name: Build and push Worker Docker image
        run: |
          docker build \
            -f OrderFlow.Worker/Dockerfile \
            -t ghcr.io/${IMAGE_OWNER}/orderflow-worker:latest \
            -t ghcr.io/${IMAGE_OWNER}/orderflow-worker:${{ github.sha }} \
            .

          docker push ghcr.io/${IMAGE_OWNER}/orderflow-worker:latest
          docker push ghcr.io/${IMAGE_OWNER}/orderflow-worker:${{ github.sha }}

      - name: Build and push gRPC Docker image
        run: |
          docker build \
            -f OrderFlow.Grpc/Dockerfile \
            -t ghcr.io/${IMAGE_OWNER}/orderflow-grpc:latest \
            -t ghcr.io/${IMAGE_OWNER}/orderflow-grpc:${{ github.sha }} \
            .

          docker push ghcr.io/${IMAGE_OWNER}/orderflow-grpc:latest
          docker push ghcr.io/${IMAGE_OWNER}/orderflow-grpc:${{ github.sha }}
~~~~

### docker-compose.yml

~~~~yml
services:

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: orderflow-sqlserver
    environment:
      SA_PASSWORD=***REDACTED***
      ACCEPT_EULA: "Y"
    ports:
      - "${SQL_PORT}:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
    healthcheck:
      test: ["CMD", "/opt/mssql-tools/bin/sqlcmd", "-S", "localhost", "-U", "sa", "-P", "${SA_PASSWORD}", "-Q", "SELECT 1"]
      interval: 10s
      timeout: 5s
      retries: 10
    networks:
      - orderflow-network

  redis:
    image: redis:latest
    container_name: orderflow-redis
    ports:
      - "${REDIS_PORT}:6379"
    volumes:
      - redis_data:/data
    networks:
      - orderflow-network

  rabbitmq:
    image: rabbitmq:3-management
    container_name: orderflow-rabbitmq
    ports:
      - "${RABBITMQ_PORT}:5672"
      - "${RABBITMQ_MANAGEMENT_PORT}:15672"
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "-q", "ping"]
      interval: 10s
      timeout: 5s
      retries: 10
      start_period: 20s
    networks:
      - orderflow-network

  kafka:
    image: apache/kafka:latest
    container_name: orderflow-kafka
    ports:
      - "${KAFKA_PORT}:9092"
    environment:
      KAFKA_NODE_ID: 1
      KAFKA_PROCESS_ROLES: broker,controller
      KAFKA_LISTENERS: INTERNAL://:29092,EXTERNAL://:9092,CONTROLLER://:9093
      KAFKA_ADVERTISED_LISTENERS: INTERNAL://kafka:29092,EXTERNAL://localhost:9092
      KAFKA_CONTROLLER_LISTENER_NAMES: CONTROLLER
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: INTERNAL:PLAINTEXT,EXTERNAL:PLAINTEXT,CONTROLLER:PLAINTEXT
      KAFKA_INTER_BROKER_LISTENER_NAME: INTERNAL
      KAFKA_CONTROLLER_QUORUM_VOTERS: 1@kafka:9093
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
    volumes:
      - kafka_data:/var/lib/kafka/data
    networks:
      - orderflow-network

  orderflow-grpc:
    build:
      context: .
      dockerfile: OrderFlow.Grpc/Dockerfile
    container_name: orderflow-grpc
    ports:
      - "5001:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: "Development"
      ASPNETCORE_URLS: "http://+:8080"

      ConnectionStrings__DefaultConnection: "Server=sqlserver,1433;Database=OrderFlowDb;User Id=***REDACTED***;Password=***REDACTED***;TrustServerCertificate=True"

      Redis__ConnectionString: "redis:6379"

      Kafka__BootstrapServers: "kafka:29092"

      RabbitMq__HostName: "rabbitmq"

      Serilog__WriteTo__1__Name: "Seq"
      Serilog__WriteTo__1__Args__serverUrl: "http://seq:5341"

    depends_on:
      - sqlserver
      - redis
      - kafka
      - rabbitmq
      - seq

    networks:
      - orderflow-network
      
  orderflow-api:
    build:
      context: .
      dockerfile: OrderFlow.Api/Dockerfile
    container_name: orderflow-api
    ports:
      - "5000:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: "Development"
      ConnectionStrings__DefaultConnection: "Server=sqlserver,1433;Database=OrderFlowDb;User Id=***REDACTED***;Password=***REDACTED***;TrustServerCertificate=True"
      Redis__ConnectionString: "redis:6379"
      Kafka__BootstrapServers: "kafka:29092"
      RabbitMq__HostName: "rabbitmq"
      Serilog__WriteTo__1__Name: "Seq"
      Serilog__WriteTo__1__Args__serverUrl: "http://seq:5341"
    depends_on:
      - sqlserver
      - redis
      - kafka
      - rabbitmq
      - seq
    volumes:
      - dataprotection_keys:/root/.aspnet/DataProtection-Keys
    networks:
      - orderflow-network
      
  orderflow-worker:
    build:
      context: .
      dockerfile: OrderFlow.Worker/Dockerfile
    container_name: orderflow-worker
    environment:
      DOTNET_ENVIRONMENT: "Development"
      ConnectionStrings__DefaultConnection: "Server=sqlserver,1433;Database=OrderFlowDb;User Id=***REDACTED***;Password=***REDACTED***;TrustServerCertificate=True"
      Redis__ConnectionString: "redis:6379"
      Kafka__BootstrapServers: "kafka:29092"
      RabbitMq__HostName: "rabbitmq"
      Messaging__Provider: "RabbitMq"
      Serilog__WriteTo__1__Name: "Seq"
      Serilog__WriteTo__1__Args__serverUrl: "http://seq:5341"
    depends_on:
      rabbitmq:
        condition: service_healthy
      sqlserver:
        condition: service_started
      redis:
        condition: service_started
      kafka:
        condition: service_started
      seq:
        condition: service_started
    networks:
      - orderflow-network
      
  seq:
    image: datalust/seq:latest
    container_name: orderflow-seq
    environment:
      ACCEPT_EULA: "Y"
      SEQ_FIRSTRUN_NOAUTHENTICATION: "true"
    ports:
      - "5341:5341"
      - "8081:80"
    volumes:
      - seq_data:/data
    networks:
      - orderflow-network
 
  prometheus:
    image: prom/prometheus:latest
    container_name: orderflow-prometheus
    ports:
      - "9090:9090"
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml
      - prometheus_data:/prometheus
    depends_on:
      - orderflow-api
    networks:
      - orderflow-network

  grafana:
    image: grafana/grafana:latest
    container_name: orderflow-grafana
    ports:
      - "3000:3000"
    volumes:
      - grafana_data:/var/lib/grafana
    depends_on:
      - prometheus
    networks:
      - orderflow-network

volumes:
  sqlserver_data:
  redis_data:
  rabbitmq_data:
  kafka_data:
  seq_data:
  dataprotection_keys:
  prometheus_data:
  grafana_data:

networks:
  orderflow-network:
    driver: bridge
~~~~

### OrderFlow.Api\appsettings.Development.json

~~~~json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
~~~~

### OrderFlow.Api\appsettings.json

~~~~json
{
  //"ConnectionStrings": {
  //  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OrderFlowDb;Trusted_Connection=True;TrustServerCertificate=True"
  //},

  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OrderFlowDb;User Id=***REDACTED***;Password=***REDACTED***;TrustServerCertificate=True"
  },

  "Jwt": {
    "Issuer": "OrderFlow",
    "Audience": "OrderFlow.Api",
    "SecretKey": "MINHA_CHAVE_SUPER_SECRETA_COM_MAIS_DE_32_CARACTERES"
  },

  "RabbitMq": {
    "HostName": "localhost",
    "QueueName": "order-created",
    "DeadLetterQueueName": "order-created-dlq",
    "MaxRetryCount": 3
  },

  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "OrderCompletedTopic": "order-completed",
    "OrderStatusChangedTopic": "order-status-changed"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://seq:5341"
        }
      }
    ],
    "Enrich": [ "FromLogContext" ]
  },

  "Redis": {
    "ConnectionString": "localhost:6379",
    "OrderCacheExpirationMinutes": 5
  },

  "AllowedHosts": "*"
}
~~~~

### OrderFlow.Api\Dockerfile

~~~~text
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OrderFlow.Api/OrderFlow.Api.csproj", "OrderFlow.Api/"]
COPY ["OrderFlow.Application/OrderFlow.Application.csproj", "OrderFlow.Application/"]
COPY ["OrderFlow.Domain/OrderFlow.Domain.csproj", "OrderFlow.Domain/"]
COPY ["OrderFlow.Infrastructure/OrderFlow.Infrastructure.csproj", "OrderFlow.Infrastructure/"]

RUN dotnet restore "OrderFlow.Api/OrderFlow.Api.csproj"

COPY . .

RUN dotnet publish "OrderFlow.Api/OrderFlow.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderFlow.Api.dll"]
~~~~

### OrderFlow.Api\Program.cs

~~~~cs

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
            .AddSqlClientInstrumentation()
            .AddSource("OrderFlow")
            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddMeter("OrderFlow")
            .AddPrometheusExporter()
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

app.MapPrometheusScrapingEndpoint();

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
~~~~

### OrderFlow.Grpc.Client\Program.cs

~~~~cs
using Grpc.Core;
using Grpc.Net.Client;
using OrderFlow.Grpc.Protos;

Console.Write("Informe o OrderId: ");
var orderId = Console.ReadLine();

// using var channel = GrpcChannel.ForAddress("https://localhost:7001");

using var channel = GrpcChannel.ForAddress("http://localhost:5001");

var client = new OrderQuery.OrderQueryClient(channel);

using var call = client.WatchOrderStatus(new WatchOrderStatusRequest
{
    OrderId = orderId
});

Console.WriteLine("Aguardando atualizações de status...");

await foreach (var update in call.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"{update.UpdatedAt} | OrderId: {update.OrderId} | Status: {update.Status}");
}

var response = await client.GetOrderByIdAsync(new GetOrderByIdRequest
{
    OrderId = orderId
});

Console.WriteLine($"OrderId: {response.OrderId}");
Console.WriteLine($"UserId: {response.UserId}");
Console.WriteLine($"Amount: {response.Amount}");
Console.WriteLine($"Status: {response.Status}");
Console.WriteLine($"Type: {response.Type}");
Console.WriteLine($"Priority: {response.Priority}");
Console.WriteLine($"ExternalReference: {response.ExternalReference}");
Console.WriteLine($"CreatedAt: {response.CreatedAt}");
Console.WriteLine("Eventos:");

foreach (var ev in response.Events)
{
    Console.WriteLine($"- {ev.Type} | {ev.Description} | {ev.CreatedAt}");
}
~~~~

### OrderFlow.Grpc\appsettings.Development.json

~~~~json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
~~~~

### OrderFlow.Grpc\appsettings.json

~~~~json
{
  //"ConnectionStrings": {
  //  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OrderFlowDb;Trusted_Connection=True;TrustServerCertificate=True"
  //},

  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OrderFlowDb;User Id=***REDACTED***;Password=***REDACTED***;TrustServerCertificate=True"
  },

  "RabbitMq": {
    "HostName": "localhost",
    "QueueName": "order-created",
    "DeadLetterQueueName": "order-created-dlq",
    "MaxRetryCount": 3
  },

  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "OrderCompletedTopic": "order-completed"
  },

  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    }
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "AllowedHosts": "*",

  "Kestrel": {
    "EndpointDefaults": {
      "Protocols": "Http2"
    }
  },

  "Redis": {
    "ConnectionString": "localhost:6379",
    "OrderCacheExpirationMinutes": 5
  }
}
~~~~

### OrderFlow.Grpc\Dockerfile

~~~~text
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OrderFlow.Grpc/OrderFlow.Grpc.csproj", "OrderFlow.Grpc/"]
COPY ["OrderFlow.Application/OrderFlow.Application.csproj", "OrderFlow.Application/"]
COPY ["OrderFlow.Domain/OrderFlow.Domain.csproj", "OrderFlow.Domain/"]
COPY ["OrderFlow.Infrastructure/OrderFlow.Infrastructure.csproj", "OrderFlow.Infrastructure/"]

RUN dotnet restore "OrderFlow.Grpc/OrderFlow.Grpc.csproj"

COPY . .

RUN dotnet publish "OrderFlow.Grpc/OrderFlow.Grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderFlow.Grpc.dll"]
~~~~

### OrderFlow.Grpc\Program.cs

~~~~cs
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
~~~~

### OrderFlow.Simulator\appsettings.json

~~~~json
{
	"Simulator": {
		"ApiBaseUrl": "http://localhost:5000",
		"BearerToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjcyZTlmOTNlLTcyY2YtNDc1Yy1iZDQxLTA3YzdjMTlhZGI3YiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJkZXZlbG9wZXIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUcmFkZXIiLCJleHAiOjE3ODMxMTA3NTYsImlzcyI6Ik9yZGVyRmxvdyIsImF1ZCI6Ik9yZGVyRmxvdy5BcGkifQ.J-liPojf63CTuXoCa5GYJ4CmZqGEkeNnN29sdHa3-bU",
		"TotalOrders": 5000,
		"Concurrency": 50,
		"DelayBetweenBatchesMilliseconds": 500,
		"Scenario": "Default"
	}
}
~~~~

### OrderFlow.Simulator\Program.cs

~~~~cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OrderFlow.Simulator.Configuration;
using OrderFlow.Simulator.Scenarios;
using OrderFlow.Simulator.Services;

var host = Host.CreateDefaultBuilder(args)
    .UseContentRoot(AppContext.BaseDirectory)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<SimulatorSettings>(
            context.Configuration.GetSection("Simulator"));

        services.AddHttpClient("OrderFlowApi", (serviceProvider, client) =>
        {
            var settings = serviceProvider
                .GetRequiredService<IOptions<SimulatorSettings>>()
                .Value;

            client.BaseAddress = new Uri(settings.ApiBaseUrl);
        });

        services.AddSingleton<OrderGenerator>();
        services.AddSingleton<OrderSender>();
        services.AddSingleton<ISimulationRunner, SimulationRunner>();
        services.AddHostedService<SimulationHostedService>();
        services.AddSingleton<SimulationStatistics>();
        services.AddSingleton<ProgressRenderer>();
        services.AddSingleton<ISimulationScenario, DefaultSimulationScenario>();
        services.AddSingleton<ISimulationScenario, MarketOpenSimulationScenario>();
        services.AddSingleton<SimulationScenarioResolver>();
    })
    .Build();

await host.RunAsync();

~~~~

### OrderFlow.Worker\appsettings.Development.json

~~~~json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
~~~~

### OrderFlow.Worker\appsettings.json

~~~~json
{
  //"ConnectionStrings": {
  //  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OrderFlowDb;Trusted_Connection=True;TrustServerCertificate=True"
  //},

  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OrderFlowDb;User Id=***REDACTED***;Password=***REDACTED***;TrustServerCertificate=True"
  },

  //"Messaging": {
  //  "Provider": "RabbitMq"
  //},

  "Messaging": {
    "Provider": "Sqs"
  },

  "RabbitMq": {
    "HostName": "localhost",
    "QueueName": "order-created",
    "DeadLetterQueueName": "order-created-dlq",
    "MaxRetryCount": 3
  },

  "Sqs": {
    "Enabled": true,
    "Region": "sa-east-1",
    "QueueUrl": "https://sqs.sa-east-1.amazonaws.com/093529868676/order-created",
    "DeadLetterQueueUrl": "",
    "MaxMessages": 5,
    "WaitTimeSeconds": 10
  },

  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "OrderCompletedTopic": "order-completed",
    "OrderStatusChangedTopic": "order-status-changed"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.Hosting.Lifetime": "Warning"
    }
  },

  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "Microsoft.EntityFrameworkCore.Database.Command": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://seq:5341"
        }
      }
    ],
    "Enrich": [ "FromLogContext" ]
  },

  "Redis": {
    "ConnectionString": "localhost:6379",
    "OrderCacheExpirationMinutes": 5
  },

  "OpenTelemetry": {
    "EnableConsoleMetrics": false
  }


}
~~~~

### OrderFlow.Worker\Dockerfile

~~~~text
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OrderFlow.Worker/OrderFlow.Worker.csproj", "OrderFlow.Worker/"]
COPY ["OrderFlow.Application/OrderFlow.Application.csproj", "OrderFlow.Application/"]
COPY ["OrderFlow.Domain/OrderFlow.Domain.csproj", "OrderFlow.Domain/"]
COPY ["OrderFlow.Infrastructure/OrderFlow.Infrastructure.csproj", "OrderFlow.Infrastructure/"]

RUN dotnet restore "OrderFlow.Worker/OrderFlow.Worker.csproj"

COPY . .

RUN dotnet publish "OrderFlow.Worker/OrderFlow.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderFlow.Worker.dll"]
~~~~

### OrderFlow.Worker\Program.cs

~~~~cs
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
        metrics.AddMeter("OrderFlow");

        if (builder.Configuration.GetValue<bool>("OpenTelemetry:EnableConsoleMetrics"))
        {
            metrics.AddConsoleExporter();
        }
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
    builder.Services.AddHostedService<KafkaOrderStatusChangedAuditWorker>();

    builder.Services.AddScoped<IOrderProcessingStrategy, BuyOrderProcessingStrategy>();
    builder.Services.AddScoped<IOrderProcessingStrategy, SellOrderProcessingStrategy>();
    builder.Services.AddScoped<IOrderProcessingStrategy, TransferOrderProcessingStrategy>();

    builder.Services.AddScoped<IOrderProcessingStrategyResolver, OrderProcessingStrategyResolver>();

    builder.Services.AddScoped<IOrderEventPublisher, KafkaOrderEventPublisher>();

    builder.Services.AddScoped<IOrderAuditReadModelRepository, OrderAuditReadModelRepository>();

    var host = builder.Build();
    host.Run();
~~~~

## Controllers

- OrderFlow.Api\Controllers\AuthController.cs
- OrderFlow.Api\Controllers\OrderAuditController.cs
- OrderFlow.Api\Controllers\OrdersController.cs

## Workers e Background Services

- OrderFlow.Worker\KafkaOrderStatusChangedAuditWorker.cs
- OrderFlow.Worker\OutboxPublisherWorker.cs
- OrderFlow.Worker\SqsWorker.cs
- OrderFlow.Worker\Worker.cs

## Use Cases

- OrderFlow.Application\Interfaces\ICreateOrderUseCase.cs
- OrderFlow.Application\Interfaces\IGetOrderAuditUseCase.cs
- OrderFlow.Application\Interfaces\IGetOrderByIdUseCase.cs
- OrderFlow.Application\Interfaces\IProcessOrderUseCase.cs
- OrderFlow.Application\Interfaces\IPublishOutboxMessagesUseCase.cs
- OrderFlow.Application\UseCases\CreateOrderUseCase.cs
- OrderFlow.Application\UseCases\GetOrderAuditUseCase.cs
- OrderFlow.Application\UseCases\GetOrderByIdUseCase.cs
- OrderFlow.Application\UseCases\ProcessOrderUseCase.cs
- OrderFlow.Application\UseCases\PublishOutboxMessagesUseCase.cs

## Consumers

_Nenhum arquivo encontrado pelo padrao *Consumer.cs._

## Publishers

- OrderFlow.Application\Interfaces\IIntegrationMessagePublisher.cs
- OrderFlow.Application\Interfaces\IOrderEventPublisher.cs
- OrderFlow.Application\Interfaces\IOrderMessagePublisher.cs
- OrderFlow.Infrastructure\Messaging\KafkaOrderEventPublisher.cs
- OrderFlow.Infrastructure\Messaging\RabbitMqIntegrationMessagePublisher.cs
- OrderFlow.Infrastructure\Messaging\RabbitMqOrderMessagePublisher.cs
- OrderFlow.Infrastructure\Messaging\SqsIntegrationMessagePublisher.cs

## Repositories

- OrderFlow.Domain\Interfaces\IOrderAuditReadModelRepository.cs
- OrderFlow.Domain\Interfaces\IOrderRepository.cs
- OrderFlow.Domain\Interfaces\IOutboxMessageRepository.cs
- OrderFlow.Infrastructure\Repositories\InMemoryOrderRepository.cs
- OrderFlow.Infrastructure\Repositories\OrderAuditReadModelRepository.cs
- OrderFlow.Infrastructure\Repositories\OrderRepository.cs
- OrderFlow.Infrastructure\Repositories\OutboxMessageRepository.cs

## Health Checks

- OrderFlow.Infrastructure\HealthChecks\KafkaHealthCheck.cs
- OrderFlow.Infrastructure\HealthChecks\RabbitMqHealthCheck.cs

## Indicadores de observabilidade encontrados no codigo


### Serilog

- OrderFlow.Api\Middlewares\UserContextLoggingMiddleware.cs - linha 1
- OrderFlow.Api\appsettings.json - linha 36
- OrderFlow.Api\Program.cs - linha 24
- OrderFlow.Api\Program.cs - linha 77
- OrderFlow.Grpc\appsettings.json - linha 22
- OrderFlow.Worker\appsettings.json - linha 47
- OrderFlow.Worker\Program.cs - linha 16
- OrderFlow.Worker\Program.cs - linha 22
- docker-compose.yml - linha 87
- docker-compose.yml - linha 88
- docker-compose.yml - linha 113
- docker-compose.yml - linha 114
- docker-compose.yml - linha 138
- docker-compose.yml - linha 139

### OpenTelemetry

- OrderFlow.Api\Program.cs - linha 8
- OrderFlow.Api\Program.cs - linha 9
- OrderFlow.Api\Program.cs - linha 142
- OrderFlow.Worker\appsettings.json - linha 74
- OrderFlow.Worker\Program.cs - linha 2
- OrderFlow.Worker\Program.cs - linha 3
- OrderFlow.Worker\Program.cs - linha 54
- OrderFlow.Worker\Program.cs - linha 62
- OrderFlow.Worker\Program.cs - linha 67

### AddOpenTelemetry

- OrderFlow.Api\Program.cs - linha 142
- OrderFlow.Worker\Program.cs - linha 54
- OrderFlow.Worker\Program.cs - linha 62

### HealthChecks

- OrderFlow.Api\Program.cs - linha 3
- OrderFlow.Api\Program.cs - linha 5
- OrderFlow.Api\Program.cs - linha 20
- OrderFlow.Api\Program.cs - linha 166
- OrderFlow.Api\Program.cs - linha 226
- OrderFlow.Api\Program.cs - linha 231
- OrderFlow.Infrastructure\HealthChecks\KafkaHealthCheck.cs - linha 2
- OrderFlow.Infrastructure\HealthChecks\KafkaHealthCheck.cs - linha 5
- OrderFlow.Infrastructure\HealthChecks\RabbitMqHealthCheck.cs - linha 1
- OrderFlow.Infrastructure\HealthChecks\RabbitMqHealthCheck.cs - linha 5

### MapHealthChecks

- OrderFlow.Api\Program.cs - linha 226
- OrderFlow.Api\Program.cs - linha 231

### Prometheus

- OrderFlow.Api\Program.cs - linha 158
- OrderFlow.Api\Program.cs - linha 222
- docker-compose.yml - linha 168
- docker-compose.yml - linha 169
- docker-compose.yml - linha 170
- docker-compose.yml - linha 174
- docker-compose.yml - linha 175
- docker-compose.yml - linha 189
- docker-compose.yml - linha 200

### Grafana

- docker-compose.yml - linha 181
- docker-compose.yml - linha 182
- docker-compose.yml - linha 183
- docker-compose.yml - linha 187
- docker-compose.yml - linha 201

### CorrelationId

- OrderFlow.Api\Middlewares\CorrelationIdMiddleware.cs - linha 5
- OrderFlow.Api\Middlewares\CorrelationIdMiddleware.cs - linha 10
- OrderFlow.Api\Middlewares\CorrelationIdMiddleware.cs - linha 12
- OrderFlow.Api\Middlewares\CorrelationIdMiddleware.cs - linha 20
- OrderFlow.Api\Middlewares\CorrelationIdMiddleware.cs - linha 22
- OrderFlow.Api\Middlewares\CorrelationIdMiddleware.cs - linha 23
- OrderFlow.Api\Middlewares\CorrelationIdMiddleware.cs - linha 25
- OrderFlow.Api\Middlewares\CorrelationIdMiddleware.cs - linha 27
- OrderFlow.Api\Middlewares\CorrelationIdMiddleware.cs - linha 29
- OrderFlow.Api\Middlewares\CorrelationIdMiddleware.cs - linha 33
- OrderFlow.Api\Program.cs - linha 214
- OrderFlow.Application\Dtos\GetOrderAuditResponse.cs - linha 13
- OrderFlow.Application\Interfaces\IIntegrationMessagePublisher.cs - linha 6
- OrderFlow.Application\Messaging\OrderCompletedIntegrationEvent.cs - linha 9
- OrderFlow.Application\Messaging\OrderStatusChangedIntegrationEvent.cs - linha 19
- OrderFlow.Application\Observability\LogProperties.cs - linha 5
- OrderFlow.Application\UseCases\CreateOrderUseCase.cs - linha 52
- OrderFlow.Application\UseCases\CreateOrderUseCase.cs - linha 70
- OrderFlow.Application\UseCases\CreateOrderUseCase.cs - linha 92
- OrderFlow.Application\UseCases\CreateOrderUseCase.cs - linha 93

### ActivitySource

- OrderFlow.Application\Observability\Telemetry.cs - linha 7
- OrderFlow.Application\UseCases\CreateOrderUseCase.cs - linha 60
- OrderFlow.Application\UseCases\ProcessOrderUseCase.cs - linha 47
- OrderFlow.Infrastructure\Cache\RedisOrderCacheService.cs - linha 36
- OrderFlow.Infrastructure\Cache\RedisOrderCacheService.cs - linha 74
- OrderFlow.Infrastructure\Cache\RedisOrderCacheService.cs - linha 101
- OrderFlow.Worker\OutboxPublisherWorker.cs - linha 26
- OrderFlow.Worker\SqsWorker.cs - linha 78
- OrderFlow.Worker\Worker.cs - linha 91

### Meter

- OrderFlow.Api\Program.cs - linha 43
- OrderFlow.Api\Program.cs - linha 110
- OrderFlow.Api\Program.cs - linha 157
- OrderFlow.Application\Observability\Metrics.cs - linha 7
- OrderFlow.Application\Observability\Metrics.cs - linha 10
- OrderFlow.Application\Observability\Metrics.cs - linha 13
- OrderFlow.Application\Observability\Metrics.cs - linha 16
- OrderFlow.Application\Observability\Metrics.cs - linha 19
- OrderFlow.Application\Observability\Metrics.cs - linha 22
- OrderFlow.Application\Observability\Metrics.cs - linha 25
- OrderFlow.Application\Observability\Metrics.cs - linha 28
- OrderFlow.Application\Observability\Metrics.cs - linha 31
- OrderFlow.Worker\Program.cs - linha 65

### UseSerilog

- OrderFlow.Api\Program.cs - linha 77

### Tempo

- OrderFlow.Simulator\Services\ProgressRenderer.cs - linha 47
- OrderFlow.Simulator\Services\ProgressRenderer.cs - linha 48

## Git

- Branch atual: feature/grpc
- Ultimo commit: 74d843c - refactor(testcontainers): unify integration infrastructure with SQL Server, Redis, RabbitMQ and Kafka (2026-07-01)

## Checklist de revisao manual

- [ ] Confirmar se nenhum segredo permaneceu no arquivo.
- [ ] Confirmar se a solucao e os projetos listados estao corretos.
- [ ] Confirmar se Docker, workflows e configuracoes representam o estado atual.
- [ ] Confirmar se os componentes de observabilidade foram detectados.
- [ ] Enviar este arquivo para revisao arquitetural e documental.
