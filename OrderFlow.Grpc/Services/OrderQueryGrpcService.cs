using Grpc.Core;
using OrderFlow.Application.Interfaces;
using OrderFlow.Grpc.Protos;

namespace OrderFlow.Grpc.Services
{
    public class OrderQueryGrpcService : OrderQuery.OrderQueryBase
    {
        private readonly IGetOrderByIdUseCase _getOrderByIdUseCase;
        private readonly ILogger<OrderQueryGrpcService> _logger;

        public OrderQueryGrpcService(
            IGetOrderByIdUseCase getOrderByIdUseCase,
            ILogger<OrderQueryGrpcService> logger)
        {
            _getOrderByIdUseCase = getOrderByIdUseCase;
            _logger = logger;
        }

        public override async Task<GetOrderByIdResponse> GetOrderById(
            GetOrderByIdRequest request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.OrderId, out var orderId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "OrderId inválido."));

            var order = await _getOrderByIdUseCase.ExecuteAsync(orderId, context.CancellationToken);

            if (order is null)
                throw new RpcException(new Status(StatusCode.NotFound, "Ordem não encontrada."));

            _logger.LogInformation(
                "Consulta gRPC realizada para OrderId {OrderId}",
                orderId);

            return new GetOrderByIdResponse
            {
                OrderId = order.OrderId.ToString(),
                UserId = order.UserId.ToString(),
                Amount = Convert.ToDouble(order.Amount),
                Status = order.Status,
                Type = order.Type,
                Priority = order.Priority,
                ExternalReference = order.ExternalReference,
                CreatedAt = order.CreatedAt.ToString("O")
            };
        }
    }
}