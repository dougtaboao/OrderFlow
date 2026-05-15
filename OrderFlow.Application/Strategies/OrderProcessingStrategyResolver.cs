using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Strategies
{
    public class OrderProcessingStrategyResolver : IOrderProcessingStrategyResolver     
    {
        private readonly IEnumerable<IOrderProcessingStrategy> _strategies;

        public OrderProcessingStrategyResolver(IEnumerable<IOrderProcessingStrategy> strategies)
        {
            _strategies = strategies;
        }

        public IOrderProcessingStrategy Resolve(OrderType type)
        {
            var strategy = _strategies.FirstOrDefault(x => x.Type == type);

            if (strategy is null)
                throw new InvalidOperationException($"Nenhuma estratégia encontrada para o tipo de ordem {type}.");

            return strategy;
        }
    }
}