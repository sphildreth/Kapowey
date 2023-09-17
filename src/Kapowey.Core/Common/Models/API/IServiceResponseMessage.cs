namespace Kapowey.Core.Common.Models.API
{
    public interface IServiceResponseMessage
    {
        string Message { get; }
        ServiceResponseMessageType MessageType { get; }
    }
}
