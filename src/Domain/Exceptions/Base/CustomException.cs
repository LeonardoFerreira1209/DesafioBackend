using Domain.Dtos.Response.Base;
using System.Net;

namespace Domain.Exceptions.Base;

/// <summary>
/// Exception customizado.
/// </summary>
public class CustomException : BaseException
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="dados"></param>
    /// <param name="notificacoes"></param>
    public CustomException(
        HttpStatusCode statusCode,
        object dados, List<DadosNotificacao> notificacoes)
    {
        Response = new ErrorResponse
            (statusCode, dados, notificacoes);
    }
}
