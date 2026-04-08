namespace MyApi.Modules.Payment.Application.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string CustomerReference { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? RefundedAt { get; set; }
}

public class CreatePaymentRequestDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string CustomerReference { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class RefundPaymentRequestDto
{
    public string Reason { get; set; } = string.Empty;
}