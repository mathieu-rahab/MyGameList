using System.Net;

namespace Mygamelist.Core.Exceptions;

public class BusinessException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorCode { get; }

    public BusinessException(HttpStatusCode statusCode, string errorCode, string message = "") 
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
