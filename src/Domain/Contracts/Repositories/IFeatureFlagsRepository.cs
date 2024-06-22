using Domain.Contracts.Repositories.Base;
using Domain.Entities;

namespace Domain.Contracts.Repositories;

/// <summary>
/// Interface de feature flags
/// </summary>
public interface IFeatureFlagsRepository
    : IGenericRepository<FeatureFlagsEntity>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="featureName"></param>
    /// <returns></returns>
    Task<FeatureFlagsEntity> GetFeatureDefinitionAsync(string featureName);

    /// <summary>
    /// Retonrna todas as featuire flags.
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<FeatureFlagsEntity> GetAllFeatureDefinitionsAsync();
}
