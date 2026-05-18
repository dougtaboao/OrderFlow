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

            var response = new GetOrderByIdResponse
            {
                OrderId = order.OrderId.ToString(),
                UserId = order.UserId.ToString(),
                Amount = Convert.ToDouble(order.Amount),
                Status = order.Status,
                Type = order.Type,
                Priority = order.Priority,
                ExternalReference = order.ExternalReference,
                AssetCode = order.AssetCode ?? string.Empty,
                Quantity = order.Quantity ?? 0,
                UnitPrice = Convert.ToDouble(order.UnitPrice ?? 0),
                SourceAccount = order.SourceAccount ?? string.Empty,
                DestinationAccount = order.DestinationAccount ?? string.Empty,
                CreatedAt = order.CreatedAt.ToString("O")
            };

            response.Events.AddRange(order.Events.Select(e => new OrderEventResponse
            {
                Type = e.Type,
                Description = e.Description,
                CreatedAt = e.CreatedAt.ToString("O")
            }));

            return response;
        }

        public override async Task WatchOrderStatus(
    WatchOrderStatusRequest request,
    IServerStreamWriter<OrderStatusUpdate> responseStream,
    ServerCallContext context)
        {
            if (!Guid.TryParse(request.OrderId, out var orderId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "OrderId inválido."));

            _logger.LogInformation(
                "Iniciando stream de status para OrderId {OrderId}",
                orderId);

            string? lastStatus = null;

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var order = await _getOrderByIdUseCase.ExecuteAsync(
                    orderId,
                    context.CancellationToken);

                if (order is null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Ordem não encontrada."));

                if (order.Status != lastStatus)
                {
                    lastStatus = order.Status;

                    await responseStream.WriteAsync(new OrderStatusUpdate
                    {
                        OrderId = order.OrderId.ToString(),
                        Status = order.Status,
                        UpdatedAt = DateTime.UtcNow.ToString("O")
                    });

                    _logger.LogInformation(
                        "Status enviado via stream gRPC. OrderId {OrderId}, Status {Status}",
                        orderId,
                        order.Status);
                }

                if (order.Status is "Completed" or "Failed")
                {
                    _logger.LogInformation(
                        "Stream encerrado para OrderId {OrderId}. Status final {Status}",
                        orderId,
                        order.Status);

                    break;
                }

                await Task.Delay(1000, context.CancellationToken);
            }
        }
    }
}