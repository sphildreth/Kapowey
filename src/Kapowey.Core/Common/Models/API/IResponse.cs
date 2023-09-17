namespace Kapowey.Core.Common.Models.API
{
    public interface IResponse
    {
        bool IsSuccess { get; }
        IEnumerable<IServiceResponseMessage> Messages { get; }
    }
}