using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.Java;
using System.Text;

namespace OrderFlow.Domain.Tests.Entities
{
    public class OrderTests
    {
        [Fact]
        public void Should_Not_Move_From_Processing_To_Processing()
        { 
            // Arrange
            var order = CreateValidOrder();

            // Act
            order.MarkAsProcessing(100M);

            var exception = Assert.Throws<InvalidOperationException>(() => order.MarkAsProcessing(100m));

            // Assert
            Assert.Equal("Somente ordens criadas podem ir para processamento.", exception.Message);

            Assert.Equal(OrderStatus.Processing, order.Status);

        }

        [Fact]
        public void Should_Add_Completed_Event_When_Order_Is_Completed()
        { 
            // Arrange
            var order = CreateValidOrder();

            // Act
            order.MarkAsProcessing(100m);
            order.MarkAsCompleted(100m);

            // Assert
            Assert.Contains(order.Events, e => e.Type == OrderEventType.Completed);

            // ou Assert : assim bem explicito 

            Assert.Equal(3, order.Events.Count);

            var completedEvent = order.Events.Last();

            Assert.Equal(OrderEventType.Completed, completedEvent.Type);

        }

        [Fact]
        public void Should_Add_Processing_Event_When_Order_Starts_Processing()
        { 
            // Arrange 
            var userId = Guid.NewGuid();

            // Act
            var order = CreateValidOrder();
            order.MarkAsProcessing(100m);

            // Assert
            Assert.Contains(
                order.Events,
                e => e.Type == OrderEventType.Processing);

            // ou Assert : assim bem explicito 

            Assert.Equal(2, order.Events.Count);

            var processingEvent = order.Events.Last();

            Assert.Equal(OrderEventType.Processing, processingEvent.Type);

        }

        [Fact]
        public void Should_Throw_When_ExternalReference_Is_Empty()
        { 
            // Arrange
            var externalReferencia = string.Empty;

            // Act
            var exception = Assert.Throws<ArgumentException>(() => new Order(
                Guid.NewGuid(),
                100m,
                OrderType.Buy,
                OrderPriority.Normal,
                externalReferencia,
                "PETR4",
                10,
                10m,
                null,
                null));

            // Assert
            Assert.Equal(
                "ExternalReference deve ser informado.",
                exception.Message);
        }

        [Fact]
        public void Should_Throw_When_UserId_Is_Empty()
        {
            // Arrange
            var userId = Guid.Empty;

            // Act
            var exception = Assert.Throws<ArgumentException>(() =>  new Order(
                userId,
                100m,
                OrderType.Buy,
                OrderPriority.Normal,
                "EXT-001",
                "PETR4",
                10,
                10m,
                null,
                null));

            // Assert
            Assert.Equal("UserId inválido.", exception.Message);

        }

        [Fact]
        public void Should_Not_Fail_When_Status_Is_Completed()
        {
            // Arrange

            var order = CreateValidOrder();
            order.MarkAsProcessing(100m);
            order.MarkAsCompleted(100m);

            // Act

            var exception = Assert.Throws<InvalidOperationException>(() =>
                order.MarkAsFailed("Falha no teste"));

            // Assert
            Assert.Equal("Somente ordens criadas ou em processamento podem falhar.", exception.Message);
            Assert.Equal(OrderStatus.Completed, order.Status);
        }

        [Fact]
        public void Should_Move_From_Created_To_Failed()
        {
            // Arrange
            var order = CreateValidOrder();

            // Act
            order.MarkAsFailed("Falha no teste");

            // Assert
            Assert.Equal(OrderStatus.Failed, order.Status);

            Assert.Equal(2, order.Events.Count);

            Assert.Contains(
                order.Events,
                e => e.Type == OrderEventType.Failed);
        }

        [Fact]
        public void Should_Not_Complete_When_Status_Is_Created()
        {
            // Arrange
            var order = CreateValidOrder();

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
                order.MarkAsCompleted(100m));

            // Assert
            Assert.Equal(
                "Somente ordens em processamento podem ser concluídas.",
                exception.Message);

            Assert.Equal(OrderStatus.Created, order.Status);
        }

        [Fact]
        public void Should_Move_From_Processing_To_Completed()
        {
            // Arrange
            var order = CreateValidOrder();
            order.MarkAsProcessing(100m);

            // Act
            order.MarkAsCompleted(100m);

            // Assert
            Assert.Equal(OrderStatus.Completed, order.Status);

            Assert.Equal(3, order.Events.Count);
            Assert.Contains(order.Events, e => e.Type == OrderEventType.Completed);
        }

        [Fact]
        public void Should_Move_From_Created_To_Processing()
        {
            // Arrange
            var order = CreateValidOrder();

            // Act

            order.MarkAsProcessing(100m);

            // Assert

            Assert.Equal(OrderStatus.Processing, order.Status);
            Assert.Equal(2, order.Events.Count);
            Assert.Contains(order.Events, x => x.Type == OrderEventType.Processing);
        }

        [Fact]
        public void Should_Throw_When_Amount_Is_Zero()
        {
            // Arrange

            var amount = 0m;

            // Act
            var exception = Assert.Throws<ArgumentException>(() =>
                                new Order(
                                Guid.NewGuid(),
                                amount,
                                OrderType.Sell,
                                OrderPriority.High,
                                "EXT-003",
                                "PETR5",
                                5,
                                50m,
                                "Cta1",
                                "Cta2"));

            // Assert
            Assert.Equal("O valor da ordem deve ser maior que zero.", exception.Message);
        }

        private static Order CreateValidOrder()
        {
            return new Order(
                Guid.NewGuid(),
                100m,
                OrderType.Buy,
                OrderPriority.Normal,
                "EXT-001",
                "PETR4",
                10,
                10m,
                null,
                null);
        }

        [Fact]
        public void Should_Add_Created_Event_When_Order_Is_Created()
        {

            //  Quando uma Order nasce
            //  então deve ser registrado um evento Created

            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var order = new Order(
                userId,
                100m,
                OrderType.Buy,
                OrderPriority.Normal,
                "EXT-001",
                "PETR4",
                10,
                10m,
                null,
                null);

            // Assert
            Assert.Single(order.Events);

            var createdEvent = order.Events.First();

            Assert.Equal(OrderEventType.Created, createdEvent.Type);

        }

        [Fact]
        public void Should_Create_Order_With_Created_Status()
        {
            //   Quando uma Order nasce
            //   então seu Status deve ser Created

            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var order = new Order(
                userId,
                100m,
                OrderType.Buy,
                OrderPriority.Normal,
                "EXT-001",
                "PETR4",
                10,
                10m,
                null,
                null);

            // Assert
            Assert.Equal(OrderStatus.Created, order.Status);

        }
    }
}
