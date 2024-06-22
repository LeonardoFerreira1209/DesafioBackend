using Domain.Dtos.Request;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Builders.Creates;

/// <summary>
/// Construtor de usuários.
/// </summary>
public static class CreateUser
{
    /// <summary>
    /// Constroi usuário.
    /// </summary>
    /// <param name="registerUserRequest"></param>
    /// <returns></returns>
    public static UserEntity CreateDefault(this RegisterUserRequest registerUserRequest)
        => new UserBuilder()
            .AddFirstName(registerUserRequest.FirstName)
                .AddUserName(registerUserRequest.UserName)
                    .AddLastName(registerUserRequest.LastName)
                        .AddEmail(registerUserRequest.Email)
                            .AddPhoneNumber(registerUserRequest.PhoneNumber)
                                .AddCreated(DateTime.Now)
                                    .AddEmailConfirmed(true)
                                        .AddStatus(Status.Ativo)
                                              .Builder();
}
