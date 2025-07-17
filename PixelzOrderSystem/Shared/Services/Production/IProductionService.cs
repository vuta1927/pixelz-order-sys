using PixelzOrderSystem.Shared.Dto.Production;

namespace PixelzOrderSystem.Shared.Services.Production;

public interface IProductionService
{
    Task<ProductionResult> PushOrderAsync(ProductionOrderRequest request);
}