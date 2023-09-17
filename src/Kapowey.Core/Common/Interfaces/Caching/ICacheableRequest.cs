using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Kapowey.Core.Common.Interfaces.Caching;

public interface ICacheableRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey { get=>String.Empty; }
    MemoryCacheEntryOptions? Options { get; }
}
