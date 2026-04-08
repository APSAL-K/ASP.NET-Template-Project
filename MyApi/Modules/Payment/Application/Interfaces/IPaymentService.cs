using MyApi.Modules.Payment.Application.DTOs;

namespace MyApi.Modules.Payment.Application.Interfaces;

public interface IPaymentService
{
    Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync();
    Task<PaymentDto?> GetPaymentAsync(Guid paymentId);
    Task<PaymentDto> ChargeAsync(CreatePaymentRequestDto request);
    Task<PaymentDto> RefundAsync(Guid paymentId, RefundPaymentRequestDto request);
}