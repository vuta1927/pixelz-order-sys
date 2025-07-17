using PixelzOrderSystem.Shared.Dto.Invoices;

namespace PixelzOrderSystem.Shared.Services.Invoices;

public class MockInvoiceService(ILogger<MockInvoiceService> logger): IInvoiceService
{
    public async Task<CreateInvoiceResult> CreateInvoiceAsync(CreateInvoiceRequest request)
    {
        try
        {
            await Task.Delay(1000);

            var invoiceId = Guid.NewGuid().ToString();
            var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{invoiceId.Substring(0, 8)}";

            return new CreateInvoiceResult
            {
                IsSuccess = true,
                InvoiceId = invoiceId,
                InvoiceNumber = invoiceNumber,
                Message = "Hóa đơn đã được tạo thành công."
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating invoice for order {OrderId}", request.OrderId);
            return new CreateInvoiceResult
            {
                IsSuccess = false,
                Message = "Failed to create invoice."
            };
        }
    }
}