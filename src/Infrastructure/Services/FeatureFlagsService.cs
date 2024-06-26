﻿using Domain.Contracts.Repositories;
using Domain.Contracts.Repositories.Base;
using Domain.Contracts.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions.Base;
using Domain.Helpers;
using Infrastructure.Repositories;
using System.Net;

namespace Infrastructure.Services;

/// <summary>
/// Serviços de flags do sistema.
/// </summary>
/// <param name="featureFlags"></param>
/// <param name="unitOfWork"></param>
public class FeatureFlagsService(
    FeatureFlagsRepository featureFlags, IUnitOfWork<Context.Context> unitOfWork) : IFeatureFlagsService
{
    private readonly IFeatureFlagsRepository _featureFlags = featureFlags;
    private readonly IUnitOfWork<Context.Context> _unitOfWork = unitOfWork;

    /// <summary>
    /// Verifica se endponint está cadastro, caso esteja verifica se esta ativo, se não cadastra.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodName"></param>
    /// <param name="method"></param>
    /// <param name="methodDescription"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    public async Task<T> ExecuteAsync<T>(string methodName, Func<Task<T>> method, string methodDescription)
    {
        var featureFlag
            = await _featureFlags.GetFeatureDefinitionAsync(methodName)
            ?? await _featureFlags.CreateAsync(new FeatureFlagsEntity
            {
                Name = methodName,
                Created = DateTime.UtcNow,
                IsEnabled = true,
                Status = Status.Ativo,

            }).ContinueWith(async (taskResult) =>
            {
                await _unitOfWork.CommitAsync();

                return taskResult.Result;

            }).Result;

        if (featureFlag.IsEnabled is false || featureFlag.Status is Status.Inativo)
            throw new CustomException(HttpStatusCode.NotImplemented, null, [
                new($"Método {methodName} inativado!")
            ]);

        return await Tracker.Time(
            () => method(), $"{methodName} - {methodDescription}");
    }
}