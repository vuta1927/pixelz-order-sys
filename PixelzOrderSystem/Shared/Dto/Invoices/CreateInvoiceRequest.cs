namespace PixelzOrderSystem.Shared.Dto.Invoices;

public class CreateInvoiceRequest
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}