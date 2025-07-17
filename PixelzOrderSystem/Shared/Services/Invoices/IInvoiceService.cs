using PixelzOrderSystem.Shared.Dto.Invoices;

namespace PixelzOrderSystem.Shared.Services.Invoices;

public interface IInvoiceService
{
    Task<CreateInvoiceResult> CreateInvoiceAsync(CreateInvoiceRequest request);
}