﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using static Domain.Exceptions.CustomUserException;

namespace Application.Configurations.Extensions.Initializers;

/// <summary>
/// Classe de configuração do Autenticação da aplicação.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Configuração da autenticação do sistema.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configurations"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configurations)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            options.DefaultScheme = IdentityConstants.ApplicationScheme;

        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = async context =>
                {
                    context.Options.TokenValidationParameters = new TokenValidationParameters
                    {
                        LogValidationExceptions = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromHours(3),

                        ValidIssuer = configurations.GetValue<string>("Auth:ValidIssuer"),
                        ValidAudience = configurations.GetValue<string>("Auth:ValidAudience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configurations.GetValue<string>("Auth:SecurityKey")))
                    };

                    await Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Log.Error($"[LOG ERROR] {nameof(JwtBearerEvents)} - METHOD OnAuthenticationFailed - {context.Exception.Message}\n");

                    throw new UnauthorizedUserException("Token do usuário não é permitido ou está incorreto!");
                },
                OnTokenValidated = context =>
                {
                    Log.Information($"[LOG INFORMATION] {nameof(JwtBearerEvents)} - OnTokenValidated - {context.SecurityToken}\n");

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}
