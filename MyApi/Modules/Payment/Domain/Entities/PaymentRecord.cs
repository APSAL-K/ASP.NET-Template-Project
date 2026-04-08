namespace MyApi.Modules.Payment.Domain.Entities;

public class PaymentRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string CustomerReference { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RefundedAt { get; set; }
}