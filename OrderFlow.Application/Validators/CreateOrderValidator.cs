using OrderFlow.Application.Dtos;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Application.Validators
{
    public class CreateOrderValidator : ICreateOrderValidator
    {
        public void Validate(CreateOrderRequest request)
        {
            // UserId não vem mais do body
            // UserId vem do token JWT validado
            // if (request.UserId == Guid.Empty)
            //    throw new ArgumentException("UserId inválido.");

            if (request.Amount <= 0)
                throw new ArgumentException("Amount deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(request.ExternalReference))
                throw new ArgumentException("ExternalReference deve ser informado.");

            switch (request.Type)
            {
                case OrderType.Buy:
                case OrderType.Sell:
                    ValidateAssetOrder(request);
                    break;

                case OrderType.Transfer:
                    ValidateTransferOrder(request);
                    break;

                default:
                    throw new ArgumentException("Tipo de ordem inválido.");
            }
        }

        private static void ValidateAssetOrder(CreateOrderRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.AssetCode))
                throw new ArgumentException("AssetCode deve ser informado para compra/venda.");

            if (request.Quantity is null or <= 0)
                throw new ArgumentException("Quantity deve ser maior que zero para compra/venda.");

            if (request.UnitPrice is null or <= 0)
                throw new ArgumentException("UnitPrice deve ser maior que zero para compra/venda.");

            var expectedAmount = request.Quantity.Value * request.UnitPrice.Value;

            if (request.Amount != expectedAmount)
                throw new ArgumentException("Amount deve ser igual a Quantity x UnitPrice.");
        }

        private static void ValidateTransferOrder(CreateOrderRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SourceAccount))
                throw new ArgumentException("SourceAccount deve ser informado para transferência.");

            if (string.IsNullOrWhiteSpace(request.DestinationAccount))
                throw new ArgumentException("DestinationAccount deve ser informado para transferência.");

            if (request.SourceAccount == request.DestinationAccount)
                throw new ArgumentException("SourceAccount e DestinationAccount não podem ser iguais.");
        }
    }
}