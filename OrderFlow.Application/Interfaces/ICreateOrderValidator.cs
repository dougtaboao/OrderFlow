using OrderFlow.Application.Dtos;

namespace OrderFlow.Application.Interfaces
{
    public interface ICreateOrderValidator
    {
        void Validate(CreateOrderRequest request);
    }
}