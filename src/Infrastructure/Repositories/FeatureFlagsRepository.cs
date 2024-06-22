using Domain.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Repositories.BASE;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Feature flags provider
/// </summary>
/// <remarks>
/// ctor
/// </remarks>
public class FeatureFlagsRepository(
    Context.Context context) : GenericEntityCoreRepository<FeatureFlagsEntity>(context), IFeatureFlagsRepository
{
    private readonly Context.Context _context = context;

    /// <summary>
    /// Recupera uma feature flag
    /// </summary>
    /// <param name="featureName"></param>
    /// <returns></returns>
    public async Task<FeatureFlagsEntity> GetFeatureDefinitionAsync(string featureName)
        => await _context.FeatureFlags.SingleOrDefaultAsync(f => f.Name == featureName);

    /// <summary>
    /// Retorna todas as featuire flags.
    /// </summary>
    /// <returns></returns>
    public IAsyncEnumerable<FeatureFlagsEntity> GetAllFeatureDefinitionsAsync()
        => _context.FeatureFlags.AsAsyncEnumerable();
}
