using Microsoft.AspNetCore.Mvc;
using ModuleDrivenFramwork.Modules.Payment.Application.DTOs;
using ModuleDrivenFramwork.Modules.Payment.Application.Interfaces;

namespace ModuleDrivenFramwork.Modules.Payment.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetAll()
    {
        return Ok(await _paymentService.GetPaymentsAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id)
    {
        var payment = await _paymentService.GetPaymentAsync(id);
        return payment == null ? NotFound() : Ok(payment);
    }

    [HttpPost("charge")]
    public async Task<ActionResult<PaymentDto>> Charge([FromBody] CreatePaymentRequestDto request)
    {
        try
        {
            var payment = await _paymentService.ChargeAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost("{id:guid}/refund")]
    public async Task<ActionResult<PaymentDto>> Refund(Guid id, [FromBody] RefundPaymentRequestDto request)
    {
        try
        {
            return Ok(await _paymentService.RefundAsync(id, request));
        }
        catch (InvalidOperationException exception) when (exception.Message == "Payment not found.")
        {
            return NotFound(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}