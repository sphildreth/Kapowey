namespace Kapowey.Core.Common.Models.API
{
    public class RequestException : Exception
    {
        public RequestException() : base()
        {
        }

        public RequestException(string message) : base(message)
        {
        }

        public RequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}