namespace Kapowey.Core.Common.ExceptionHandlers;

public class NotFoundException : ServerException
{


    public NotFoundException(string message)
        : base(message,System.Net.HttpStatusCode.NotFound)
    {
    }
   public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.", System.Net.HttpStatusCode.NotFound)
    {
    }
}
