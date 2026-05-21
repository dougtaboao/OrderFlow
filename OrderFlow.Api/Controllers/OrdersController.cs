using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Dtos;
using OrderFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IGetOrderByIdUseCase _getOrderByIdUseCase;
        private readonly ICreateOrderUseCase _createOrderUseCase;

        public OrdersController(ICreateOrderUseCase createOrderUseCase, IGetOrderByIdUseCase getOrderByIdUseCase)
        {
            _getOrderByIdUseCase = getOrderByIdUseCase;
            _createOrderUseCase = createOrderUseCase;
        }

        [Authorize(Policy = "CanCreateOrder")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var response = await _createOrderUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = response.OrderId }, response);
        }

        [Authorize(Policy = "CanViewOrder")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _getOrderByIdUseCase.ExecuteAsync(id, cancellationToken);

            if (response is null)
                return NotFound(new { message = "Ordem não encontrada." });

            return Ok(response);
        }
    }
}