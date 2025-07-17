using PixelzOrderSystem.Shared.Dto;
using PixelzOrderSystem.Shared.Dto.Production;

namespace PixelzOrderSystem.Infrastructure.Services.Production;

public interface IProductionService
{
    Task<ProductionResult> PushOrderAsync(ProductionOrderRequest request)
}