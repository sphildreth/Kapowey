namespace Kapowey.Core.Common.ExceptionHandlers;

public class ForbiddenException : ServerException
{
    public ForbiddenException(string message) : base(message,System.Net.HttpStatusCode.Forbidden) { }
}
