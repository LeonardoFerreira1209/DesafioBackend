using Domain.Dtos.Request;
using Domain.Enums;
using Domain.Helpers.Extensions;
using FluentValidation;

namespace Domain.Validators;

/// <summary>
/// Classe de validação de request de login.
/// </summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    // <summary>
    /// ctor
    /// </summary>
    public LoginRequestValidator()
    {
        RuleFor(t => t.Username)
            .NotEmpty()
            .NotNull()
            .WithErrorCode(ErrorCode.ErroInesperado.ToCode())
            .WithMessage("Preencha o campo username.");

        RuleFor(t => t.Password)
            .NotEmpty()
            .NotNull()
            .WithErrorCode(ErrorCode.ErroInesperado.ToCode())
            .WithMessage("Preencha o campo password.");
    }
}
