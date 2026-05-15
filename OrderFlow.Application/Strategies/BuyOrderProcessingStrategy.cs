using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Services.Orders;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Strategies
{
    public class BuyOrderProcessingStrategy : IOrderProcessingStrategy
    {
        private IBuyOrderService _buyOrderService;

        public BuyOrderProcessingStrategy(IBuyOrderService buyOrderService)
        {
            _buyOrderService = buyOrderService;
        }
        public OrderType Type => OrderType.Buy;

        public async Task ProcessAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _buyOrderService.ValidateAsync(order, cancellationToken);
            await _buyOrderService.ExecuteAsync(order, cancellationToken);
        }
    }
}