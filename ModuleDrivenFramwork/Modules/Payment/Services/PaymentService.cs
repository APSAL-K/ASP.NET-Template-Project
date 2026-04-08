using System.Collections.Concurrent;
using ModuleDrivenFramwork.Modules.Payment.Application.DTOs;
using ModuleDrivenFramwork.Modules.Payment.Application.Interfaces;
using ModuleDrivenFramwork.Modules.Payment.Domain.Entities;

namespace ModuleDrivenFramwork.Modules.Payment.Services;

public class PaymentService : IPaymentService
{
    private static readonly ConcurrentDictionary<Guid, PaymentRecord> Payments = new();

    public Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync()
    {
        var payments = Payments.Values
            .OrderByDescending(payment => payment.CreatedAt)
            .Select(Map)
            .ToList();

        return Task.FromResult((IReadOnlyList<PaymentDto>)payments);
    }

    public Task<PaymentDto?> GetPaymentAsync(Guid paymentId)
    {
        var found = Payments.TryGetValue(paymentId, out var payment);
        return Task.FromResult(found ? Map(payment!) : null);
    }

    public Task<PaymentDto> ChargeAsync(CreatePaymentRequestDto request)
    {
        if (request.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be greater than zero.");
        }

        var payment = new PaymentRecord
        {
            Amount = request.Amount,
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? "USD" : request.Currency.Trim().ToUpperInvariant(),
            CustomerReference = request.CustomerReference.Trim(),
            Description = request.Description.Trim(),
            Status = "charged",
            CreatedAt = DateTime.UtcNow
        };

        Payments[payment.Id] = payment;
        return Task.FromResult(Map(payment));
    }

    public Task<PaymentDto> RefundAsync(Guid paymentId, RefundPaymentRequestDto request)
    {
        if (!Payments.TryGetValue(paymentId, out var payment))
        {
            throw new InvalidOperationException("Payment not found.");
        }

        if (string.Equals(payment.Status, "refunded", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Payment is already refunded.");
        }

        payment.Status = string.IsNullOrWhiteSpace(request.Reason)
            ? "refunded"
            : $"refunded:{request.Reason.Trim()}";
        payment.RefundedAt = DateTime.UtcNow;

        return Task.FromResult(Map(payment));
    }

    private static PaymentDto Map(PaymentRecord payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            Amount = payment.Amount,
            Currency = payment.Currency,
            CustomerReference = payment.CustomerReference,
            Description = payment.Description,
            Status = payment.Status,
            CreatedAt = payment.CreatedAt,
            RefundedAt = payment.RefundedAt
        };
    }
}