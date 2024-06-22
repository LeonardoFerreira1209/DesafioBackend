using Domain.Auth.Token;
using Domain.Contracts.Repositories.Base;
using Domain.Contracts.Services;
using Domain.Dtos.Request;
using Domain.Dtos.Response;
using Domain.Dtos.Response.Base;
using Domain.Entities;
using Domain.Helpers.Extensions;
using Domain.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using static Domain.Exceptions.CustomUserException;
using IAuthenticationService = Domain.Contracts.Services.IAuthenticationService;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de Authenticação do sistema.
/// </summary>
/// <remarks>
/// ctor
/// </remarks>
public sealed class AuthenticationService(
    UserManager<UserEntity> userManager,
    SignInManager<UserEntity> signInManager,
    ITokenService tokenService,
    IUnitOfWork<Context.Context> unitOfWork) : IAuthenticationService
{
    /// <summary>
    /// Método de registro de usuário.
    /// </summary>
    /// <param name="registerUserRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="CreateUserFailedException"></exception>
    public async Task<ObjectResult> RegisterAsync(
        RegisterUserRequest registerUserRequest, CancellationToken cancellationToken)
    {
        Log.Information(
            $"[LOG INFORMATION] - SET TITLE {nameof(AuthenticationService)} - METHOD {nameof(RegisterAsync)}\n");

        try
        {
            await new RegisterUserRequestValidator()
                .ValidateAsync(registerUserRequest, cancellationToken)
                    .ContinueWith(async (validationTask) =>
                    {
                        var validation = validationTask.Result;

                        if (validation.IsValid is false) await validation.GetValidationErrors();

                    }).Unwrap();

            var transaction =
                await unitOfWork.BeginTransactAsync();

            try
            {
                var userEntity
                    = registerUserRequest.ToUserEntity();

                return await userManager
                    .CreateAsync(userEntity, registerUserRequest.Password).ContinueWith(async (identityResultTask) =>
                    {
                        var identityResult
                                = identityResultTask.Result;

                        if (identityResult.Succeeded is false)
                            throw new CreateUserFailedException(
                                registerUserRequest, identityResult.Errors.Select((e)
                                    => new DadosNotificacao(e.Description)).ToList());

                        return await userManager.AddToRoleAsync(
                            userEntity, "System").ContinueWith(identityResultTask =>
                            {
                                var identityResult
                                        = identityResultTask.Result;

                                if (identityResult.Succeeded is false)
                                    throw new UserToRoleFailedException(
                                        registerUserRequest, identityResult.Errors.Select((e)
                                            => new DadosNotificacao(e.Description)).ToList());

                                return new OkObjectResult(
                                    new ApiResponse<UserResponse>(
                                        identityResult.Succeeded,
                                        HttpStatusCode.Created,
                                        userEntity.ToResponse(), [
                                            new DadosNotificacao("Usuário criado com sucesso!")]));
                            });

                    }).Unwrap();
            }
            catch
            {
                transaction.Rollback(); throw;
            }
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception: {exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsável por fazer a authorização do usuário.
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundUserException"></exception>
    public async Task<ObjectResult> AuthenticationAsync(LoginRequest loginRequest)
    {
        Log.Information(
            $"[LOG INFORMATION] - SET TITLE {nameof(AuthenticationService)} - METHOD {nameof(AuthenticationAsync)}\n");

        try
        {
            await new LoginRequestValidator().ValidateAsync(
                loginRequest).ContinueWith(async (validationTask) =>
                {
                    var validation = validationTask.Result;

                    if (validation.IsValid is false)
                        await validation.GetValidationErrors();

                }).Unwrap();

            return await userManager.FindByNameAsync(
                loginRequest.Username).ContinueWith(async (userEntityTask) =>
                    {
                        var userEntity =
                            userEntityTask.Result
                            ?? throw new NotFoundUserException(loginRequest);

                        await signInManager.PasswordSignInAsync(
                            userEntity, loginRequest.Password, true, true).ContinueWith((signInResultTask) =>
                            {
                                var signInResult = signInResultTask.Result;

                                if (signInResult.Succeeded is false)
                                    ThrownAuthorizationException(signInResult, userEntity.Id, loginRequest);
                            });

                        Log.Information(
                            $"[LOG INFORMATION] - Usuário autenticado com sucesso!\n");

                        return await GenerateTokenJwtAsync(loginRequest).ContinueWith(
                            (tokenJwtTask) =>
                            {
                                var tokenJWT =
                                    tokenJwtTask.Result
                                    ?? throw new TokenJwtException(null);

                                Log.Information(
                                    $"[LOG INFORMATION] - Token gerado com sucesso {JsonConvert.SerializeObject(tokenJWT)}!\n");

                                return new OkObjectResult(
                                    new ApiResponse<TokenJWT>(
                                        true, HttpStatusCode.Created, tokenJWT, [
                                            new("Token criado com sucesso!")]));
                            });

                    }).Unwrap();
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsavel por gerar um tokenJwt.
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    private async Task<TokenJWT> GenerateTokenJwtAsync(LoginRequest loginRequest)
        => await tokenService.CreateJsonWebToken(
            loginRequest.Username).ContinueWith((tokenTask) =>
            {
                var tokenJwt =
                    tokenTask.Result
                    ?? throw new TokenJwtException(loginRequest);

                return tokenJwt;
            });

    /// <summary>
    /// Método responsável por tratar os erros de autenticação.
    /// </summary>
    /// <param name="signInResult"></param>
    /// <returns></returns>
    /// <exception cref="LockedOutAuthenticationException"></exception>
    /// <exception cref="IsNotAllowedAuthenticationException"></exception>
    /// <exception cref="RequiresTwoFactorAuthenticationException"></exception>
    /// <exception cref="InvalidUserAuthenticationException"></exception>
    private static void ThrownAuthorizationException(
        SignInResult signInResult, Guid userId, LoginRequest loginRequest)
    {
        if (signInResult.IsLockedOut)
        {
            Log.Information($"[LOG INFORMATION] - Falha ao autenticar usuário, está bloqueado.\n");

            throw new LockedOutAuthenticationException(loginRequest);
        }
        else if (signInResult.IsNotAllowed)
        {
            Log.Information($"[LOG INFORMATION] - Falha ao autenticar usuário, não está confirmado.\n");

            throw new IsNotAllowedAuthenticationException(new
            {
                userId,
                isNotAllowed = true,
                loginRequest
            });
        }
        else if (signInResult.RequiresTwoFactor)
        {
            Log.Information($"[LOG INFORMATION] - Falha ao autenticar usuário, requer verificação de dois fatores.\n");

            throw new RequiresTwoFactorAuthenticationException(loginRequest);
        }
        else
        {
            Log.Information($"[LOG INFORMATION] - Falha na autenticação dados incorretos!\n");

            throw new InvalidUserAuthenticationException(loginRequest);
        }
    }
}
