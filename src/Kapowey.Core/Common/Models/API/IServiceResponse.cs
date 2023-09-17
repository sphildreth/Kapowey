namespace Kapowey.Core.Common.Models.API
{
    public interface IServiceResponse<T> : IResponse
    {
        object Id { get;}

        T Data { get; }
    }
}