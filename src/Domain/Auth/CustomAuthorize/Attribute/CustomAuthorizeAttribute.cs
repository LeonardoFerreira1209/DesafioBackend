using Domain.Auth.CustomAuthorize.Filter;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Domain.Auth.CustomAuthorize.Attribute;

/// <summary>
/// Classe de autorização customizada.
/// </summary>
public class CustomAuthorizeAttribute : TypeFilterAttribute
{
    /// <summary>
    /// Atributo de autorização customizavel.
    /// </summary>
    /// <param name="claim"></param>
    /// <param name="values"></param>
    public CustomAuthorizeAttribute(Claims claim, params string[] values) : base(typeof(CustomAuthorizeFilter))
    {
        Arguments = [
            values.Select(value => new Claim(claim.ToString(), value)).ToList()
        ];
    }
}
