using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

/// <summary>
/// Classe de entidade de usuário.
/// </summary>
public class UserEntity
    : IdentityUser<Guid>, IEntityBase
{
    /// <summary>
    /// ctor
    /// </summary>
    public UserEntity()
    {

    }

    /// <summary>
    /// ctor
    /// </summary>
    public UserEntity(string firstName, string lastName,
        string userName, string email, string phoneNumber, Status status,
        DateTime created, bool emailConfirmed,
        DateTime? updated = null)
    {
        FirstName = firstName;
        LastName = lastName;
        UserName = userName;
        Email = email;
        EmailConfirmed = emailConfirmed;
        PhoneNumber = phoneNumber;
        Status = status;
        Created = created;
        Updated = updated;
    }

    /// <summary>
    /// Nome do usuário.
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// Ultimo nome do usuário.
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    /// Data de criação.
    /// </summary>
    public DateTime Created { get; private set; }

    /// <summary>
    /// Data de atualização.
    /// </summary>
    public DateTime? Updated { get; private set; }

    /// <summary>
    /// Status do cadastro.
    /// </summary>
    public Status Status { get; private set; }
}
