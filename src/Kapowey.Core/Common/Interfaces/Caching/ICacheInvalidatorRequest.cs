using MediatR;

namespace Kapowey.Core.Common.Interfaces.Caching;

public interface ICacheInvalidatorRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey { get => String.Empty; } 
    CancellationTokenSource? SharedExpiryTokenSource { get; }
}
