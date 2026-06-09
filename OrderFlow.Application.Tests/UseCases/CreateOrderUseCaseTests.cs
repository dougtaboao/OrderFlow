using Microsoft.Extensions.Logging;
using Moq;
using OrderFlow.Application.Dtos;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;
using OrderFlow.Application.Observability;
using OrderFlow.Application.Security;
using OrderFlow.Application.UseCases;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Application.Tests.UseCases
{
    public class CreateOrderUseCaseTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
        private readonly Mock<IOutboxMessageRepository> _outboxRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ICorrelationContext> _correlationContextMock = new();
        private readonly Mock<ILogger<CreateOrderUseCase>> _loggerMock = new();
        private readonly Mock<ICreateOrderValidator> _validatorMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();

        private CreateOrderUseCase CreateUseCase()
        {
            return new CreateOrderUseCase(
                _orderRepositoryMock.Object,
                _outboxRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _correlationContextMock.Object,
                _loggerMock.Object,
                _validatorMock.Object,
                _currentUserMock.Object);
        }

        private void SetupAuthenticatedUser(Guid? userId = null)
        {
            _currentUserMock
                .Setup(x => x.IsAuthenticated)
                .Returns(true);

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns(userId ?? Guid.NewGuid());
        }

        private void SetupCorrelationId(string correlationId = "correlation-test-001")
        {
            _correlationContextMock
                .Setup(x => x.CorrelationId)
                .Returns(correlationId);
        }

        private void SetupUnitOfWorkSuccess()
        {
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
        }

        private static CreateOrderRequest CreateValidRequest()
        {
            return new CreateOrderRequest
            {
                Amount = 100m,
                Type = OrderType.Buy,
                Priority = OrderPriority.Normal,
                ExternalReference = "EXT-001",
                AssetCode = "PETR4",
                Quantity = 10,
                UnitPrice = 10m,
                SourceAccount = null,
                DestinationAccount = null
            };
        }

        [Fact]
        public async Task ExecuteAsync_Should_Create_Order_And_Outbox_When_User_Is_Authenticated()
        {
            var userId = Guid.NewGuid();

            // Arrange
            SetupAuthenticatedUser(userId);
            SetupCorrelationId();
            SetupUnitOfWorkSuccess();
                
            var useCase = CreateUseCase();
            var request = CreateValidRequest();

            // Act
            var response = await useCase.ExecuteAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, response.OrderId);
            Assert.Equal("Created", response.Status);

            _validatorMock.Verify(
                x => x.Validate(request),
                Times.Once);

            _orderRepositoryMock.Verify(
                x => x.AddAsync(
                    It.Is<Order>(order =>
                        order.UserId == userId &&
                        order.Amount == request.Amount &&
                        order.Type == request.Type &&
                        order.Priority == request.Priority &&
                        order.ExternalReference == request.ExternalReference),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _outboxRepositoryMock.Verify(
                x => x.AddAsync(
                    It.Is<OutboxMessage>(outbox =>
                        outbox.Type == nameof(OrderCreatedMessage) &&
                        outbox.CorrelationId == "correlation-test-001"),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Throw_When_User_Is_Not_Authenticated()
        {
            // Arrange
            // SetupAuthenticatedUser();
            SetupCorrelationId();
            SetupUnitOfWorkSuccess();

            var useCase = CreateUseCase();
            var request = CreateValidRequest();

            _currentUserMock
                .Setup(x => x.IsAuthenticated)
                .Returns(false);

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns(Guid.Empty);

            // Act
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.ExecuteAsync(request));

            // Assert
            Assert.Equal("Usuário autenticado inválido.", exception.Message);

            _validatorMock.Verify(
                x => x.Validate(It.IsAny<CreateOrderRequest>()),
                Times.Never);

            _orderRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Order>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _outboxRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<OutboxMessage>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Throw_When_SaveChanges_Fails()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Arrange
            SetupAuthenticatedUser();
            SetupCorrelationId();
            SetupUnitOfWorkSuccess();

            var useCase = CreateUseCase();
            var request = CreateValidRequest();

            _correlationContextMock
                .Setup(x => x.CorrelationId)
                .Returns("correlation-test-001");

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro SQL"));


            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                useCase.ExecuteAsync(request));

            // Assert
            Assert.Contains("Erro ao salvar Order + Outbox", exception.Message);
            Assert.Contains("Erro SQL", exception.Message);

            _orderRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _outboxRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Not_Save_When_Validator_Throws()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Arrange
            SetupAuthenticatedUser();
            SetupCorrelationId();
            SetupUnitOfWorkSuccess();

            var useCase = CreateUseCase();
            var request = CreateValidRequest();

            _validatorMock
                .Setup(x => x.Validate(It.IsAny<CreateOrderRequest>()))
                .Throws(new ValidationException("Request inválido"));

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                useCase.ExecuteAsync(request));

            // Assert
            Assert.Equal("Request inválido", exception.Message);

            _orderRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _outboxRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Throw_When_UserId_Is_Empty()
        {
            // Arrange
            // Arrange
            SetupAuthenticatedUser();
            SetupCorrelationId();
            SetupUnitOfWorkSuccess();

            var useCase = CreateUseCase();
            var request = CreateValidRequest();

            _currentUserMock.Setup(x => x.UserId).Returns(Guid.Empty);

            // Act
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.ExecuteAsync(request));

            // Assert
            Assert.Equal("Usuário autenticado inválido.", exception.Message);

            _validatorMock.Verify(
                x => x.Validate(It.IsAny<CreateOrderRequest>()),
                Times.Never);

            _orderRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _outboxRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}