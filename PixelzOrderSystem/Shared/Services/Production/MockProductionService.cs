using PixelzOrderSystem.Shared.Dto.Production;

namespace PixelzOrderSystem.Shared.Services.Production;

public class MockProductionService(ILogger<MockProductionService> logger): IProductionService
{
    public async Task<ProductionResult> PushOrderAsync(ProductionOrderRequest request)
    {
        try
        {
            await Task.Delay(1000);

            // Log the order details
            logger.LogInformation("Production order pushed successfully: {@Order}", request);

            return new ProductionResult
            {
                IsSuccess = true,
                Message = "Đơn hàng đã được đẩy vào Production Service thành công."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing production order: {@Order}", request);
            return new ProductionResult
            {
                IsSuccess = false,
                Message = "Failed to process production order."
            };
        }
    }
}