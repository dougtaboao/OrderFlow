using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Interfaces;

namespace OrderFlow.Api.Controllers
{
    [Authorize(Policy = "CanViewOrder")]
    [ApiController]
    [Route("api/order-audit")]
    public class OrderAuditController : ControllerBase
    {
        private readonly IGetOrderAuditUseCase _getOrderAuditUseCase;

        public OrderAuditController(IGetOrderAuditUseCase getOrderAuditUseCase)
        {
            _getOrderAuditUseCase = getOrderAuditUseCase;
        }

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetByOrderId(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            var result = await _getOrderAuditUseCase.ExecuteAsync(
                orderId,
                cancellationToken);

            return Ok(result);
        }
    }
}