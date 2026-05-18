using Grpc.Core;
using Grpc.Net.Client;
using OrderFlow.Grpc.Protos;

Console.Write("Informe o OrderId: ");
var orderId = Console.ReadLine();

using var channel = GrpcChannel.ForAddress("https://localhost:7001");

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